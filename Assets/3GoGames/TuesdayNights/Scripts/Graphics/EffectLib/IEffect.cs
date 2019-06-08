using UnityEngine;

using System.Collections;

public delegate void AnimCompletedCallback();
public delegate void AnimEventCallback(string i_EventName);

public interface IEffect
{
    bool isPlaying { get; }

    void Play(AnimCompletedCallback i_AnimCompletedCallback = null, AnimEventCallback i_AnimEventCallback = null);
    void Stop();
}