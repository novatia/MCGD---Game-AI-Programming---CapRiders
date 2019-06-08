using System.Collections.Generic;
using System;
using System.Reflection;

namespace TrueSync
{
    /**
     *  @brief Provides a few utilities to be used on TrueSync exposed classes.
     **/
    public class UnityUtils
    {
        /**
         *  @brief Comparer class to guarantee PhotonPlayer order.
         **/
        public class PlayerComparer : Comparer<PhotonPlayer>
        {
            public override int Compare(PhotonPlayer x, PhotonPlayer y)
            {
                return x.ID - y.ID;
            }
        }

        public class TrueSyncBehaviourComparer : Comparer<TrueSyncManagedBehaviour>
        {
            public override int Compare(TrueSyncManagedBehaviour x, TrueSyncManagedBehaviour y)
            {
                TrueSyncBehaviour x1 = x.trueSyncBehavior as TrueSyncBehaviour;
                TrueSyncBehaviour y1 = y.trueSyncBehavior as TrueSyncBehaviour;

                return x1.sortOrder - y1.sortOrder;
            }
        }

        /**
         *  @brief Instance of a {@link PlayerComparer}.
         **/
        public static PlayerComparer playerComparer = new PlayerComparer();

        /**
         *  @brief Instance of a {@link TrueSyncBehaviourComparer}.
         **/
        public static TrueSyncBehaviourComparer trueSyncBehaviourComparer = new TrueSyncBehaviourComparer();

        /**
         *  @brief Comparer class to guarantee {@link TSCollider2D} order.
         **/
        public class TSBody2DComparer : Comparer<TSCollider2D>
        {
            public override int Compare(TSCollider2D x, TSCollider2D y)
            {
                return x.gameObject.name.CompareTo(y.gameObject.name);
            }
        }

        /**
         *  @brief Instance of a {@link TSBody2DComparer}.
         **/
        public static TSBody2DComparer body2DComparer = new TSBody2DComparer();
    }
}