using UnityEngine;

namespace TrueSync {

    /**
    * @brief Represents a set of configurations for TrueSync.
    **/
    public class TrueSyncConfig : ScriptableObject
    {
        // 32 layers -> 516 unique intersections

        private const int COLLISION_LAYERS = 32;
        private const int COLLISION_TOGGLES = 516;

        /**
         * @brief Synchronization window size.
         **/
        public int syncWindow = 10;

        /**
         * @brief Rollback window size.
         **/
        public int rollbackWindow = 0;

        /**
         * @brief Maximum number of ticks to wait until all other players inputs arrive.
         **/
        public int panicWindow = 100;

        /**
         * @brief Holds which layers should be ingnored in 2D collisions
         **/
        public bool[] collisionMatrix = new bool[COLLISION_TOGGLES];

        /**
         *  @brief Represents the simulated gravity.
         **/
        public TSVector2 gravity = new TSVector2(0, 0);

        /**
         *  @brief If true enables a deeper collision detection system.
         **/
        public bool speculativeContacts = false;

        /**
         * @brief When true shows a debug interface with a few information.
         **/
        public bool showStats = false;

        /**
         * @brief Time between each TrueSync's frame.
         **/
        public FP lockedTimeStep = 0.02f;

        public TrueSyncConfig()
        {

        }

        /**
         * @brief Returns true if the collision between layerA and layerB should be ignored.
         **/
        public bool CollisionEnabled(int i_LayerA, int i_LayerB)
        {
            if (i_LayerB < i_LayerA)
            {
                int aux = i_LayerA;
                i_LayerA = i_LayerB;
                i_LayerB = aux;
            }

            int matrixIndex = ((COLLISION_LAYERS + COLLISION_LAYERS - i_LayerA + 1) * i_LayerA) / 2 + i_LayerB;
            return collisionMatrix[matrixIndex];
        }

        public int ComputeCollisionMask(int i_Layer)
        {
            int layerMask = 0;

            for (int layerIndex = 0; layerIndex < COLLISION_LAYERS; ++layerIndex)
            {
                if (CollisionEnabled(i_Layer, layerIndex))
                {
                    layerMask |= (1 << layerIndex);
                }
            }

            return layerMask;
        }
    }
}