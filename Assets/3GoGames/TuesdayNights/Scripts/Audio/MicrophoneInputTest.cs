using UnityEngine;

public class MicrophoneInputTest : MonoBehaviour
{
    public bool printDBValue = false;
    public bool printRMSValue = false;
    public bool printPitchValue = false;

    void Awake()
    {
        MicrophoneInput.InitMain();
    }

    void Start()
    {
        string[] devices = MicrophoneInput.devicesMain;
        if (devices != null)
        {
            Debug.Log(devices.Length);
            for (int i = 0; i < devices.Length; ++i)
            {
                Debug.Log(devices[i]);
            }
        }
    }

    void Update()
    {
        Debug.Log("------------------//////////---------------");

        if (MicrophoneInput.IsRecordingMain())
        {
            if (printDBValue)
            {
                Debug.Log("DB  Value: " + MicrophoneInput.dbValueMain);
            }

            if (printRMSValue)
            {
                Debug.Log("RMS Value: " + MicrophoneInput.rmsValueMain);
            }

            if (printPitchValue)
            {
                Debug.Log("Pitch Value: " + MicrophoneInput.pitchValueMain);
            }


            if (Input.GetKeyDown(KeyCode.T))
            {
                Debug.Log("STOP");
                MicrophoneInput.StopRecordingMain();
            }
        }
        else
        {
            Debug.Log("Microphone is not recording.");

            if (Input.GetKeyDown(KeyCode.T))
            {
                Debug.Log("START");
                MicrophoneInput.StartRecordingMain();
            }
        }

        Debug.Log("---------------------------------------");
    }
}
