using System;
using System.Collections;
using System.Collections.Generic;

using System.Reflection;
using System.Runtime.CompilerServices;

using UnityEngine;

using Object = System.Object;

namespace StateMachine
{
	public interface IStateMachine
	{
		MonoBehaviour component { get; }

		StateMapping currentStateMapping { get; }
		bool isInTransition { get; }
	}

	public class StateMachine<T> : IStateMachine where T : struct, IConvertible, IComparable
	{
        private MonoBehaviour m_Component = null;
        private StateMachineRunner m_StateMachineRunner = null;

        private Dictionary<object, StateMapping> m_StateLookup = null;

        private StateMapping m_PrevState = null;
        private StateMapping m_CurrentState = null;

        private StateMapping m_DestinationState = null;

		private bool m_IsInTransition = false;

		private IEnumerator m_CurrentTransition;

		private IEnumerator m_EnterRoutine;
        private IEnumerator m_ExitRoutine;
		private IEnumerator m_QueuedChange;

        private event Action<T> m_OnStateChanged;

        // ACCESSORS

        public MonoBehaviour component
        {
            get
            {
                return m_Component;
            }
        }

        public StateMapping currentStateMapping
        {
            get
            {
                return m_CurrentState;
            }
        }

        public T prevState
        {
            get
            {
                if (m_PrevState == null)
                {
                    return default(T);
                }

                return (T)m_PrevState.state;
            }
        }

        public T currentState
        {
            get
            {
                if (m_CurrentState == null)
                {
                    return default(T);
                }

                return (T)m_CurrentState.state;
            }
        }

        public bool isInTransition
        {
            get
            {
                return m_IsInTransition;
            }
        }

        public event Action<T> onStateChanged
        {
            add
            {
                m_OnStateChanged += value;
            }

            remove
            {
                m_OnStateChanged -= value;
            }
        }

        // BUSINESS LOGIC

		public void ChangeState(T i_NewState)
		{
            ChangeToNewState(i_NewState);
		}

        // CTOR

        public StateMachine(StateMachineRunner i_StateMachineRunner, MonoBehaviour i_Component)
        {
            m_StateMachineRunner = i_StateMachineRunner;

            m_Component = i_Component;

            // Define states.

            Array values = Enum.GetValues(typeof(T));
            if (values.Length < 1)
            {
                throw new ArgumentException("Enum provided to Initialize must have at least 1 visible definition.");
            }

            m_StateLookup = new Dictionary<object, StateMapping>();
            for (int valueIndex = 0; valueIndex < values.Length; ++valueIndex)
            {
                StateMapping mapping = new StateMapping((Enum)values.GetValue(valueIndex));
                m_StateLookup.Add(mapping.state, mapping);
            }

            // Reflect methods.

            MethodInfo[] methods = i_Component.GetType().GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic);

            // Bind methods to states.

            string separatorString = "_";
            char[] separator = separatorString.ToCharArray();

            for (int methodIndex = 0; methodIndex < methods.Length; ++methodIndex)
            {
                MethodInfo methodInfo = methods[methodIndex];

                if (methodInfo.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Length != 0)
                {
                    continue;
                }

                string methodName = methodInfo.Name;

                string[] nameSplit = methodName.Split(separator);

                // Ignore functions without an underscore.

                if (nameSplit.Length <= 1)
                {
                    continue;
                }

                Enum key;
                try
                {
                    key = (Enum)Enum.Parse(typeof(T), nameSplit[0]);
                }
                catch (ArgumentException)
                {

                    continue; // Selected method is not list in state enum.
                }

                StateMapping targetMapping = m_StateLookup[key];

                switch (nameSplit[1])
                {
                    case "Enter":

                        if (methodInfo.ReturnType == typeof(IEnumerator))
                        {
                            targetMapping.hasEnterRoutine = true;
                            targetMapping.EnterRoutine = CreateDelegate<Func<IEnumerator>>(methodInfo, i_Component);
                        }
                        else
                        {
                            targetMapping.hasEnterRoutine = false;
                            targetMapping.EnterCall = CreateDelegate<Action>(methodInfo, i_Component);
                        }

                        break;

                    case "Exit":

                        if (methodInfo.ReturnType == typeof(IEnumerator))
                        {
                            targetMapping.hasExitRoutine = true;
                            targetMapping.ExitRoutine = CreateDelegate<Func<IEnumerator>>(methodInfo, i_Component);
                        }
                        else
                        {
                            targetMapping.hasExitRoutine = false;
                            targetMapping.ExitCall = CreateDelegate<Action>(methodInfo, i_Component);
                        }

                        break;

                    case "Finally":

                        targetMapping.Finally = CreateDelegate<Action>(methodInfo, i_Component);

                        break;

                    case "FixedUpdate":

                        targetMapping.FixedUpdate = CreateDelegate<Action>(methodInfo, i_Component);

                        break;

                    case "Update":

                        targetMapping.Update = CreateDelegate<Action>(methodInfo, i_Component);

                        break;

                    case "LateUpdate":

                        targetMapping.LateUpdate = CreateDelegate<Action>(methodInfo, i_Component);

                        break;
                }
            }

            // Create nil state mapping.

            m_CurrentState = new StateMapping(null);
        }

        // INTERNALS

        private V CreateDelegate<V>(MethodInfo i_Method, Object i_Target) where V : class
        {
            V ret = (Delegate.CreateDelegate(typeof(V), i_Target, i_Method) as V);

            if (ret == null)
            {
                throw new ArgumentException("Unabled to create delegate for method called " + i_Method.Name);
            }

            return ret;
        }

        private void ChangeToNewState(T i_NewState)
        {
            if (m_StateLookup == null)
            {
                throw new Exception("States have not been configured, please call initialized before trying to set state.");
            }

            if (!m_StateLookup.ContainsKey(i_NewState))
            {
                throw new Exception("No state with the name " + i_NewState.ToString() + " can be found. Please make sure you are called the correct type the statemachine was initialized with.");
            }

            StateMapping nextState = m_StateLookup[i_NewState];

            if (m_CurrentState == nextState)
                return;

            // Cancel any queued changes.

            if (m_QueuedChange != null)
            {
                m_StateMachineRunner.StopCoroutine(m_QueuedChange);
                m_QueuedChange = null;
            }

            if (m_IsInTransition)
            {
                if (m_ExitRoutine != null) // We are already exiting current state on our way to our previous target state.
                {
                    m_DestinationState = nextState; // Overwrite with our new target.
                    return;
                }

                if (m_EnterRoutine != null) // We are already entering our previous target state. Need to wait for that to finish and call the exit routine.
                {
                    m_QueuedChange = WaitForPreviousTransition(nextState);
                    m_StateMachineRunner.StartCoroutine(m_QueuedChange);
                    return;
                }
            }

            if ((m_CurrentState != null && m_CurrentState.hasExitRoutine) || nextState.hasEnterRoutine)
            {
                m_IsInTransition = true;

                m_CurrentTransition = ChangeToNewStateRoutine(nextState);
                m_StateMachineRunner.StartCoroutine(m_CurrentTransition);
            }
            else // Same frame transition, no coroutines are present.
            {
                if (m_CurrentState != null)
                {
                    m_CurrentState.ExitCall();
                    m_CurrentState.Finally();
                }

                m_PrevState = m_CurrentState;
                m_CurrentState = nextState;

                if (m_CurrentState != null)
                {
                    m_CurrentState.EnterCall();

                    if (m_OnStateChanged != null)
                    {
                        m_OnStateChanged((T)m_CurrentState.state);
                    }
                }

                m_IsInTransition = false;
            }
        }

        private IEnumerator ChangeToNewStateRoutine(StateMapping i_NewState)
		{
			m_DestinationState = i_NewState; // Chache this so that we can overwrite it and hijack a transition.

			if (m_CurrentState != null)
			{
				if (m_CurrentState.hasExitRoutine)
				{
					m_ExitRoutine = m_CurrentState.ExitRoutine();

					if (m_ExitRoutine != null)
					{
						yield return m_StateMachineRunner.StartCoroutine(m_ExitRoutine);
					}

                    m_ExitRoutine = null;
				}
				else
				{
                    m_CurrentState.ExitCall();
				}

                m_CurrentState.Finally();
			}

			m_PrevState = m_CurrentState;
            m_CurrentState = m_DestinationState;

			if (m_CurrentState != null)
			{
				if (m_CurrentState.hasEnterRoutine)
				{
					m_EnterRoutine = m_CurrentState.EnterRoutine();

					if (m_EnterRoutine != null)
					{
						yield return m_StateMachineRunner.StartCoroutine(m_EnterRoutine);
					}

                    m_EnterRoutine = null;
				}
				else
				{
                    m_CurrentState.EnterCall();
				}

				// Broadcast change only after enter transition has begun.

				if (m_OnStateChanged != null)
				{
                    m_OnStateChanged((T)m_CurrentState.state);
				}
			}

			m_IsInTransition = false;
		}

		private IEnumerator WaitForPreviousTransition(StateMapping i_NextState)
		{
			while (m_IsInTransition)
			{
				yield return null;
			}

			ChangeState((T)i_NextState.state);
		}

		// STATIC METHODS

		/// <summary>
		/// Inspects a MonoBehaviour for state methods as definied by the supplied Enum, and returns a stateMachine instance used to trasition states.
		/// </summary>
		public static StateMachine<T> Initialize(MonoBehaviour i_Component)
		{
            StateMachineRunner stateMachineRunner = i_Component.GetComponent<StateMachineRunner>();
            if (stateMachineRunner == null)
            {
                stateMachineRunner = i_Component.gameObject.AddComponent<StateMachineRunner>();
            }

			return stateMachineRunner.Initialize<T>(i_Component);
		}

		/// <summary>
		/// Inspects a MonoBehaviour for state methods as definied by the supplied Enum, and returns a stateMachine instance used to trasition states. 
		/// </summary>
		public static StateMachine<T> Initialize(MonoBehaviour i_Component, T i_StartState)
		{
            StateMachineRunner stateMachineRunner = i_Component.GetComponent<StateMachineRunner>();
            if (stateMachineRunner == null)
            {
                stateMachineRunner = i_Component.gameObject.AddComponent<StateMachineRunner>();
            }

            return stateMachineRunner.Initialize<T>(i_Component, i_StartState);
        }
	}
}
