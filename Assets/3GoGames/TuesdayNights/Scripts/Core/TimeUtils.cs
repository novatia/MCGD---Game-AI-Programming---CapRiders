using UnityEngine;

public static class TimeUtils
{
    public static string TimeToString(double i_Time, bool i_ForceZeroInMinutes = false, bool i_ForceZeroInSeconds = false)
    {
        int minutes = ((int) i_Time) / 60;
        int seconds = ((int)i_Time) % 60;

        string minutesString = minutes.ToString();

        if (i_ForceZeroInMinutes)
        {
            if (minutes < 10)
            {
                minutesString = "0" + minutesString;
            }
        }

        string secondsString = seconds.ToString();

        if (i_ForceZeroInSeconds)
        {
            if (seconds < 10)
            {
                secondsString = "0" + secondsString;
            }
        }

        return minutesString + ":" + secondsString;
    }

    public static string TimeToString(float i_Time, bool i_ForceZeroInMinutes = false, bool i_ForceZeroInSeconds = false)
    {
        int minutes = ((int)i_Time) / 60;
        int seconds = ((int)i_Time) % 60;

        string minutesString = minutes.ToString();

        if (i_ForceZeroInMinutes)
        {
            if (minutes < 10)
            {
                minutesString = "0" + minutesString;
            }
        }

        string secondsString = seconds.ToString();

        if (i_ForceZeroInSeconds)
        {
            if (seconds < 10)
            {
                secondsString = "0" + secondsString;
            }
        }

        return minutesString + ":" + secondsString;
    }

    public static double RoundTime(double i_Time)
    {
        return (int)i_Time * 1.0; 
    }

    public static int GetRoundedSecond(float i_Seconds)
    {
        int seconds = Mathf.RoundToInt(i_Seconds);
        return seconds;
    }

    public static bool CheckSeconds(float i_A, float i_B)
    {
        int a = GetRoundedSecond(i_A);
        int b = GetRoundedSecond(i_B);

        return (a != b);
    }
}
