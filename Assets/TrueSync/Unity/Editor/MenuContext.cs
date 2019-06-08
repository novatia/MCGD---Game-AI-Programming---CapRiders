using UnityEditor;

namespace TrueSync
{
    public class MenuContext
    {
        // INTERNALS

        private static void CreateTrueSyncConfigAsset()
        {
            ScriptableObjectUtility.CreateAsset<TrueSyncConfig>("TrueSyncConfig");
        }

        private static void CreateTrueSyncPhysicsMaterial()
        {
            ScriptableObjectUtility.CreateAsset<TSPhysicsMaterial2D>("TSPhysicsMaterial2D");
        }

        // MENU ENTRIES

        [MenuItem("Assets/Create/TrueSync Config", false, 0)]
        static void CreateTrueSyncConfig()
        {
            CreateTrueSyncConfigAsset();
        }

        [MenuItem("Assets/Create/TrueSync PhysicsMaterial2D", false, 0)]
        static void CreateTrueSyncPhysicsMaterial2D()
        {
            CreateTrueSyncPhysicsMaterial();
        }
    }
}