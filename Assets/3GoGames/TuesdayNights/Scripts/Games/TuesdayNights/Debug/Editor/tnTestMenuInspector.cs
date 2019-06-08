using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(tnTestMenu))]
public class tnTestMenuInspector : Editor
{
    // Fields

    private SerializedProperty m_GameModeProperty = null;
    private SerializedProperty m_BallProperty = null;
    private SerializedProperty m_StadiumProperty = null;
    private SerializedProperty m_MatchDurationProperty = null;
    private SerializedProperty m_GoldenGoalProperty = null;
    private SerializedProperty m_RefereeProperty = null;

    private SerializedProperty m_TeamAProperty = null;
    private SerializedProperty m_TeamBProperty = null;
    private SerializedProperty m_PlayersPerTeamProperty = null;

    private SerializedProperty m_AllHumansProperty = null;
    private SerializedProperty m_AllCPUsProperty = null;

    private void OnEnable()
    {
        m_GameModeProperty = serializedObject.FindProperty("m_GameMode");
        m_BallProperty = serializedObject.FindProperty("m_Ball");
        m_StadiumProperty = serializedObject.FindProperty("m_Stadium");
        m_MatchDurationProperty = serializedObject.FindProperty("m_MatchDuration");
        m_GoldenGoalProperty = serializedObject.FindProperty("m_GoldenGoal");
        m_RefereeProperty = serializedObject.FindProperty("m_Referee");

        m_TeamAProperty = serializedObject.FindProperty("m_TeamA");
        m_TeamBProperty = serializedObject.FindProperty("m_TeamB");
        m_PlayersPerTeamProperty = serializedObject.FindProperty("m_PlayersPerTeam");

        m_AllHumansProperty = serializedObject.FindProperty("m_AllHumans");
        m_AllCPUsProperty = serializedObject.FindProperty("m_AllCPUs");
    }

    public override void OnInspectorGUI()
    {
        tnTestMenu testMenu = target as tnTestMenu;

        if (testMenu == null)
            return;

        string stadiumId = testMenu.GetStadiumId();

        EditorGUI.BeginChangeCheck();
        bool forceApply = false;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Match Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_GameModeProperty);
        EditorGUILayout.PropertyField(m_BallProperty);
        EditorGUILayout.PropertyField(m_StadiumProperty);
        EditorGUILayout.PropertyField(m_MatchDurationProperty);
        EditorGUILayout.PropertyField(m_GoldenGoalProperty);
        EditorGUILayout.PropertyField(m_RefereeProperty);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Teams", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_TeamAProperty);
        EditorGUILayout.PropertyField(m_TeamBProperty);

        int value = m_PlayersPerTeamProperty.intValue;
        int min = GetMinPlayers(stadiumId);
        int max = GetMaxPlayers(stadiumId);

        if (min > value || max < value)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("This stadium doesn't allow " + value + " players per team.", EditorStyles.miniLabel);
            EditorGUILayout.LabelField(min + "<= players per team <= " + max, EditorStyles.miniLabel);
        }

        EditorGUILayout.PropertyField(m_PlayersPerTeamProperty);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Input", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_AllHumansProperty);
        EditorGUILayout.PropertyField(m_AllCPUsProperty);

        if (EditorGUI.EndChangeCheck() || forceApply)
        {
            serializedObject.ApplyModifiedProperties();
        }

        serializedObject.Update();
    }

    // UTILS

    private int GetMinPlayers(string i_StadiumId)
    {
        tnStadiumsDatabase stadiumDatabase = Resources.Load<tnStadiumsDatabase>("Database/Game/StadiumsDatabase");

        if (stadiumDatabase == null)
        {
            return 0;
        }

        for (int index = 0; index < stadiumDatabase.stadiumsCount; ++index)
        {
            tnStadiumDataEntry entry = stadiumDatabase.GetStadiumDataEntry(index);
            if (entry.id == i_StadiumId)
            {
                tnStadiumDataDescriptor stadiumDataDescriptor = entry.descriptor;
                if (stadiumDataDescriptor != null)
                {
                    return stadiumDataDescriptor.minTeamSize;
                }
            }
        }

        return 0;
    }

    private int GetMaxPlayers(string i_StadiumId)
    {
        tnStadiumsDatabase stadiumDatabase = Resources.Load<tnStadiumsDatabase>("Database/Game/StadiumsDatabase");

        if (stadiumDatabase == null)
        {
            return int.MaxValue;
        }

        for (int index = 0; index < stadiumDatabase.stadiumsCount; ++index)
        {
            tnStadiumDataEntry entry = stadiumDatabase.GetStadiumDataEntry(index);
            if (entry.id == i_StadiumId)
            {
                tnStadiumDataDescriptor stadiumDataDescriptor = entry.descriptor;
                if (stadiumDataDescriptor != null)
                {
                    return stadiumDataDescriptor.maxTeamSize;
                }
            }
        }

        return int.MaxValue;
    }
}
