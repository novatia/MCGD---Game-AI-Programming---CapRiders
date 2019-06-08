using UnityEngine;

using System.Collections;

public class _TestPlayer : MonoBehaviour
{
    public Effect effect = null;
    
    public KeyCode playKey = KeyCode.P;
    public KeyCode stopKey = KeyCode.S;

    void Update()
    {
        if (effect == null)
            return;

        if (Input.GetKeyDown(playKey))
        {
            effect.Play(OnEffectCompleted);
        }

        if (Input.GetKeyDown(stopKey))
        {
            effect.Stop();
        }
    }

    private void OnEffectCompleted()
    {
        Debug.Log("Effect completed");
    }
}