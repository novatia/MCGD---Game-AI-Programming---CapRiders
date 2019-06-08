using UnityEngine;

public static class LogManager
{

    public static void Log(System.Object source, string logMessage)
    {
        Debug.Log("[" + source.GetType().ToString() + "][" + Time.time + "]" + " " + logMessage);
    }

    public static void Log(System.Object source, string context, string logMessage)
    {
        Debug.Log("[" + context + "][" + source.GetType().ToString() + "][" + Time.time + "]" + " " + logMessage);
    }

    public static void Log(MonoBehaviour source, string context, string logMessage)
    {
        Debug.Log("[" + context + "][" + source.gameObject.name + "]" + "[" + source.name + "][" + Time.time + "]" + " " + logMessage);
    }

    public static void LogWarning(System.Object source, string logMessage)
    {
        Debug.LogWarning("[" + source.GetType().ToString() + "][" + Time.time + "]" + " " + logMessage);
    }

    public static void LogWarning(MonoBehaviour source, string context, string logMessage)
    {
        Debug.LogWarning("[" + context + "][" + source.gameObject.name + "]" + "[" + source.name + "][" + Time.time + "]" + " " + logMessage);
    }

    public static void LogError(System.Object source, string logMessage)
    {
        Debug.LogError("[" + source.GetType().ToString() + "][" + Time.time + "]" + " " + logMessage);
    }

    public static void LogError(MonoBehaviour source, string context, string logMessage)
    {
        Debug.LogError("[" + context + "][" + source.gameObject.name + "]" + "[" + source.name + "][" + Time.time + "]" + " " + logMessage);
    }

}
