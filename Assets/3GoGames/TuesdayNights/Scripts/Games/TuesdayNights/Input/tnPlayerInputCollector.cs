using UnityEngine;

using TrueSync;

public class tnPlayerInputCollector : TrueSyncBehaviour
{
    private tnPlayerSyncedInput m_PlayerSyncedInput = new tnPlayerSyncedInput();
    private tnInput m_Input = new tnInput();

    // MonoBehaviour's interface

    private void Awake()
    {
        sortOrder = 0; // This should not collide with nothing. It's just input collection.
    }

    // TrueSyncBehaviour's interface

    public override void OnSyncedInput()
    {
        base.OnSyncedInput();

        // Clear temp structure.

        m_Input.Clear();

        // Fill temp structure.

        m_PlayerSyncedInput.Step(m_Input);

        // Write TrueSyncInput.

        // FP

        for (int fpIndex = 0; fpIndex < m_Input.fpCount; ++fpIndex)
        {
            tnInput.FPInput fpInput;
            bool found = m_Input.GetFPInput(fpIndex, out fpInput);
            if (found)
            {
                TrueSyncInput.SetFP(fpInput.key, fpInput.value);
            }
        }

        // Int

        for (int intIndex = 0; intIndex < m_Input.intCount; ++intIndex)
        {
            tnInput.IntInput intInput;
            bool found = m_Input.GetIntInput(intIndex, out intInput);
            if (found)
            {
                TrueSyncInput.SetInt(intInput.key, intInput.value);
            }
        }

        // Bytes

        for (int byteIndex = 0; byteIndex < m_Input.byteCount; ++byteIndex)
        {
            tnInput.ByteInput byteInput;
            bool found = m_Input.GetByteInput(byteIndex, out byteInput);
            if (found)
            {
                TrueSyncInput.SetByte(byteInput.key, byteInput.value);
            }
        }
    }

    // LOGIC

    public void RegisterGameObject(GameObject i_Go, int i_Delay = 0)
    {
        m_PlayerSyncedInput.RegisterGameObject(i_Go, i_Delay);
    }
}