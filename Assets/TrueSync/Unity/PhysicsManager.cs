using UnityEngine;

using System;

namespace TrueSync
{
    public static class PhysicsManager
    {
        // Fields

        private static IPhysicsManager m_Instance = null;

        // ACCESSORS

        public static IPhysicsManager Instance
        {
            get
            {
                return m_Instance;
            }
        }

        // LOGIC

        public static void Initialize(IPhysicsManager i_Instance)
        {
            m_Instance = i_Instance;
        }

        public static void Cleanup()
        {
            m_Instance = null;
        }

        // IPhysicsManager's PROXY

        public static void Init()
        {
            if (m_Instance != null)
            {
                m_Instance.Init();
            }
        }

        public static GameObject GetGameObject(IBody i_RigidBody)
        {
            if (m_Instance != null)
            {
                return m_Instance.GetGameObject(i_RigidBody);
            }

            return null;
        }

        public static IWorld GetWorld()
        {
            if (m_Instance != null)
            {
                return m_Instance.GetWorld();
            }

            return null;
        }

        public static IWorldClone GetWorldClone()
        {
            if (m_Instance != null)
            {
                return m_Instance.GetWorldClone();
            }

            return null;
        }

        public static void AddBody(ICollider i_Collider)
        {
            if (m_Instance != null)
            {
                m_Instance.AddBody(i_Collider);
            }
        }

        public static void RemoveBody(IBody i_RigidBody)
        {
            if (m_Instance != null)
            {
                m_Instance.RemoveBody(i_RigidBody);
            }
        }

        public static void OnRemoveBody(Action<IBody> i_OnRemoveBody)
        {
            if (m_Instance != null)
            {
                m_Instance.OnRemoveBody(i_OnRemoveBody);
            }
        }

        public static void UpdateStep()
        {
            if (m_Instance != null)
            {
                m_Instance.UpdateStep();
            }
        }

        public static bool IsCollisionEnabled(IBody i_RigidBody1, IBody i_RigidBody2)
        {
            if (m_Instance != null)
            {
                return m_Instance.IsCollisionEnabled(i_RigidBody1, i_RigidBody2);
            }

            return false;
        }
    }
}
