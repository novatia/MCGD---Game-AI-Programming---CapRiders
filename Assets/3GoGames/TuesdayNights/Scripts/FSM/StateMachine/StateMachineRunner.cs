using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace StateMachine
{
	public class StateMachineRunner : MonoBehaviour
	{
		private List<IStateMachine> m_StateMachineList = new List<IStateMachine>();

        // BUSINESS LOGIC

        /// <summary>
        /// Creates a StateMachine token object which is used to managed to the state of a Monobehaviour. 
        /// </summary>
        public StateMachine<T> Initialize<T>(MonoBehaviour i_Component) where T : struct, IConvertible, IComparable
		{
            StateMachine<T> fsm = new StateMachine<T>(this, i_Component);
            m_StateMachineList.Add(fsm);

			return fsm;
		}

        /// <summary>
        /// Creates a StateMachine token object which is used to managed to the state of a monobehaviour. Will automatically transition the i_StartState.
        /// </summary>
        public StateMachine<T> Initialize<T>(MonoBehaviour i_Component, T i_StartState) where T : struct, IConvertible, IComparable
		{
            StateMachine<T> fsm = Initialize<T>(i_Component);
			fsm.ChangeState(i_StartState);

			return fsm;
		}

        // MonoBehaviour's INTERFACE

        void FixedUpdate()
		{
			for (int stateMachineIndex = 0; stateMachineIndex < m_StateMachineList.Count; ++stateMachineIndex)
			{
                IStateMachine fsm = m_StateMachineList[stateMachineIndex];

                if (!fsm.isInTransition && fsm.component.enabled)
                {
                    fsm.currentStateMapping.FixedUpdate();
                }
			}
		}

		void Update()
		{
            for (int stateMachineIndex = 0; stateMachineIndex < m_StateMachineList.Count; ++stateMachineIndex)
            {
                IStateMachine fsm = m_StateMachineList[stateMachineIndex];

                if (!fsm.isInTransition && fsm.component.enabled)
				{
					fsm.currentStateMapping.Update();
				}
			}
		}

		void LateUpdate()
		{
            for (int stateMachineIndex = 0; stateMachineIndex < m_StateMachineList.Count; ++stateMachineIndex)
            {
                IStateMachine fsm = m_StateMachineList[stateMachineIndex];

                if (!fsm.isInTransition && fsm.component.enabled)
				{
					fsm.currentStateMapping.LateUpdate();
				}
			}
		}

        // UTILS

		public static void DoNothing()
		{

		}

		public static IEnumerator DoNothingCoroutine()
		{
			yield break;
		}
	}
}


