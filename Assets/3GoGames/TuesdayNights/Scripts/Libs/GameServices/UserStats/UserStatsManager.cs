using UnityEngine;

using System;
using System.Collections.Generic;

public enum IntStatUpdateFunction
{
    Set,
    Min,
    Max,
    Add,
    Multiply,
}

public enum BoolStatUpdateFunction
{
    Set,
    Add,
    Multiply,
}

public enum FloatStatUpdateFunction
{
    Set,
    Min,
    Max,
    Add,
    Multiply,
}

public enum StringStatUpdateFunction
{
    Set,
    Min,
    Max,
    Add,
}

public class UserStatsManager
{
    private string m_DatabaseResourcePath = "";
    private UserStatsDatabase m_Database = null;

    private List<UserStat> m_UserStats = null;

    // Getters

    public int statsCount
    {
        get { return m_UserStats.Count; }
    }

    // LOGIC

    public void Initialize()
    {
        m_Database = Resources.Load<UserStatsDatabase>(m_DatabaseResourcePath);

        if (m_Database != null)
        {
            for (int userStatIndex = 0; userStatIndex < m_Database.statsCount; ++userStatIndex)
            {
                UserStatDescriptor descriptor = m_Database.GetUserStat(userStatIndex);
                if (descriptor != null)
                {
                    UserStat userStat = CreateUserStat(descriptor);
                    if (userStat != null)
                    {
                        m_UserStats.Add(userStat);
                    }
                }
            }
        }
        else
        {
            LogManager.LogWarning(this, "Database could not be loaded.");
        }
    }

    public UserStat GetStatByIndex(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_UserStats.Count)
        {
            return null;
        }

        return m_UserStats[i_Index];
    }

    public UserStatType GetUserStatStype(string i_StatId)
    {
        for (int userStatIndex = 0; userStatIndex < m_UserStats.Count; ++userStatIndex)
        {
            UserStat userStat = m_UserStats[userStatIndex];
            if (userStat != null)
            {
                string statId = userStat.id;
                if (statId == i_StatId)
                {
                    return userStat.type;
                }
            }
        }

        return UserStatType.Invalid;
    }

    public void UpdateIntStat(string i_StatId, int i_Value, IntStatUpdateFunction i_UpdateFunction)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        UpdateIntStat(hash, i_Value, i_UpdateFunction);
    }

    public void UpdateIntStat(int i_StatId, int i_Value, IntStatUpdateFunction i_UpdateFunction)
    {
        for (int userStatIndex = 0; userStatIndex < m_UserStats.Count; ++userStatIndex)
        {
            UserStat userStat = m_UserStats[userStatIndex];
            if (userStat != null)
            {
                int statId = userStat.hashId;
                if (statId == i_StatId)
                {
                    if (userStat.type == UserStatType.Int)
                    {
                        InternalUpdateIntStat((UserStatInt)userStat, i_Value, i_UpdateFunction);
                        break;
                    }
                }
            }
        }
    }

    public void UpdateBoolStat(string i_StatId, bool i_Value, BoolStatUpdateFunction i_UpdateFunction)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        UpdateBoolStat(hash, i_Value, i_UpdateFunction);
    }

    public void UpdateBoolStat(int i_StatId, bool i_Value, BoolStatUpdateFunction i_UpdateFunction)
    {
        for (int userStatIndex = 0; userStatIndex < m_UserStats.Count; ++userStatIndex)
        {
            UserStat userStat = m_UserStats[userStatIndex];
            if (userStat != null)
            {
                int statId = userStat.hashId;
                if (statId == i_StatId)
                {
                    if (userStat.type == UserStatType.Bool)
                    {
                        InternalUpdateBoolStat((UserStatBool)userStat, i_Value, i_UpdateFunction);
                        break;
                    }
                }
            }
        }
    }

    public void UpdateFloatStat(string i_StatId, float i_Value, FloatStatUpdateFunction i_UpdateFunction)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        UpdateFloatStat(hash, i_Value, i_UpdateFunction);
    }

    public void UpdateFloatStat(int i_StatId, float i_Value, FloatStatUpdateFunction i_UpdateFunction)
    {
        for (int userStatIndex = 0; userStatIndex < m_UserStats.Count; ++userStatIndex)
        {
            UserStat userStat = m_UserStats[userStatIndex];
            if (userStat != null)
            {
                int statId = userStat.hashId;
                if (statId == i_StatId)
                {
                    if (userStat.type == UserStatType.Float)
                    {
                        InternalUpdateFloatStat((UserStatFloat)userStat, i_Value, i_UpdateFunction);
                        break;
                    }
                }
            }
        }
    }

    public void UpdateStringStat(string i_StatId, string i_Value, StringStatUpdateFunction i_UpdateFunction)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        UpdateStringStat(hash, i_Value, i_UpdateFunction);
    }

    public void UpdateStringStat(int i_StatId, string i_Value, StringStatUpdateFunction i_UpdateFunction)
    {
        for (int userStatIndex = 0; userStatIndex < m_UserStats.Count; ++userStatIndex)
        {
            UserStat userStat = m_UserStats[userStatIndex];
            if (userStat != null)
            {
                int statId = userStat.hashId;
                if (statId == i_StatId)
                {
                    if (userStat.type == UserStatType.String)
                    {
                        InternalUpdateStringStat((UserStatString)userStat, i_Value, i_UpdateFunction);
                        break;
                    }
                }
            }
        }
    }

    public void RegisterIntStatHandler(string i_StatId, UserStatValueChangedEvent<int> i_Handler)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        RegisterIntStatHandler(hash, i_Handler);
    }

    public void RegisterIntStatHandler(int i_StatId, UserStatValueChangedEvent<int> i_Handler)
    {
        for (int userStatIndex = 0; userStatIndex < m_UserStats.Count; ++userStatIndex)
        {
            UserStat userStat = m_UserStats[userStatIndex];
            if (userStat != null)
            {
                int statId = userStat.hashId;
                if (statId == i_StatId)
                {
                    if (userStat.type == UserStatType.Int)
                    {
                        UserStatInt intUserStat = (UserStatInt)userStat;
                        intUserStat.onValueChangedEvent += i_Handler;
                        break;
                    }
                }
            }
        }
    }

    public void RegisterBoolStatHandler(string i_StatId, UserStatValueChangedEvent<bool> i_Handler)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        RegisterBoolStatHandler(hash, i_Handler);
    }

    public void RegisterBoolStatHandler(int i_StatId, UserStatValueChangedEvent<bool> i_Handler)
    {
        for (int userStatIndex = 0; userStatIndex < m_UserStats.Count; ++userStatIndex)
        {
            UserStat userStat = m_UserStats[userStatIndex];
            if (userStat != null)
            {
                int statId = userStat.hashId;
                if (statId == i_StatId)
                {
                    if (userStat.type == UserStatType.Bool)
                    {
                        UserStatBool intUserStat = (UserStatBool)userStat;
                        intUserStat.onValueChangedEvent += i_Handler;
                        break;
                    }
                }
            }
        }
    }

    public void RegisterFloatStatHandler(string i_StatId, UserStatValueChangedEvent<float> i_Handler)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        RegisterFloatStatHandler(hash, i_Handler);
    }

    public void RegisterFloatStatHandler(int i_StatId, UserStatValueChangedEvent<float> i_Handler)
    {
        for (int userStatIndex = 0; userStatIndex < m_UserStats.Count; ++userStatIndex)
        {
            UserStat userStat = m_UserStats[userStatIndex];
            if (userStat != null)
            {
                int statId = userStat.hashId;
                if (statId == i_StatId)
                {
                    if (userStat.type == UserStatType.Float)
                    {
                        UserStatFloat intUserStat = (UserStatFloat)userStat;
                        intUserStat.onValueChangedEvent += i_Handler;
                        break;
                    }
                }
            }
        }
    }

    public void RegisterStringStatHandler(string i_StatId, UserStatValueChangedEvent<string> i_Handler)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        RegisterStringStatHandler(hash, i_Handler);
    }

    public void RegisterStringStatHandler(int i_StatId, UserStatValueChangedEvent<string> i_Handler)
    {
        for (int userStatIndex = 0; userStatIndex < m_UserStats.Count; ++userStatIndex)
        {
            UserStat userStat = m_UserStats[userStatIndex];
            if (userStat != null)
            {
                int statId = userStat.hashId;
                if (statId == i_StatId)
                {
                    if (userStat.type == UserStatType.String)
                    {
                        UserStatString intUserStat = (UserStatString)userStat;
                        intUserStat.onValueChangedEvent += i_Handler;
                        break;
                    }
                }
            }
        }
    }

    public void UnregisterIntStatHandler(string i_StatId, UserStatValueChangedEvent<int> i_Handler)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        UnregisterIntStatHandler(hash, i_Handler);
    }

    public void UnregisterIntStatHandler(int i_StatId, UserStatValueChangedEvent<int> i_Handler)
    {
        for (int userStatIndex = 0; userStatIndex < m_UserStats.Count; ++userStatIndex)
        {
            UserStat userStat = m_UserStats[userStatIndex];
            if (userStat != null)
            {
                int statId = userStat.hashId;
                if (statId == i_StatId)
                {
                    if (userStat.type == UserStatType.Int)
                    {
                        UserStatInt intUserStat = (UserStatInt)userStat;
                        intUserStat.onValueChangedEvent -= i_Handler;
                        break;
                    }
                }
            }
        }
    }

    public void UnregisterBoolStatHandler(string i_StatId, UserStatValueChangedEvent<bool> i_Handler)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        UnregisterBoolStatHandler(hash, i_Handler);
    }

    public void UnregisterBoolStatHandler(int i_StatId, UserStatValueChangedEvent<bool> i_Handler)
    {
        for (int userStatIndex = 0; userStatIndex < m_UserStats.Count; ++userStatIndex)
        {
            UserStat userStat = m_UserStats[userStatIndex];
            if (userStat != null)
            {
                int statId = userStat.hashId;
                if (statId == i_StatId)
                {
                    if (userStat.type == UserStatType.Bool)
                    {
                        UserStatBool intUserStat = (UserStatBool)userStat;
                        intUserStat.onValueChangedEvent -= i_Handler;
                        break;
                    }
                }
            }
        }
    }

    public void UnregisterFloatStatHandler(string i_StatId, UserStatValueChangedEvent<float> i_Handler)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        UnregisterFloatStatHandler(hash, i_Handler);
    }

    public void UnregisterFloatStatHandler(int i_StatId, UserStatValueChangedEvent<float> i_Handler)
    {
        for (int userStatIndex = 0; userStatIndex < m_UserStats.Count; ++userStatIndex)
        {
            UserStat userStat = m_UserStats[userStatIndex];
            if (userStat != null)
            {
                int statId = userStat.hashId;
                if (statId == i_StatId)
                {
                    if (userStat.type == UserStatType.Float)
                    {
                        UserStatFloat intUserStat = (UserStatFloat)userStat;
                        intUserStat.onValueChangedEvent -= i_Handler;
                        break;
                    }
                }
            }
        }
    }

    public void UnregisterStringStatHandler(string i_StatId, UserStatValueChangedEvent<string> i_Handler)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        UnregisterStringStatHandler(hash, i_Handler);
    }

    public void UnregisterStringStatHandler(int i_StatId, UserStatValueChangedEvent<string> i_Handler)
    {
        for (int userStatIndex = 0; userStatIndex < m_UserStats.Count; ++userStatIndex)
        {
            UserStat userStat = m_UserStats[userStatIndex];
            if (userStat != null)
            {
                int statId = userStat.hashId;
                if (statId == i_StatId)
                {
                    if (userStat.type == UserStatType.String)
                    {
                        UserStatString intUserStat = (UserStatString)userStat;
                        intUserStat.onValueChangedEvent -= i_Handler;
                        break;
                    }
                }
            }
        }
    }

    public UserStatInt GetIntStat(string i_StatId)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        return GetIntStat(hash);
    }

    public UserStatInt GetIntStat(int i_StatId)
    {
        for (int userStatIndex = 0; userStatIndex < m_UserStats.Count; ++userStatIndex)
        {
            UserStat userStat = m_UserStats[userStatIndex];
            if (userStat != null)
            {
                int statId = userStat.hashId;
                if (statId == i_StatId)
                {
                    if (userStat.type == UserStatType.Int)
                    {
                        UserStatInt intUserStat = (UserStatInt)userStat;
                        return intUserStat;
                    }
                }
            }
        }

        return null;
    }

    public UserStatBool GetBoolStat(string i_StatId)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        return GetBoolStat(hash);
    }

    public UserStatBool GetBoolStat(int i_StatId)
    {
        for (int userStatIndex = 0; userStatIndex < m_UserStats.Count; ++userStatIndex)
        {
            UserStat userStat = m_UserStats[userStatIndex];
            if (userStat != null)
            {
                int statId = userStat.hashId;
                if (statId == i_StatId)
                {
                    if (userStat.type == UserStatType.Bool)
                    {
                        UserStatBool boolUserStat = (UserStatBool)userStat;
                        return boolUserStat;
                    }
                }
            }
        }

        return null;
    }

    public UserStatFloat GetFloatStat(string i_StatId)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        return GetFloatStat(hash);
    }

    public UserStatFloat GetFloatStat(int i_StatId)
    {
        for (int userStatIndex = 0; userStatIndex < m_UserStats.Count; ++userStatIndex)
        {
            UserStat userStat = m_UserStats[userStatIndex];
            if (userStat != null)
            {
                int statId = userStat.hashId;
                if (statId == i_StatId)
                {
                    if (userStat.type == UserStatType.Float)
                    {
                        UserStatFloat floatUserStat = (UserStatFloat)userStat;
                        return floatUserStat;
                    }
                }
            }
        }

        return null;
    }

    public UserStatString GetStringStat(string i_StatId)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        return GetStringStat(hash);
    }

    public UserStatString GetStringStat(int i_StatId)
    {
        for (int userStatIndex = 0; userStatIndex < m_UserStats.Count; ++userStatIndex)
        {
            UserStat userStat = m_UserStats[userStatIndex];
            if (userStat != null)
            {
                int statId = userStat.hashId;
                if (statId == i_StatId)
                {
                    if (userStat.type == UserStatType.String)
                    {
                        UserStatString stringUserStat = (UserStatString)userStat;
                        return stringUserStat;
                    }
                }
            }
        }

        return null;
    }

    public bool TryGetIntStatValue(string i_StatId, out int o_Value)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        return TryGetIntStatValue(hash, out o_Value);
    }

    public bool TryGetIntStatValue(int i_StatId, out int o_Value)
    {
        o_Value = 0;

        for (int userStatIndex = 0; userStatIndex < m_UserStats.Count; ++userStatIndex)
        {
            UserStat userStat = m_UserStats[userStatIndex];
            if (userStat != null)
            {
                int statId = userStat.hashId;
                if (statId == i_StatId)
                {
                    if (userStat.type == UserStatType.Int)
                    {
                        UserStatInt intUserStat = (UserStatInt)userStat;
                        o_Value = intUserStat.intValue;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public bool TryGetBoolStatValue(string i_StatId, out bool o_Value)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        return TryGetBoolStatValue(hash, out o_Value);
    }

    public bool TryGetBoolStatValue(int i_StatId, out bool o_Value)
    {
        o_Value = false;

        for (int userStatIndex = 0; userStatIndex < m_UserStats.Count; ++userStatIndex)
        {
            UserStat userStat = m_UserStats[userStatIndex];
            if (userStat != null)
            {
                int statId = userStat.hashId;
                if (statId == i_StatId)
                {
                    if (userStat.type == UserStatType.Bool)
                    {
                        UserStatBool boolUserStat = (UserStatBool)userStat;
                        o_Value = boolUserStat.boolValue;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public bool TryGetFloatStatValue(string i_StatId, out float o_Value)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        return TryGetFloatStatValue(hash, out o_Value);
    }

    public bool TryGetFloatStatValue(int i_StatId, out float o_Value)
    {
        o_Value = 0f;

        for (int userStatIndex = 0; userStatIndex < m_UserStats.Count; ++userStatIndex)
        {
            UserStat userStat = m_UserStats[userStatIndex];
            if (userStat != null)
            {
                int statId = userStat.hashId;
                if (statId == i_StatId)
                {
                    if (userStat.type == UserStatType.Float)
                    {
                        UserStatFloat floatUserStat = (UserStatFloat)userStat;
                        o_Value = floatUserStat.floatValue;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public bool TryGetStringStatValue(string i_StatId, out string o_Value)
    {
        int hash = StringUtils.GetHashCode(i_StatId);
        return TryGetStringStatValue(hash, out o_Value);
    }

    public bool TryGetStringStatValue(int i_StatId, out string o_Value)
    {
        o_Value = "";

        for (int userStatIndex = 0; userStatIndex < m_UserStats.Count; ++userStatIndex)
        {
            UserStat userStat = m_UserStats[userStatIndex];
            if (userStat != null)
            {
                int statId = userStat.hashId;
                if (statId == i_StatId)
                {
                    if (userStat.type == UserStatType.String)
                    {
                        UserStatString stringUserStat = (UserStatString)userStat;
                        o_Value = stringUserStat.stringValue;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    // STATIC INTERNALS

    private static UserStat CreateUserStat(UserStatDescriptor i_Descriptor)
    {
        UserStat userStat = null;

        string id = i_Descriptor.id;
        if (id != "")
        {
            switch (i_Descriptor.type)
            {
                case UserStatType.Bool:

                    bool defaultBoolValue = i_Descriptor.defaultBoolValue;
                    BooleanCombineFunction booleanCombineFunction = i_Descriptor.booleanCombineFunction;

                    userStat = new UserStatBool(id, defaultBoolValue, booleanCombineFunction);

                    break;

                case UserStatType.Float:

                    float defaultFloatValue = i_Descriptor.defaultFloatValue;
                    bool usingFloatLowLimit = i_Descriptor.usingLowLimit;
                    bool usingFloatHighLimit = i_Descriptor.usingHighLimit;
                    float minFloatValue = i_Descriptor.minFloatValue;
                    float maxFloatValue = i_Descriptor.maxFloatValue;
                    NumericCombineFunction floatCombineFunction = i_Descriptor.numericCombineFunction;

                    userStat = new UserStatFloat(id, defaultFloatValue, usingFloatLowLimit, usingFloatHighLimit, minFloatValue, maxFloatValue, floatCombineFunction);

                    break;

                case UserStatType.Int:

                    int defaultIntValue = i_Descriptor.defaultIntValue;
                    bool usingIntLowLimit = i_Descriptor.usingLowLimit;
                    bool usingIntHighLimit = i_Descriptor.usingHighLimit;
                    int minIntValue = i_Descriptor.minIntValue;
                    int maxIntValue = i_Descriptor.maxIntValue;
                    NumericCombineFunction intCombineFunction = i_Descriptor.numericCombineFunction;

                    userStat = new UserStatInt(id, defaultIntValue, usingIntLowLimit, usingIntHighLimit, minIntValue, maxIntValue, intCombineFunction);

                    break;

                case UserStatType.String:

                    string defaultStringValue = i_Descriptor.defaultStringValue;
                    StringCombineFunction stringCombineFunction = i_Descriptor.stringCombineFunction;

                    userStat = new UserStatString(id, defaultStringValue, stringCombineFunction);

                    break;
            }
        }

        return userStat;
    }

    // INTERNALS

    private void InternalUpdateIntStat(UserStatInt i_Stat, int i_Value, IntStatUpdateFunction i_UpdateFunction)
    {
        switch (i_UpdateFunction)
        {
            case IntStatUpdateFunction.Add:
                i_Stat.Add(i_Value);
                break;

            case IntStatUpdateFunction.Max:
                i_Stat.Max(i_Value);
                break;

            case IntStatUpdateFunction.Min:
                i_Stat.Min(i_Value);
                break;

            case IntStatUpdateFunction.Multiply:
                i_Stat.Multiply(i_Value);
                break;

            case IntStatUpdateFunction.Set:
                i_Stat.Set(i_Value);
                break;
        }
    }

    private void InternalUpdateBoolStat(UserStatBool i_Stat, bool i_Value, BoolStatUpdateFunction i_UpdateFunction)
    {
        switch (i_UpdateFunction)
        {
            case BoolStatUpdateFunction.Add:
                i_Stat.Add(i_Value);
                break;

            case BoolStatUpdateFunction.Multiply:
                i_Stat.Multiply(i_Value);
                break;

            case BoolStatUpdateFunction.Set:
                i_Stat.Set(i_Value);
                break;
        }
    }

    private void InternalUpdateFloatStat(UserStatFloat i_Stat, float i_Value, FloatStatUpdateFunction i_UpdateFunction)
    {
        switch (i_UpdateFunction)
        {
            case FloatStatUpdateFunction.Add:
                i_Stat.Add(i_Value);
                break;

            case FloatStatUpdateFunction.Max:
                i_Stat.Max(i_Value);
                break;

            case FloatStatUpdateFunction.Min:
                i_Stat.Min(i_Value);
                break;

            case FloatStatUpdateFunction.Multiply:
                i_Stat.Multiply(i_Value);
                break;

            case FloatStatUpdateFunction.Set:
                i_Stat.Set(i_Value);
                break;
        }
    }

    private void InternalUpdateStringStat(UserStatString i_Stat, string i_Value, StringStatUpdateFunction i_UpdateFunction)
    {
        switch (i_UpdateFunction)
        {
            case StringStatUpdateFunction.Add:
                i_Stat.Add(i_Value);
                break;

            case StringStatUpdateFunction.Max:
                i_Stat.Max(i_Value);
                break;

            case StringStatUpdateFunction.Min:
                i_Stat.Min(i_Value);
                break;

            case StringStatUpdateFunction.Set:
                i_Stat.Set(i_Value);
                break;
        }
    }

    // CTOR

    public UserStatsManager(string i_DatabaseResourcePath)
    {
        m_UserStats = new List<UserStat>();
        m_DatabaseResourcePath = i_DatabaseResourcePath;
    }
}