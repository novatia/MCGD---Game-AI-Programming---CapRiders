using UnityEngine;
using System.Collections;

public class tnCountdownController : MonoBehaviour
{
    private Animator m_Animator = null;

    private static int s_Trigger_3 = Animator.StringToHash("Countdown_3");
    private static int s_Trigger_2 = Animator.StringToHash("Countdown_2");
    private static int s_Trigger_1 = Animator.StringToHash("Countdown_1");
    private static int s_Trigger_Go = Animator.StringToHash("Countdown_Go");

    void Awake()
    {
        m_Animator = GetComponentInChildren<Animator>();
    }

    void OnEnable()
    {
        Messenger.AddListener<int>("Countdown", OnCountdown);
    }

    void OnDisable()
    {
        Messenger.RemoveListener<int>("Countdown", OnCountdown);
    }

    // EVENTS

    private void OnCountdown(int i_CountdownPhase)
    {
        if (m_Animator == null)
            return;

        switch (i_CountdownPhase)
        {
            case 0:         // 3

                {
                    m_Animator.SetTrigger(s_Trigger_3);
                }

                break;

            case 1:         // 2

                {
                    m_Animator.SetTrigger(s_Trigger_2);
                }

                break;

            case 2:         // 1

                {
                    m_Animator.SetTrigger(s_Trigger_1);
                }

                break;

            case 3:         // GO

                {
                    m_Animator.SetTrigger(s_Trigger_Go);
                }

                break;
        }
    }
}
