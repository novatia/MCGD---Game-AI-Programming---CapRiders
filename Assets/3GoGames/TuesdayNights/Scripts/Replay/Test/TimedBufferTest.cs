using UnityEngine;

public class TimedBufferTest : MonoBehaviour
{
    [SerializeField]
    [DisallowEditInPlayMode]
    private int m_Size = 15;

    [SerializeField]
    private KeyCode m_PushKey = KeyCode.L;
    [SerializeField]
    private KeyCode m_GetKey = KeyCode.K;

    [SerializeField]
    private float m_TimeToSearch = 4000f;

    private TimedBuffer<int> m_Buffer = null;

    private float m_Timer = 0f;

    void Awake()
    {
        m_Buffer = new TimedBuffer<int>(m_Size);
    }

    void Start()
    {
        m_Timer = 0f;
        m_Buffer.Push(0f, Random.Range(0, 101));
    }

    void Update()
    {
        m_Timer += Time.deltaTime;

        if (Input.GetKeyDown(m_PushKey))
        {
            m_Buffer.Push(m_Timer, Random.Range(0, 101));
            Debug.Log("Push at t: " + m_Timer);
        }
        else
        {
            if (Input.GetKeyDown(m_GetKey))
            {
                int index;
                float t;
                if (m_Buffer.TryGetIndex(m_TimeToSearch, out index, out t))
                {
                    Debug.Log("Data found: " + index + ", " + t);
                }
            }
        }
    }
}
