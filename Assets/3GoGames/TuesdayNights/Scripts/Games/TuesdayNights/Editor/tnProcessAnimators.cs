using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public static class tnProcessAnimators
{
    private static string s_ControllerBasePath = "Assets/3GoGames/TuesdayNights/Animations/Controllers/Characters";
    private static string s_ClipBasePath = "Assets/3GoGames/TuesdayNights/Animations/Clips/Characters";

    private static string s_IdleRClipName = "Idle_R";
    private static string s_IdleLClipName = "Idle_L";

    private static string s_EffectNoneClipName = "Effect_None";
    private static string s_EffectBlinkClipName = "Effect_Blink";

    private static string s_ParameterFacingRight = "FacingRight";
    private static string s_ParameterInCooldown = "InCooldown";

    [MenuItem("TuesdayNights/Process Animators")]
    public static void CreateAnimationPlayer()
    {
        // Load character database.

        tnCharactersDatabase characterDatabase = Resources.Load<tnCharactersDatabase>("Database/Game/CharactersDatabase");

        if (characterDatabase == null)
        {
            Debug.LogError("Character database not found.");
            return;
        }

        for (int index = 0; index < characterDatabase.charactersCount; ++index)
        {
            tnCharacterDataEntry entry = characterDatabase.GetCharacterDataEntry(index);
            if (entry != null)
            {
                EditorUtility.DisplayProgressBar("Processing", "Processing Animator for " + entry.id + "...", Mathf.Clamp01((float)index / characterDatabase.charactersCount));

                tnCharacterDataDescriptor descriptor = entry.descriptor;
                if (descriptor != null)
                {
                    SerializedObject serializedDescriptor = new SerializedObject(descriptor);

                    SerializedProperty serializedProperty = serializedDescriptor.FindProperty("m_AnimatorController");
                    serializedProperty.objectReferenceValue = CreateAnimatorController(entry.id, entry.descriptor);

                    serializedDescriptor.ApplyModifiedProperties();
                }
            }
        }

        AssetDatabase.SaveAssets();

        EditorUtility.ClearProgressBar();
    }

    private static AnimatorController CreateAnimatorController(string i_CharacterName, tnCharacterDataDescriptor i_Descriptor)
    {
        if (i_Descriptor == null)
        {
            return null;
        }

        return CreateAnimatorController(i_CharacterName, i_Descriptor.leftFrames, i_Descriptor.rightFrames);
    }

    private static AnimatorController CreateAnimatorController(string i_CharacterName, Sprite[] i_Left, Sprite[] i_Right)
    {
        // Create controller.

        string path = s_ControllerBasePath + "/" + i_CharacterName + ".controller";
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(path);

        // Add parameters.

        controller.AddParameter(s_ParameterFacingRight, AnimatorControllerParameterType.Bool);
        controller.AddParameter(s_ParameterInCooldown, AnimatorControllerParameterType.Bool);

        // Add layers.

        if (controller.layers.Length == 0)
        {
            controller.AddLayer("Base Layer");
        }

        controller.AddLayer("Effects Layer");

        // Configure Base Layer.
        
        {
            AnimatorControllerLayer baseLayer = controller.layers[0];

            AnimatorStateMachine rootStateMachine = baseLayer.stateMachine;

            // Add States

            AnimatorState stateRight = rootStateMachine.AddState("Idle R");
            AnimatorState stateLeft = rootStateMachine.AddState("Idle L");

            // Add Clips

            float[] c_Times = { 0f, 0.5f, 1.0f, 1.5f, 2f };
            int[] c_Indices = { 0, 1, 2, 1, 0 };

            AnimationClip idleRightClip = CreateAnimationClip(i_Right, c_Times, c_Indices);
            AnimationClip idleLeftClip = CreateAnimationClip(i_Left, c_Times, c_Indices);

            if (idleRightClip != null)
            {
                AssetDatabase.CreateAsset(idleRightClip, s_ClipBasePath + "/" + i_CharacterName + "_" + s_IdleRClipName + ".anim");
            }

            if (idleLeftClip != null)
            {
                AssetDatabase.CreateAsset(idleLeftClip, s_ClipBasePath + "/" + i_CharacterName + "_" + s_IdleLClipName + ".anim");
            }

            stateRight.motion = idleRightClip;
            stateLeft.motion = idleLeftClip;

            // Add Transitions

            AnimatorStateTransition transitionLR = stateLeft.AddTransition(stateRight);
            transitionLR.hasExitTime = false;
            transitionLR.hasFixedDuration = true;
            transitionLR.duration = 0f;
            transitionLR.offset = 0f;
            transitionLR.interruptionSource = TransitionInterruptionSource.None;

            AnimatorStateTransition transitionRL = stateRight.AddTransition(stateLeft);
            transitionRL.hasExitTime = false;
            transitionRL.hasFixedDuration = true;
            transitionRL.duration = 0f;
            transitionRL.offset = 0f;
            transitionRL.interruptionSource = TransitionInterruptionSource.None;

            // Add Condition

            transitionLR.AddCondition(AnimatorConditionMode.If, 1, s_ParameterFacingRight);
            transitionRL.AddCondition(AnimatorConditionMode.IfNot, 1, s_ParameterFacingRight);
        }

        // Configure Effects Layer.

        {
            AnimatorControllerLayer effectsLayer = controller.layers[1];

            AnimatorStateMachine rootStateMachine = effectsLayer.stateMachine;

            // Add States

            AnimatorState stateNone = rootStateMachine.AddState("None");
            AnimatorState stateBlink = rootStateMachine.AddState("Blink");

            // Add Clips

            AnimationClip effectNoneClip = null;
            AnimationClip effectBlinkClip = null;

            {
                float[] c_Times = { 0f };
                float[] c_Alpha = { 1f };

                effectNoneClip = CreateBlinkClip(c_Times, c_Alpha);

                if (effectNoneClip != null)
                {
                    AssetDatabase.CreateAsset(effectNoneClip, s_ClipBasePath + "/" + i_CharacterName + "_" + s_EffectNoneClipName + ".anim");
                }
            }

            {
                float[] c_Times = { 0f, 0.25f, 0.5f };
                float[] c_Alpha = { 1f, 0f, 1f };

                effectBlinkClip = CreateBlinkClip(c_Times, c_Alpha);

                if (effectBlinkClip != null)
                {
                    AssetDatabase.CreateAsset(effectBlinkClip, s_ClipBasePath + "/" + i_CharacterName + "_" + s_EffectBlinkClipName + ".anim");
                }
            }

            stateNone.motion = effectNoneClip;
            stateBlink.motion = effectBlinkClip;

            // Add Transitions

            AnimatorStateTransition transitionToBlink = stateNone.AddTransition(stateBlink);
            transitionToBlink.hasExitTime = false;
            transitionToBlink.hasFixedDuration = true;
            transitionToBlink.duration = 0f;
            transitionToBlink.offset = 0f;
            transitionToBlink.interruptionSource = TransitionInterruptionSource.None;

            AnimatorStateTransition transitionToNone = stateBlink.AddTransition(stateNone);
            transitionToNone.hasExitTime = false;
            transitionToNone.hasFixedDuration = true;
            transitionToNone.duration = 0f;
            transitionToNone.offset = 0f;
            transitionToNone.interruptionSource = TransitionInterruptionSource.None;

            // Add Condition

            transitionToBlink.AddCondition(AnimatorConditionMode.If, 1, s_ParameterInCooldown);
            transitionToNone.AddCondition(AnimatorConditionMode.IfNot, 1, s_ParameterInCooldown);
        }

        return controller;
    }

    private static AnimationClip CreateAnimationClip(Sprite[] i_Sprites, float[] i_Times, int[] i_Indices)
    {
        if (i_Sprites == null || i_Sprites.Length == 0)
        {
            return null;
        }

        if (i_Times == null || i_Times.Length == 0)
        {
            return null;
        }

        if (i_Indices == null || i_Indices.Length == 0)
        {
            return null;
        }

        if (i_Times.Length != i_Indices.Length)
        {
            return null;
        }

        AnimationClip newClip = new AnimationClip();

        AnimationClipSettings clipSettings = AnimationUtility.GetAnimationClipSettings(newClip);
        clipSettings.loopTime = true;

        AnimationUtility.SetAnimationClipSettings(newClip, clipSettings);

        EditorCurveBinding curveBinding = new EditorCurveBinding();
        curveBinding.type = typeof(SpriteRenderer);
        curveBinding.path = "";
        curveBinding.propertyName = "m_Sprite";

        int numKeyframes = i_Times.Length;

        ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[numKeyframes];
        for (int i = 0; i < numKeyframes; ++i)
        {
            keyframes[i] = new ObjectReferenceKeyframe();
            keyframes[i].time = i_Times[i];
            keyframes[i].value = i_Sprites[i_Indices[i]];
        }

        AnimationUtility.SetObjectReferenceCurve(newClip, curveBinding, keyframes);

        return newClip;
    }

    private static AnimationClip CreateBlinkClip(float[] i_Times, float[] i_AlphaValues)
    {
        if (i_Times == null || i_Times.Length == 0)
        {
            return null;
        }

        AnimationClip newClip = new AnimationClip();

        AnimationClipSettings clipSettings = AnimationUtility.GetAnimationClipSettings(newClip);
        clipSettings.loopTime = true;

        AnimationUtility.SetAnimationClipSettings(newClip, clipSettings);

        int numKeyframes = i_Times.Length;

        Keyframe[] keyframes = new Keyframe[numKeyframes];
        for (int i = 0; i < numKeyframes; ++i)
        {
            keyframes[i] = new Keyframe();
            keyframes[i].time = i_Times[i];
            keyframes[i].value = i_AlphaValues[i];
        }

        newClip.SetCurve("", typeof(SpriteRenderer), "m_Color.a", new AnimationCurve(keyframes));
        return newClip;
    }
}
