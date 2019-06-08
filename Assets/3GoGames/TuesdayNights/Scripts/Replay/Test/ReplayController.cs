using UnityEngine;

public class ReplayController : MonoBehaviour
{
    [SerializeField]
    private KeyCode m_RecordKey = KeyCode.R;
    [SerializeField]
    private KeyCode m_PlayKey = KeyCode.P;

    void Awake()
    {
        if (!ReplaySystem.initializedMain)
        {
            ReplaySystem.InitializeMain();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(m_RecordKey))
        {
            if (ReplaySystem.isPlayingMain)
            {
                Debug.LogWarning("ReplaySystem is playing now. Stop and retry to record.");
            }
            else
            {
                if (ReplaySystem.isRecordingMain)
                {
                    ReplaySystem.StopRecordMain();
                    Debug.Log("Stop Record");
                }
                else
                {
                    ReplaySystem.StartRecordMain();
                    Debug.Log("Start Record");
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(m_PlayKey))
            {
                if (ReplaySystem.isRecordingMain)
                {
                    Debug.LogWarning("ReplaySystem is recording now. Stop and retry to play.");
                }
                else
                {
                    if (ReplaySystem.isPlayingMain)
                    {
                        ReplaySystem.StopPlayMain();
                        Debug.Log("Stop Play");
                    }
                    else
                    {
                        ReplaySystem.StartPlayMain(0f);
                        Debug.Log("Start Play");
                    }
                }
            }
        }
    }
}
