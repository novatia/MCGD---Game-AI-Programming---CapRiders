using System.Collections.Generic;

public class GameModulesManager : Singleton<GameModulesManager>
{
    private List<GameModule> modules = new List<GameModule>();

    // STATIC METHODS

    public static void InitializeMain()
    {
        if (Instance != null)
        {
            Instance.Initialize();
        }
    }

    public static T AddModuleMain<T>() where T : GameModule, new()
    {
        if (Instance != null)
        {
            return Instance.AddModule<T>();
        }

        return null;
    }

    public static T GetModuleMain<T>() where T : GameModule
    {
        if (Instance != null)
        {
            return Instance.GetModule<T>();
        }

        return null;
    }

    // MonoBehaviour's INTERFACE

    void Awake()
    {

    }

    // BUSINESS LOGIC

    public void Initialize()
    {

    }

    public T AddModule<T>() where T : GameModule, new()
    {
        T module = GetModule<T>();

        if (module == null)
        {
            module = new T();
            modules.Add(module);
        }

        return module;
    }

    public T GetModule<T>() where T : GameModule
    {
        foreach (GameModule module in modules)
        {
            if (typeof(T) == module.GetType())
            {
                return (T)module;
            }
        }

        return null;
    }
}