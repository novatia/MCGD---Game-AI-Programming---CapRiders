public class GameSettings : Singleton<GameSettings>
{
    // STATIC

    public static void InitializeMain()
    {
        if (Instance != null)
        {
            Instance.Initialize();
        }
    }

    public static void SaveMain()
    {
        if (Instance != null)
        {
            Instance.Save();
        }
    }

    public static void LoadMain()
    {
        if (Instance != null)
        {
            Instance.Load();
        }
    }

    public static void DeleteMain(string i_Id)
    {
        if (Instance != null)
        {
            Instance.Delete(i_Id);
        }
    }

    public static void DeleteMain(int i_Id)
    {
        if (Instance != null)
        {
            Instance.Delete(i_Id);
        }
    }

    public static void DeleteAllMain()
    {
        if (Instance != null)
        {
            Instance.DeleteAll();
        }
    }

    public static void SetIntMain(string i_Id, int i_Value)
    {
        if (Instance != null)
        {
            Instance.SetInt(i_Id, i_Value);
        }
    }

    public static void SetIntMain(int i_Id, int i_Value)
    {
        if (Instance != null)
        {
            Instance.SetInt(i_Id, i_Value);
        }
    }

    public static void SetFloatMain(string i_Id, float i_Value)
    {
        if (Instance != null)
        {
            Instance.SetFloat(i_Id, i_Value);
        }
    }

    public static void SetFloatMain(int i_Id, float i_Value)
    {
        if (Instance != null)
        {
            Instance.SetFloat(i_Id, i_Value);
        }
    }

    public static void SetStringMain(string i_Id, string i_Value)
    {
        if (Instance != null)
        {
            Instance.SetString(i_Id, i_Value);
        }
    }

    public static void SetStringMain(int i_Id, string i_Value)
    {
        if (Instance != null)
        {
            Instance.SetString(i_Id, i_Value);
        }
    }

    public static void SetBoolMain(string i_Id, bool i_Value)
    {
        if (Instance != null)
        {
            Instance.SetBool(i_Id, i_Value);
        }
    }

    public static void SetBoolMain(int i_Id, bool i_Value)
    {
        if (Instance != null)
        {
            Instance.SetBool(i_Id, i_Value);
        }
    }

    public static int GetIntMain(string i_Id)
    {
        if (Instance != null)
        {
            return Instance.GetInt(i_Id);
        }

        return 0;
    }

    public static int GetIntMain(int i_Id)
    {
        if (Instance != null)
        {
            return Instance.GetInt(i_Id);
        }

        return 0;
    }

    public static float GetFloatMain(string i_Id)
    {
        if (Instance != null)
        {
            return Instance.GetFloat(i_Id);
        }

        return 0f;
    }

    public static float GetFloatMain(int i_Id)
    {
        if (Instance != null)
        {
            return Instance.GetFloat(i_Id);
        }

        return 0f;
    }

    public static string GetStringMain(string i_Id)
    {
        if (Instance != null)
        {
            return Instance.GetString(i_Id);
        }

        return StringUtils.s_EMPTY;
    }

    public static string GetStringMain(int i_Id)
    {
        if (Instance != null)
        {
            return Instance.GetString(i_Id);
        }

        return StringUtils.s_EMPTY;
    }

    public static bool GetBoolMain(string i_Id)
    {
        if (Instance != null)
        {
            return Instance.GetBool(i_Id);
        }

        return false;
    }

    public static bool GetBoolMain(int i_Id)
    {
        if (Instance != null)
        {
            return Instance.GetBool(i_Id);
        }

        return false;
    }

    public static bool TryGetIntMain(string i_Id, out int o_Value)
    {
        o_Value = 0;

        if (Instance != null)
        {
            return Instance.TryGetInt(i_Id, out o_Value);
        }

        return false;
    }

    public static bool TryGetIntMain(int i_Id, out int o_Value)
    {
        o_Value = 0;

        if (Instance != null)
        {
            return Instance.TryGetInt(i_Id, out o_Value);
        }

        return false;
    }

    public static bool TryGetFloatMain(string i_Id, out float o_Value)
    {
        o_Value = 0f;

        if (Instance != null)
        {
            return Instance.TryGetFloat(i_Id, out o_Value);
        }

        return false;
    }

    public static bool TryGetFloatMain(int i_Id, out float o_Value)
    {
        o_Value = 0f;

        if (Instance != null)
        {
            return Instance.TryGetFloat(i_Id, out o_Value);
        }

        return false;
    }

    public static bool TryGetStringMain(string i_Id, out string o_Value)
    {
        o_Value = StringUtils.s_EMPTY;

        if (Instance != null)
        {
            return Instance.TryGetString(i_Id, out o_Value);
        }

        return false;
    }

    public static bool TryGetStringMain(int i_Id, out string o_Value)
    {
        o_Value = StringUtils.s_EMPTY;

        if (Instance != null)
        {
            return Instance.TryGetString(i_Id, out o_Value);
        }

        return false;
    }

    public static bool TryGetBoolMain(string i_Id, out bool o_Value)
    {
        o_Value = false;

        if (Instance != null)
        {
            return Instance.TryGetBool(i_Id, out o_Value);
        }

        return false;
    }

    public static bool TryGetBoolMain(int i_Id, out bool o_Value)
    {
        o_Value = false;

        if (Instance != null)
        {
            return Instance.TryGetBool(i_Id, out o_Value);
        }

        return false;
    }

    public static bool HasKeyMain(string i_Id)
    {
        if (Instance != null)
        {
            return Instance.HasKey(i_Id);
        }

        return false;
    }

    public static bool HasKeyMain(int i_Id)
    {
        if (Instance != null)
        {
            return Instance.HasKey(i_Id);
        }

        return false;
    }

    public static bool HasIntKeyMain(string i_Id)
    {
        if (Instance != null)
        {
            return Instance.HasIntKey(i_Id);
        }

        return false;
    }

    public static bool HasIntKeyMain(int i_Id)
    {
        if (Instance != null)
        {
            return Instance.HasIntKey(i_Id);
        }

        return false;
    }

    public static bool HasFloatKeyMain(string i_Id)
    {
        if (Instance != null)
        {
            return Instance.HasFloatKey(i_Id);
        }

        return false;
    }

    public static bool HasFloatKeyMain(int i_Id)
    {
        if (Instance != null)
        {
            return Instance.HasFloatKey(i_Id);
        }

        return false;
    }

    public static bool HasStringKeyMain(string i_Id)
    {
        if (Instance != null)
        {
            return Instance.HasStringKey(i_Id);
        }

        return false;
    }

    public static bool HasStringKeyMain(int i_Id)
    {
        if (Instance != null)
        {
            return Instance.HasStringKey(i_Id);
        }

        return false;
    }

    public static bool HasBoolKeyMain(string i_Id)
    {
        if (Instance != null)
        {
            return Instance.HasBoolKey(i_Id);
        }

        return false;
    }

    public static bool HasBoolKeyMain(int i_Id)
    {
        if (Instance != null)
        {
            return Instance.HasBoolKey(i_Id);
        }

        return false;
    }

    // Fields

    private IGameSettingsImpl m_Impl = null;

    // LOGIC

    public void Initialize()
    {
        m_Impl = new GoGameSettingsImpl();
        m_Impl.Initialize();
    }

    public void Save()
    {
        if (m_Impl != null)
        {
            m_Impl.Save();
        }
    }

    public void Load()
    {
        if (m_Impl != null)
        {
            m_Impl.Load();
        }
    }

    public void Delete(string i_Id)
    {
        if (m_Impl != null)
        {
            m_Impl.Delete(i_Id);
        }
    }

    public void Delete(int i_Id)
    {
        if (m_Impl != null)
        {
            m_Impl.Delete(i_Id);
        }
    }

    public void DeleteAll()
    {
        if (m_Impl != null)
        {
            m_Impl.DeleteAll();
        }
    }

    public void SetInt(string i_Id, int i_Value)
    {
        if (m_Impl != null)
        {
            m_Impl.SetInt(i_Id, i_Value);
        }
    }

    public void SetInt(int i_Id, int i_Value)
    {
        if (m_Impl != null)
        {
            m_Impl.SetInt(i_Id, i_Value);
        }
    }

    public void SetFloat(string i_Id, float i_Value)
    {
        if (m_Impl != null)
        {
            m_Impl.SetFloat(i_Id, i_Value);
        }
    }

    public void SetFloat(int i_Id, float i_Value)
    {
        if (m_Impl != null)
        {
            m_Impl.SetFloat(i_Id, i_Value);
        }
    }

    public void SetString(string i_Id, string i_Value)
    {
        if (m_Impl != null)
        {
            m_Impl.SetString(i_Id, i_Value);
        }
    }

    public void SetString(int i_Id, string i_Value)
    {
        if (m_Impl != null)
        {
            m_Impl.SetString(i_Id, i_Value);
        }
    }

    public void SetBool(string i_Id, bool i_Value)
    {
        if (m_Impl != null)
        {
            m_Impl.SetBool(i_Id, i_Value);
        }
    }

    public void SetBool(int i_Id, bool i_Value)
    {
        if (m_Impl != null)
        {
            m_Impl.SetBool(i_Id, i_Value);
        }
    }

    public int GetInt(string i_Id)
    {
        if (m_Impl != null)
        {
            return m_Impl.GetInt(i_Id);
        }

        return 0;
    }

    public int GetInt(int i_Id)
    {
        if (m_Impl != null)
        {
            return m_Impl.GetInt(i_Id);
        }

        return 0;
    }

    public float GetFloat(string i_Id)
    {
        if (m_Impl != null)
        {
            return m_Impl.GetFloat(i_Id);
        }

        return 0f;
    }

    public float GetFloat(int i_Id)
    {
        if (m_Impl != null)
        {
            return m_Impl.GetFloat(i_Id);
        }

        return 0f;
    }

    public string GetString(string i_Id)
    {
        if (m_Impl != null)
        {
            return m_Impl.GetString(i_Id);
        }

        return StringUtils.s_EMPTY;
    }

    public string GetString(int i_Id)
    {
        if (m_Impl != null)
        {
            return m_Impl.GetString(i_Id);
        }

        return StringUtils.s_EMPTY;
    }

    public bool GetBool(string i_Id)
    {
        if (m_Impl != null)
        {
            return m_Impl.GetBool(i_Id);
        }

        return false;
    }

    public bool GetBool(int i_Id)
    {
        if (m_Impl != null)
        {
            return m_Impl.GetBool(i_Id);
        }

        return false;
    }

    public bool TryGetInt(string i_Id, out int o_Value)
    {
        o_Value = 0;

        if (m_Impl != null)
        {
            return m_Impl.TryGetInt(i_Id, out o_Value);
        }

        return false;
    }

    public bool TryGetInt(int i_Id, out int o_Value)
    {
        o_Value = 0;

        if (m_Impl != null)
        {
            return m_Impl.TryGetInt(i_Id, out o_Value);
        }

        return false;
    }

    public bool TryGetFloat(string i_Id, out float o_Value)
    {
        o_Value = 0f;

        if (m_Impl != null)
        {
            return m_Impl.TryGetFloat(i_Id, out o_Value);
        }

        return false;
    }

    public bool TryGetFloat(int i_Id, out float o_Value)
    {
        o_Value = 0f;

        if (m_Impl != null)
        {
            return m_Impl.TryGetFloat(i_Id, out o_Value);
        }

        return false;
    }

    public bool TryGetString(string i_Id, out string o_Value)
    {
        o_Value = StringUtils.s_EMPTY;

        if (m_Impl != null)
        {
            return m_Impl.TryGetString(i_Id, out o_Value);
        }

        return false;
    }

    public bool TryGetString(int i_Id, out string o_Value)
    {
        o_Value = StringUtils.s_EMPTY;

        if (m_Impl != null)
        {
            return m_Impl.TryGetString(i_Id, out o_Value);
        }

        return false;
    }

    public bool TryGetBool(string i_Id, out bool o_Value)
    {
        o_Value = false;

        if (m_Impl != null)
        {
            return m_Impl.TryGetBool(i_Id, out o_Value);
        }

        return false;
    }

    public bool TryGetBool(int i_Id, out bool o_Value)
    {
        o_Value = false;

        if (m_Impl != null)
        {
            return m_Impl.TryGetBool(i_Id, out o_Value);
        }

        return false;
    }

    public bool HasKey(string i_Id)
    {
        if (m_Impl != null)
        {
            return m_Impl.HasKey(i_Id);
        }

        return false;
    }

    public bool HasKey(int i_Id)
    {
        if (m_Impl != null)
        {
            return m_Impl.HasKey(i_Id);
        }

        return false;
    }

    public bool HasIntKey(string i_Id)
    {
        if (m_Impl != null)
        {
            return m_Impl.HasIntKey(i_Id);
        }

        return false;
    }

    public bool HasIntKey(int i_Id)
    {
        if (m_Impl != null)
        {
            return m_Impl.HasIntKey(i_Id);
        }

        return false;
    }

    public bool HasFloatKey(string i_Id)
    {
        if (m_Impl != null)
        {
            return m_Impl.HasFloatKey(i_Id);
        }

        return false;
    }

    public bool HasFloatKey(int i_Id)
    {
        if (m_Impl != null)
        {
            return m_Impl.HasFloatKey(i_Id);
        }

        return false;
    }

    public bool HasStringKey(string i_Id)
    {
        if (m_Impl != null)
        {
            return m_Impl.HasStringKey(i_Id);
        }

        return false;
    }

    public bool HasStringKey(int i_Id)
    {
        if (m_Impl != null)
        {
            return m_Impl.HasStringKey(i_Id);
        }

        return false;
    }

    public bool HasBoolKey(string i_Id)
    {
        if (m_Impl != null)
        {
            return m_Impl.HasBoolKey(i_Id);
        }

        return false;
    }

    public bool HasBoolKey(int i_Id)
    {
        if (m_Impl != null)
        {
            return m_Impl.HasBoolKey(i_Id);
        }

        return false;
    }
}
