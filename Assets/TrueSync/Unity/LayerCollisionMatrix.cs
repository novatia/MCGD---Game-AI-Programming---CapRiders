using UnityEngine;

namespace TrueSync
{
    public class LayerCollisionMatrix
    {
        /**
         * @brief Returns true if the given layers can collide.
         * 
         * @param layerA Layer of the first object
         * @param layerB Layer of the second object
         **/
        public static bool CollisionEnabled(int i_LayerA, int i_LayerB)
        {
            TrueSyncConfig tsConfig = TrueSyncManager.configMain;
            if (tsConfig == null)
            {
                return true;
            }

            return tsConfig.CollisionEnabled(i_LayerA, i_LayerB);            
        }

        public static int ComputeCollisionMask(int i_Layer)
        {
            TrueSyncConfig tsConfig = TrueSyncManager.configMain;
            if (tsConfig == null)
            {
                return 0;
            }

            return tsConfig.ComputeCollisionMask(i_Layer);
        }
    }
}