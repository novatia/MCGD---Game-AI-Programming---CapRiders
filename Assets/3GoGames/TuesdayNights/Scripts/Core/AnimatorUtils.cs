using UnityEngine;

public static class AnimatorUtils
{
    public static bool HasParameter(this Animator i_Animator, int i_ParamNameHash)
    {
        if (i_Animator == null)
        {
            return false;
        }

        for (int index = 0; index < i_Animator.parameterCount; ++index)
        {
            AnimatorControllerParameter parameter = i_Animator.GetParameter(index);

            if (parameter == null)
                continue;

            if (parameter.nameHash == i_ParamNameHash)
            {
                return true;
            }
        }

        return false;
    }

    public static bool HasParameter(this Animator i_Animator, int i_ParamNameHash, AnimatorControllerParameterType i_Type)
    {
        if (i_Animator == null)
        {
            return false;
        }

        for (int index = 0; index < i_Animator.parameterCount; ++index)
        {
            AnimatorControllerParameter parameter = i_Animator.GetParameter(index);

            if (parameter == null)
                continue;

            if (parameter.nameHash == i_ParamNameHash && parameter.type == i_Type)
            {
                return true;
            }
        }

        return false;
    }

    public static void Safe_SetTrigger(this Animator i_Animator, int i_ParamNameHash)
    {
        if (i_Animator == null)
            return;

        if (i_Animator.HasParameter(i_ParamNameHash, AnimatorControllerParameterType.Trigger))
        {
            i_Animator.SetTrigger(i_ParamNameHash);
        }
    }

    public static void Safe_SetBool(this Animator i_Animator, int i_ParamNameHash, bool i_Value)
    {
        if (i_Animator == null)
            return;

        if (i_Animator.HasParameter(i_ParamNameHash, AnimatorControllerParameterType.Bool))
        {
            i_Animator.SetBool(i_ParamNameHash, i_Value);
        }
    }

    public static void Safe_SetInt(this Animator i_Animator, int i_ParamNameHash, int i_Value)
    {
        if (i_Animator == null)
            return;

        if (i_Animator.HasParameter(i_ParamNameHash, AnimatorControllerParameterType.Int))
        {
            i_Animator.SetInteger(i_ParamNameHash, i_Value);
        }
    }

    public static void Safe_SetFloat(this Animator i_Animator, int i_ParamNameHash, float i_Value)
    {
        if (i_Animator == null)
            return;

        if (i_Animator.HasParameter(i_ParamNameHash, AnimatorControllerParameterType.Float))
        {
            i_Animator.SetFloat(i_ParamNameHash, i_Value);
        }
    }
}
