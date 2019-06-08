public interface IGameSettingsImpl
{
    void Initialize();

    void Save();
    void Load();

    void Delete(string i_Id);
    void Delete(int i_Id);
    void DeleteInt(string i_Id);
    void DeleteInt(int i_Id);
    void DeleteFloat(string i_Id);
    void DeleteFloat(int i_Id);
    void DeleteString(string i_Id);
    void DeleteString(int i_Id);
    void DeleteBool(string i_Id);
    void DeleteBool(int i_Id);
    void DeleteAll();

    void SetInt(string i_Id, int i_Value);
    void SetInt(int i_Id, int i_Value);
    void SetFloat(string i_Id, float i_Value);
    void SetFloat(int i_Id, float i_Value);
    void SetString(string i_Id, string i_Value);
    void SetString(int i_Id, string i_Value);
    void SetBool(string i_Id, bool i_Value);
    void SetBool(int i_Id, bool i_Value);

    int GetInt(string i_Id);
    int GetInt(int i_Id);
    float GetFloat(string i_Id);
    float GetFloat(int i_Id);
    string GetString(int i_Id);
    string GetString(string i_Id);
    bool GetBool(string i_Id);
    bool GetBool(int i_Id);

    bool TryGetInt(string i_Id, out int o_Value);
    bool TryGetInt(int i_Id, out int o_Value);
    bool TryGetFloat(string i_Id, out float o_Value);
    bool TryGetFloat(int i_Id, out float o_Value);
    bool TryGetString(string i_Id, out string o_Value);
    bool TryGetString(int i_Id, out string o_Value);
    bool TryGetBool(string i_Id, out bool o_Value);
    bool TryGetBool(int i_Id, out bool o_Value);

    bool HasKey(string i_Id);
    bool HasKey(int i_Id);
    bool HasIntKey(string i_Id);
    bool HasIntKey(int i_Id);
    bool HasFloatKey(string i_Id);
    bool HasFloatKey(int i_Id);
    bool HasStringKey(string i_Id);
    bool HasStringKey(int i_Id);
    bool HasBoolKey(string i_Id);
    bool HasBoolKey(int i_Id);
}
