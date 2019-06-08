using UnityEngine;

using System;
using System.Collections.Generic;

namespace TrueSync
{
    public class Physics2DWorldManager : IPhysicsManager
    {
        // Fields

        private Physics2D.World m_World = null;

        private Dictionary<IBody, GameObject> m_GameObjectMap = null;
        private TSCollision2D m_CollisionCache = null;

        private List<TSRigidBody2D> m_RigidBodies = null;

        // IPhysicsManager's interface

        public TSVector Gravity
        {
            get;
            set;
        }

        public FP LockedTimeStep
        {
            get;
            set;
        }

        public bool SpeculativeContacts
        {
            get;
            set;
        }

        public void AddBody(ICollider i_Collider)
        {
            if (i_Collider == null)
                return;

            bool is2dCollider = (i_Collider is TSCollider2D);

            Debug.Assert(is2dCollider, "3D Physics is not supported.");

            if (!is2dCollider)
                return;

            TSCollider2D tsCollider = (TSCollider2D)i_Collider;

            Debug.Assert(tsCollider.body == null, "Body already added.");

            if (tsCollider.body != null)
                return;

            tsCollider.Initialize(m_World);

            m_GameObjectMap[tsCollider.body] = tsCollider.gameObject;

            Transform parent = tsCollider.transform.parent;
            TSCollider2D parentCollider = (parent != null) ? parent.GetComponentInParent<TSCollider2D>() : null;
            if (parentCollider != null)
            {
                Physics2D.Body childBody = tsCollider.body;

                TSTransform2D parentTransform = parentCollider.GetComponent<TSTransform2D>();
                TSTransform2D colliderTransform = tsCollider.GetComponent<TSTransform2D>();

                childBody.bodyConstraints.Add(new ConstraintHierarchy2D(parentCollider.body, tsCollider.body, (colliderTransform.position + tsCollider.center) - (parentTransform.position + parentCollider.center)));
            }

            TSRigidBody2D attachedRigidBody = tsCollider.GetComponent<TSRigidBody2D>();
            if (attachedRigidBody != null)
            {
                m_RigidBodies.Add(attachedRigidBody);
            }

            m_World.ProcessAddedBodies();
        }

        public void RemoveBody(IBody i_Body)
        {
            m_World.RemoveBody((Physics2D.Body)i_Body);
            m_World.ProcessRemovedBodies();
        }

        public void OnRemoveBody(Action<IBody> i_OnRemoveBody)
        {
            m_World.BodyRemoved += delegate (Physics2D.Body body) { i_OnRemoveBody(body); };
        }

        public GameObject GetGameObject(IBody i_Body)
        {
            GameObject outGo;
            m_GameObjectMap.TryGetValue(i_Body, out outGo);

            return outGo;
        }

        public int GetBodyLayer(IBody i_Body)
        {
            return 0;
        }

        public IWorld GetWorld()
        {
            return m_World;
        }

        public IWorldClone GetWorldClone()
        {
            return new WorldClone2D();
        }

        public void Init()
        {
            ChecksumExtractor.Init(this);

            Physics2D.Settings.ContinuousPhysics = SpeculativeContacts;

            TSVector2 gravity = new TSVector2(Gravity.x, Gravity.y);
            m_World = new Physics2D.World(gravity);

            Physics2D.ContactManager.physicsManager = this;

            m_World.BodyRemoved += OnRemovedRigidbody;

            m_World.ContactManager.BeginContact += CollisionEnter;
            m_World.ContactManager.StayContact += CollisionStay;
            m_World.ContactManager.EndContact += CollisionExit;
        }

        public bool IsCollisionEnabled(IBody i_RigidBody1, IBody i_RigidBody2)
        {
            return true;
        }

        public void UpdateStep()
        {
            InternalStep();
            InternalUpdateTransformCache();
        }

        // CALLBACKS

        private void OnRemovedRigidbody(Physics2D.Body i_Body)
        {
            GameObject go = m_GameObjectMap[i_Body];

            // Remove cached rb.
            if (go != null)
            {
                TSRigidBody2D attachedRigidBody = go.GetComponent<TSRigidBody2D>();
                if (attachedRigidBody != null)
                {
                    m_RigidBodies.Remove(attachedRigidBody);
                }
            }

            // Destroy object.

            if (go != null)
            {
                GameObject.Destroy(go);
            }

            // Clear object map.

            m_GameObjectMap.Remove(i_Body);
        }

        private bool CollisionEnter(Physics2D.Contact i_Contact)
        {
            if (i_Contact.FixtureA.IsSensor || i_Contact.FixtureB.IsSensor)
            {
                TriggerEnter(i_Contact);
                return true;
            }

            CallSyncedCollisionEnter(i_Contact.FixtureA.Body, i_Contact.FixtureB.Body, i_Contact);

            return true;
        }

        private void CollisionStay(Physics2D.Contact i_Contact)
        {
            if (i_Contact.FixtureA.IsSensor || i_Contact.FixtureB.IsSensor)
            {
                TriggerStay(i_Contact);
                return;
            }

            CallSyncedCollisionStay(i_Contact.FixtureA.Body, i_Contact.FixtureB.Body, i_Contact);
        }

        private void CollisionExit(Physics2D.Contact i_Contact)
        {
            if (i_Contact.FixtureA.IsSensor || i_Contact.FixtureB.IsSensor)
            {
                TriggerExit(i_Contact);
                return;
            }

            CallSyncedCollisionExit(i_Contact.FixtureA.Body, i_Contact.FixtureB.Body, null);
        }

        private void TriggerEnter(Physics2D.Contact i_Contact)
        {
            CallSyncedTriggerEnter(i_Contact.FixtureA.Body, i_Contact.FixtureB.Body, i_Contact);
        }

        private void TriggerStay(Physics2D.Contact i_Contact)
        {
            CallSyncedTriggerStay(i_Contact.FixtureA.Body, i_Contact.FixtureB.Body, i_Contact);
        }

        private void TriggerExit(Physics2D.Contact i_Contact)
        {
            CallSyncedTriggerExit(i_Contact.FixtureA.Body, i_Contact.FixtureB.Body, null);
        }

        // INTERNALS

        private void InternalStep()
        {
            m_World.Step(LockedTimeStep);
        }

        private void InternalUpdateTransformCache()
        {
            float elapsedTime = Time.fixedTime;

            for (int rbIndex = 0; rbIndex < m_RigidBodies.Count; ++rbIndex)
            {
                TSRigidBody2D rb = m_RigidBodies[rbIndex];
                rb.UpdateTransformCache(elapsedTime);
            }
        }

        private void CallSyncedCollisionEnter(Physics2D.Body i_Body1, Physics2D.Body i_Body2, Physics2D.Contact i_Contact)
        {
            GameObject b1;
            m_GameObjectMap.TryGetValue(i_Body1, out b1);

            GameObject b2;
            m_GameObjectMap.TryGetValue(i_Body2, out b2);

            if (b1 == null || b2 == null)
                return;

            // Notify b1.

            {
                TrueSyncBehaviour[] trueSyncBehaviours = b1.GetComponentsInChildren<TrueSyncBehaviour>();
                if (trueSyncBehaviours != null)
                {
                    for (int index = 0; index < trueSyncBehaviours.Length; ++index)
                    {
                        TrueSyncBehaviour behaviour = trueSyncBehaviours[index];
                        behaviour.OnSyncedCollisionEnter(FillCollisionInfo(b2, i_Contact));
                    }
                }
            }

            // Notify b2.

            {
                TrueSyncBehaviour[] trueSyncBehaviours = b2.GetComponentsInChildren<TrueSyncBehaviour>();
                if (trueSyncBehaviours != null)
                {
                    for (int index = 0; index < trueSyncBehaviours.Length; ++index)
                    {
                        TrueSyncBehaviour behaviour = trueSyncBehaviours[index];
                        behaviour.OnSyncedCollisionEnter(FillCollisionInfo(b1, i_Contact));
                    }
                }
            }

            // Clear cache.

            ClearCollisionCache();
        }

        private void CallSyncedCollisionStay(Physics2D.Body i_Body1, Physics2D.Body i_Body2, Physics2D.Contact i_Contact)
        {
            GameObject b1;
            m_GameObjectMap.TryGetValue(i_Body1, out b1);

            GameObject b2;
            m_GameObjectMap.TryGetValue(i_Body2, out b2);

            if (b1 == null || b2 == null)
                return;

            // Notify b1.

            {
                TrueSyncBehaviour[] trueSyncBehaviours = b1.GetComponentsInChildren<TrueSyncBehaviour>();
                if (trueSyncBehaviours != null)
                {
                    for (int index = 0; index < trueSyncBehaviours.Length; ++index)
                    {
                        TrueSyncBehaviour behaviour = trueSyncBehaviours[index];
                        behaviour.OnSyncedCollisionStay(FillCollisionInfo(b2, i_Contact));
                    }
                }
            }

            // Notify b2.

            {
                TrueSyncBehaviour[] trueSyncBehaviours = b2.GetComponentsInChildren<TrueSyncBehaviour>();
                if (trueSyncBehaviours != null)
                {
                    for (int index = 0; index < trueSyncBehaviours.Length; ++index)
                    {
                        TrueSyncBehaviour behaviour = trueSyncBehaviours[index];
                        behaviour.OnSyncedCollisionStay(FillCollisionInfo(b1, i_Contact));
                    }
                }
            }

            // Clear cache.

            ClearCollisionCache();
        }

        private void CallSyncedCollisionExit(Physics2D.Body i_Body1, Physics2D.Body i_Body2, Physics2D.Contact i_Contact)
        {
            GameObject b1;
            m_GameObjectMap.TryGetValue(i_Body1, out b1);

            GameObject b2;
            m_GameObjectMap.TryGetValue(i_Body2, out b2);

            if (b1 == null || b2 == null)
                return;

            // Notify b1.

            {
                TrueSyncBehaviour[] trueSyncBehaviours = b1.GetComponentsInChildren<TrueSyncBehaviour>();
                if (trueSyncBehaviours != null)
                {
                    for (int index = 0; index < trueSyncBehaviours.Length; ++index)
                    {
                        TrueSyncBehaviour behaviour = trueSyncBehaviours[index];
                        behaviour.OnSyncedCollisionExit(FillCollisionInfo(b2, i_Contact));
                    }
                }
            }

            // Notify b2.

            {
                TrueSyncBehaviour[] trueSyncBehaviours = b2.GetComponentsInChildren<TrueSyncBehaviour>();
                if (trueSyncBehaviours != null)
                {
                    for (int index = 0; index < trueSyncBehaviours.Length; ++index)
                    {
                        TrueSyncBehaviour behaviour = trueSyncBehaviours[index];
                        behaviour.OnSyncedCollisionExit(FillCollisionInfo(b1, i_Contact));
                    }
                }
            }

            // Clear cache.

            ClearCollisionCache();
        }

        private void CallSyncedTriggerEnter(Physics2D.Body i_Body1, Physics2D.Body i_Body2, Physics2D.Contact i_Contact)
        {
            GameObject b1;
            m_GameObjectMap.TryGetValue(i_Body1, out b1);

            GameObject b2;
            m_GameObjectMap.TryGetValue(i_Body2, out b2);

            if (b1 == null || b2 == null)
                return;

            // Notify b1.

            {
                TrueSyncBehaviour[] trueSyncBehaviours = b1.GetComponentsInChildren<TrueSyncBehaviour>();
                if (trueSyncBehaviours != null)
                {
                    for (int index = 0; index < trueSyncBehaviours.Length; ++index)
                    {
                        TrueSyncBehaviour behaviour = trueSyncBehaviours[index];
                        behaviour.OnSyncedTriggerEnter(FillCollisionInfo(b2, i_Contact));
                    }
                }
            }

            // Notify b2.

            {
                TrueSyncBehaviour[] trueSyncBehaviours = b2.GetComponentsInChildren<TrueSyncBehaviour>();
                if (trueSyncBehaviours != null)
                {
                    for (int index = 0; index < trueSyncBehaviours.Length; ++index)
                    {
                        TrueSyncBehaviour behaviour = trueSyncBehaviours[index];
                        behaviour.OnSyncedTriggerEnter(FillCollisionInfo(b1, i_Contact));
                    }
                }
            }

            // Clear cache.

            ClearCollisionCache();
        }

        private void CallSyncedTriggerStay(Physics2D.Body i_Body1, Physics2D.Body i_Body2, Physics2D.Contact i_Contact)
        {
            GameObject b1;
            m_GameObjectMap.TryGetValue(i_Body1, out b1);

            GameObject b2;
            m_GameObjectMap.TryGetValue(i_Body2, out b2);

            if (b1 == null || b2 == null)
                return;

            // Notify b1.

            {
                TrueSyncBehaviour[] trueSyncBehaviours = b1.GetComponentsInChildren<TrueSyncBehaviour>();
                if (trueSyncBehaviours != null)
                {
                    for (int index = 0; index < trueSyncBehaviours.Length; ++index)
                    {
                        TrueSyncBehaviour behaviour = trueSyncBehaviours[index];
                        behaviour.OnSyncedTriggerStay(FillCollisionInfo(b2, i_Contact));
                    }
                }
            }

            // Notify b2.

            {
                TrueSyncBehaviour[] trueSyncBehaviours = b2.GetComponentsInChildren<TrueSyncBehaviour>();
                if (trueSyncBehaviours != null)
                {
                    for (int index = 0; index < trueSyncBehaviours.Length; ++index)
                    {
                        TrueSyncBehaviour behaviour = trueSyncBehaviours[index];
                        behaviour.OnSyncedTriggerStay(FillCollisionInfo(b1, i_Contact));
                    }
                }
            }

            // Clear cache.

            ClearCollisionCache();
        }

        private void CallSyncedTriggerExit(Physics2D.Body i_Body1, Physics2D.Body i_Body2, Physics2D.Contact i_Contact)
        {
            GameObject b1;
            m_GameObjectMap.TryGetValue(i_Body1, out b1);

            GameObject b2;
            m_GameObjectMap.TryGetValue(i_Body2, out b2);

            if (b1 == null || b2 == null)
                return;

            // Notify b1.

            {
                TrueSyncBehaviour[] trueSyncBehaviours = b1.GetComponentsInChildren<TrueSyncBehaviour>();
                if (trueSyncBehaviours != null)
                {
                    for (int index = 0; index < trueSyncBehaviours.Length; ++index)
                    {
                        TrueSyncBehaviour behaviour = trueSyncBehaviours[index];
                        behaviour.OnSyncedTriggerExit(FillCollisionInfo(b2, i_Contact));
                    }
                }
            }

            // Notify b2.

            {
                TrueSyncBehaviour[] trueSyncBehaviours = b2.GetComponentsInChildren<TrueSyncBehaviour>();
                if (trueSyncBehaviours != null)
                {
                    for (int index = 0; index < trueSyncBehaviours.Length; ++index)
                    {
                        TrueSyncBehaviour behaviour = trueSyncBehaviours[index];
                        behaviour.OnSyncedTriggerExit(FillCollisionInfo(b1, i_Contact));
                    }
                }
            }

            // Clear cache.

            ClearCollisionCache();
        }

        private TSCollision2D FillCollisionInfo(GameObject i_Go, Physics2D.Contact i_Contact)
        {
            m_CollisionCache.Update(i_Go, i_Contact);
            return m_CollisionCache;
        }

        // UTILS

        private string GetChecksum(bool i_Plain)
        {
            return ChecksumExtractor.GetEncodedChecksum();
        }

        private void ClearCollisionCache()
        {
            if (m_CollisionCache != null)
            {
                m_CollisionCache.Clear();
            }
        }

        // RAYCAST

        public TSRaycastHit2D[] Raycast(TSVector2 i_Origin, TSVector2 i_Direction, FP i_Distance, int i_Mask)
        {
            List<TSRaycastHit2D> result = new List<TSRaycastHit2D>();

            Func<Physics2D.Fixture, TSVector2, TSVector2, FP, FP> callback = delegate (Physics2D.Fixture i_Fixture, TSVector2 i_Point, TSVector2 i_Normal, FP i_Fraction)
            {
                GameObject go = GetGameObject(i_Fixture.Body);

                int layerMask = (1 << go.layer);
                if ((i_Mask & layerMask) != 0)
                {
                    TSCollider2D collider2D = go.GetComponent<TSCollider2D>();

                    TSVector2 distanceVector = i_Point - i_Origin;
                    FP distance = distanceVector.magnitude;

                    result.Add(new TSRaycastHit2D(collider2D, i_Point, i_Normal, distance, i_Fraction));
                }

                return -1;
            };

            m_World.RayCast(callback, i_Origin, i_Origin + i_Direction * i_Distance);

            if (result.Count == 0)
            {
                return null;
            }

            return result.ToArray();
        }

        // CTOR

        public Physics2DWorldManager()
        {
            m_GameObjectMap = new Dictionary<IBody, GameObject>();
            m_RigidBodies = new List<TSRigidBody2D>();

            m_CollisionCache = new TSCollision2D();
        }
    }
}
