using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class EffectInstancer : MonoBehaviour
{
    private List<Effect> spawnedEffect = new List<Effect>();

    public Effect effect = null;

    public bool recycleEffects = true;

    // MonoBehaviour 's interface

    void OnDisable()
    {
        if (!recycleEffects)
            return;

        for (int effectIndex = 0; effectIndex < spawnedEffect.Count; ++effectIndex)
        {
            Effect currentEffect = spawnedEffect[effectIndex];

            currentEffect.Stop();
            DestroyEffect(currentEffect);
        }

        spawnedEffect.Clear();
    }

    void Update()
    {
        if (!recycleEffects)
            return;

        for (int effectIndex = spawnedEffect.Count - 1; effectIndex >= 0; --effectIndex)
        {
            Effect currentEffect = spawnedEffect[effectIndex];

            if (!currentEffect.isPlaying)
            {
                spawnedEffect.Remove(currentEffect);
                DestroyEffect(currentEffect);
            }
        }
    }

    // BUSINESS LOGIC

    public void SetEffect(Effect i_Effect)
    {
        effect = i_Effect;
    }

    public void PlayEffect(Vector3 i_Position, AnimCompletedCallback i_AnimCompletedCallback = null, AnimEventCallback i_AnimEventCallback = null)
    {
        InternalPlayEffect(null, i_Position, Quaternion.identity, i_AnimCompletedCallback, i_AnimEventCallback);
    }

    public void PlayEffect(Quaternion i_Rotation, AnimCompletedCallback i_AnimCompletedCallback = null, AnimEventCallback i_AnimEventCallback = null)
    {
        InternalPlayEffect(null, Vector3.zero, i_Rotation, i_AnimCompletedCallback, i_AnimEventCallback);
    }

    public void PlayEffect(Vector3 i_Position, Quaternion i_Rotation, AnimCompletedCallback i_AnimCompletedCallback = null, AnimEventCallback i_AnimEventCallback = null)
    {
        InternalPlayEffect(null, i_Position, i_Rotation, i_AnimCompletedCallback, i_AnimEventCallback);
    }

    public void PlayEffect(Transform i_Parent, Vector3 i_Position, Quaternion i_Rotation, AnimCompletedCallback i_AnimCompletedCallback = null, AnimEventCallback i_AnimEventCallback = null)
    {
        InternalPlayEffect(i_Parent, i_Position, i_Rotation, i_AnimCompletedCallback, i_AnimEventCallback);
    }

    public void PlayEffect(Transform i_Parent, Vector3 i_Position, AnimCompletedCallback i_AnimCompletedCallback = null, AnimEventCallback i_AnimEventCallback = null)
    {
        InternalPlayEffect(i_Parent, i_Position, Quaternion.identity, i_AnimCompletedCallback, i_AnimEventCallback);
    }

    public void PlayEffect(Transform i_Parent, AnimCompletedCallback i_AnimCompletedCallback = null, AnimEventCallback i_AnimEventCallback = null)
    {
        InternalPlayEffect(i_Parent, Vector3.zero, Quaternion.identity, i_AnimCompletedCallback, i_AnimEventCallback);
    }

    public void PlayEffect(AnimCompletedCallback i_AnimCompletedCallback = null, AnimEventCallback i_AnimEventCallback = null)
    {
        InternalPlayEffect(null, Vector3.zero, Quaternion.identity, i_AnimCompletedCallback, i_AnimEventCallback);
    }

    // INTERNALS

    private void InternalPlayEffect(Transform i_Parent, Vector3 i_Position, Quaternion i_Rotation, AnimCompletedCallback i_AnimCompletedCallback = null, AnimEventCallback i_AnimEventCallback = null)
    {
        if (effect == null)
            return;

        Effect currentEffect = effect.Spawn<Effect>(i_Parent, i_Position, i_Rotation);
        spawnedEffect.Add(currentEffect);

        currentEffect.Play(i_AnimCompletedCallback, i_AnimEventCallback);
    }

    private void DestroyEffect(Effect i_Effect)
    {
        i_Effect.Recycle<Effect>();
    }
}