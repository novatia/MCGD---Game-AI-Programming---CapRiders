using UnityEngine;
using System.Collections;

public class tnScreenShakeTest : MonoBehaviour
{
    private tnScreenShake m_Shake = null;

	void Start()
    {
        m_Shake = FindObjectOfType<tnScreenShake>();
	}
	
	void Update()
    {
        if (m_Shake == null)
            return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            m_Shake.ForceShake(1f, 0.2f, ShakeMode.Interruput, OnEnd1);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            m_Shake.ForceShake(2f, 0.1f, ShakeMode.DontInterrupt, OnEnd2);
        }
    }

    private void OnEnd1()
    {
        Debug.Log("Screen shake ended [1]");
    }

    private void OnEnd2()
    {
        Debug.Log("Screen shake ended [2]");
    }
}
