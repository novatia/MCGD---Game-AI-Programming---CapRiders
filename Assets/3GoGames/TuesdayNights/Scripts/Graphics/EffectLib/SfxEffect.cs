using UnityEngine;
using UnityEngine.Audio;

public class SfxEffect : Effect 
{
    public AudioClip[] clips;
    public AudioMixerGroup audioMixerGroup = null;

    public float minVolume = 1f;
    public float maxVolume = 1f;

    public float minPitch = 1f;
    public float maxPitch = 1f;

    protected override void OnPlay(AnimEventCallback i_Unused = null)
    {
        if (clips == null || clips.Length == 0)
        {
            Finish();
        }
        else
        {
            int clipIndex = Random.Range(0, clips.Length);
            AudioClip selectedClip = clips[clipIndex];

            float clipVolume = Random.Range(minVolume, maxVolume);
            float clipPitch = Random.Range(minPitch, maxPitch);

            SfxPlayer.PlayMain(selectedClip, audioMixerGroup, clipVolume, clipPitch);
            Finish();
        }
    }

    protected override void OnStop()
    {

    }
}
