using UnityEngine;

using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class MicrophoneInput : Singleton<MicrophoneInput> 
{                                               
    private int m_NumSamples = 1024;               
    private float m_RefValue = 2e-5f;    
    private float m_AmplitudeThreshold = 0.02f;    

    private int m_MicChannel = 0;

    private float m_RmsValue;                      
    private float m_DbValue;                       
    private float m_PitchValue;                    

    private float[] m_Samples;                    
    private float[] m_Spectrum;                   

    private AudioSource m_AudioSource;

    // STATIC Getters

    public static float rmsValueMain
    {
        get
        {
            if (Instance != null)
            {
                return Instance.rmsValue;
            }

            return 0f;
        }
    }

    public static float dbValueMain
    {
        get
        {
            if (Instance != null)
            {
                return Instance.dbValue;
            }

            return 0f;
        }
    }

    public static float pitchValueMain
    {
        get
        {
            if (Instance != null)
            {
                return Instance.pitchValue;
            }

            return 0f;
        }
    }

    public static string[] devicesMain
    {
        get
        {
            if (Instance != null)
            {
                return Instance.devices;
            }

            return null;
        }
    }

    public static int devicesCountMain
    {
        get
        {
            if (Instance != null)
            {
                return Instance.devicesCount;
            }

            return 0;
        }
    }

    // STATIC Methods

    public static void InitMain()
    {
        if (Instance != null)
        {
            Instance.Init();
        }
    }

    public static void UninitMain()
    {
        if (Instance != null)
        {
            Instance.Uninit();
        }
    }

    public static bool IsRecordingMain(string i_DeviceName = "")
    {
        if (Instance != null)
        {
            return Instance.IsRecording(i_DeviceName);
        }

        return false;
    }

    public static void StartRecordingMain(string i_DeviceName = "")
    {
        if (Instance != null)
        {
            Instance.StartRecording(i_DeviceName);
        }
    }

    public static void StopRecordingMain(string i_DeviceName = "")
    {
        if (Instance != null)
        {
            Instance.StopRecording(i_DeviceName);
        }
    }

    // GETTERS

    public float rmsValue
    {
        get { return m_RmsValue; }
    }

    public float dbValue
    {
        get { return m_DbValue; }
    }

    public float pitchValue
    {
        get { return m_PitchValue; }
    }

    public string[] devices
    {
        get { return Microphone.devices; }
    }

    public int devicesCount
    {
        get
        {
            return Microphone.devices.Length;
        }
    }

    // LOGIC

    public void Init()
    {
        AudioMixerGroup mixerGroup = Resources.Load<AudioMixerGroup>("Audio/Mixers/Microphone");
        m_AudioSource.outputAudioMixerGroup = mixerGroup;   

        m_NumSamples = 1024;
        m_RefValue = 2e-5f;
        m_AmplitudeThreshold = 0.02f;

        m_MicChannel = 0;

        m_Samples = new float[m_NumSamples];
        m_Spectrum = new float[m_NumSamples];
    }

    public void Uninit()
    {

    }

    public bool IsRecording(string i_DeviceName = "")
    {
        return Microphone.IsRecording((i_DeviceName == "") ? null : i_DeviceName);
    }

    public void StartRecording(string i_DeviceName = "")
    {
        if (CheckMicrophone())
        {
            InternalStartRecording(i_DeviceName);
        }
    }

    public void StopRecording(string i_DeviceName = "")
    {
        InternalStopRecording(i_DeviceName);
    }

    // MonoBehaviour's interface

    void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();

        Init();
    }

    void Update()
    {
        if (m_AudioSource.isPlaying)
        {
            if (!CheckMicrophone())
            {
                InternalStopRecording();
            }
            else
            {
                m_AudioSource.GetOutputData(m_Samples, m_MicChannel);  // Fill array with samples

                float sum = 0f;

                for (int sampleIndex = 0; sampleIndex < m_Samples.Length; ++sampleIndex)
                {
                    sum += m_Samples[sampleIndex] * m_Samples[sampleIndex]; // Sum squared samples
                }

                m_RmsValue = Mathf.Sqrt(sum / m_NumSamples); // RMS - Square root of average

                if (rmsValue < Mathf.Epsilon)
                {
                    m_DbValue = -160f;
                }
                else
                {
                    m_DbValue = 20f * Mathf.Log10(rmsValue / m_RefValue); // Calculate dB
                    m_DbValue = Mathf.Max(dbValue, -160f);
                }

                // Analyze sound spectrum

                m_AudioSource.GetSpectrumData(m_Spectrum, m_MicChannel, FFTWindow.BlackmanHarris);

                float maxV = 0f;
                int maxN = 0;

                for (int spectrumIndex = 0; spectrumIndex < m_NumSamples; ++spectrumIndex) // Find max value
                {
                    if (m_Spectrum[spectrumIndex] > maxV && m_Spectrum[spectrumIndex] > m_AmplitudeThreshold)
                    {
                        maxV = m_Spectrum[spectrumIndex];
                        maxN = spectrumIndex;
                    }
                }

                float freqN = maxN;

                if (maxN > 0 && maxN < m_NumSamples - 1) // Index interpolation
                {
                    float dL = m_Spectrum[maxN - 1] / m_Spectrum[maxN];
                    float dR = m_Spectrum[maxN + 1] / m_Spectrum[maxN];
                    freqN += 0.5f * (dR * dR - dL * dL);
                }

                m_PitchValue = freqN * (AudioSettings.outputSampleRate / 2f) / m_NumSamples; // Convert index to frequency
            }
        }
        else
        {
            m_DbValue = -160f;
            m_RmsValue = 0f;
            m_PitchValue = 0f;
        }
    }

    // INTERNALS

    private void InternalStartRecording(string i_DeviceName = "")
    {
        m_AudioSource.clip = Microphone.Start((i_DeviceName == "") ? null : i_DeviceName, true, 10, 44100);
        m_AudioSource.loop = true;

        while (!(Microphone.GetPosition("AudioInputDevice") > 0))
        {
            // Do nothing.
        }

        m_AudioSource.Play();
    }

    private void InternalStopRecording(string i_DeviceName = "")
    {
        m_AudioSource.Stop();
        m_AudioSource.clip = null;

        Microphone.End((i_DeviceName == "") ? null : i_DeviceName);
    }

    private bool CheckMicrophone()
    {
        return (Microphone.devices.Length > 0);
    }
}
