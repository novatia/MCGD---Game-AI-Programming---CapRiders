using UnityEngine;
using System.Collections.Generic;

using TrueSync;

public class tnInput
{
    // Types

    public struct FPInput
    {
        // Fields

        private byte m_Key;
        private FP m_Value;

        // ACCESSORS

        public byte key
        {
            get { return m_Key; }
        }

        public FP value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        // CTOR

        public FPInput(byte i_Key, FP i_Value)
        {
            m_Key = i_Key;
            m_Value = i_Value;
        }

        public FPInput(FPInput i_Original)
        {
            m_Key = i_Original.key;
            m_Value = i_Original.value;
        }
    }

    public struct IntInput
    {
        // Fields

        private byte m_Key;
        private int m_Value;

        // ACCESSORS

        public byte key
        {
            get { return m_Key; }
        }

        public int value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        // CTOR

        public IntInput(byte i_Key, int i_Value)
        {
            m_Key = i_Key;
            m_Value = i_Value;
        }

        public IntInput(IntInput i_Original)
        {
            m_Key = i_Original.key;
            m_Value = i_Original.value;
        }
    }

    public struct ByteInput
    {
        // Fields

        private byte m_Key;
        private byte m_Value;

        // ACCESSORS

        public byte key
        {
            get { return m_Key; }
        }

        public byte value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        // CTOR

        public ByteInput(byte i_Key, byte i_Value)
        {
            m_Key = i_Key;
            m_Value = i_Value;
        }

        public ByteInput(ByteInput i_Original)
        {
            m_Key = i_Original.key;
            m_Value = i_Original.value;
        }
    }

    // Fields

    private List<FPInput> m_FPs = null;
    private List<IntInput> m_Ints = null;
    private List<ByteInput> m_Bytes = null;

    // ACCESSORS

    public int count
    {
        get
        {
            return m_FPs.Count + m_Bytes.Count + m_Ints.Count;
        }
    }

    public int fpCount
    {
        get
        {
            return m_FPs.Count;
        }
    }

    public int intCount
    {
        get
        {
            return m_Ints.Count;
        }
    }

    public int byteCount
    {
        get
        {
            return m_Bytes.Count;
        }
    }

    // LOGIC

    public void AddRange(tnInput i_Input)
    {
        // FP

        for (int index = 0; index < i_Input.fpCount; ++index)
        {
            FPInput fpInput;
            bool found = i_Input.GetFPInput(index, out fpInput);
            if (found)
            {
                m_FPs.Add(fpInput);
            }
        }

        // Int

        for (int index = 0; index < i_Input.intCount; ++index)
        {
            IntInput intInput;
            bool found = i_Input.GetIntInput(index, out intInput);
            if (found)
            {
                m_Ints.Add(intInput);
            }
        }

        // Byte

        for (int index = 0; index < i_Input.byteCount; ++index)
        {
            ByteInput byteInput;
            bool found = i_Input.GetByteInput(index, out byteInput);
            if (found)
            {
                m_Bytes.Add(byteInput);
            }
        }
    }

    public void Clear()
    {
        m_FPs.Clear();
        m_Ints.Clear();
        m_Bytes.Clear();
    }

    public void SetFP(byte i_Key, FP i_Value)
    {
        RemoveFP(i_Key);

        FPInput fpInput = new FPInput(i_Key, i_Value);
        m_FPs.Add(fpInput);
    }

    public void SetInt(byte i_Key, int i_Value)
    {
        RemoveInt(i_Key);

        IntInput intInput = new IntInput(i_Key, i_Value);
        m_Ints.Add(intInput);
    }

    public void SetByte(byte i_Key, byte i_Value)
    {
        RemoveByte(i_Key);

        ByteInput byteInput = new ByteInput(i_Key, i_Value);
        m_Bytes.Add(byteInput);
    }

    public bool HasFP(byte i_Key)
    {
        for (int index = 0; index < m_FPs.Count; ++index)
        {
            FPInput fpInput = m_FPs[index];
            if (fpInput.key == i_Key)
            {
                return true;
            }
        }

        return false;
    }

    public bool HasInt(byte i_Key)
    {
        for (int index = 0; index < m_Ints.Count; ++index)
        {
            IntInput intInput = m_Ints[index];
            if (intInput.key == i_Key)
            {
                return true;
            }
        }

        return false;
    }

    public bool HasByte(byte i_Key)
    {
        for (int index = 0; index < m_Bytes.Count; ++index)
        {
            ByteInput byteInput = m_Bytes[index];
            if (byteInput.key == i_Key)
            {
                return true;
            }
        }

        return false;
    }

    public FP GetFP(byte i_Key)
    {
        int index = GetFPInputIndex(i_Key);
        if (index >= 0)
        {
            FPInput fpInput = m_FPs[index];
            return fpInput.value;
        }

        return FP.Zero;
    }

    public int GetInt(byte i_Key)
    {
        int index = GetIntInputIndex(i_Key);
        if (index >= 0)
        {
            IntInput intInput = m_Ints[index];
            return intInput.value;
        }

        return 0;
    }

    public byte GetByte(byte i_Key)
    {
        int index = GetByteInputIndex(i_Key);
        if (index >= 0)
        {
            ByteInput byteInput = m_Bytes[index];
            return byteInput.value;
        }

        return (byte)0;
    }

    public bool GetFPInput(int i_Index, out FPInput o_FPInput)
    {
        if (!IsValidFPIndex(i_Index))
        {
            o_FPInput = new FPInput(0, FP.Zero);
            return false;
        }

        FPInput fpInput = m_FPs[i_Index];
        o_FPInput = new FPInput(fpInput);
        return true;
    }

    public bool GetIntInput(int i_Index, out IntInput o_IntInput)
    {
        if (!IsValidIntIndex(i_Index))
        {
            o_IntInput = new IntInput(0, 0);
            return false;
        }

        IntInput intInput = m_Ints[i_Index];
        o_IntInput = new IntInput(intInput);
        return true;
    }

    public bool GetByteInput(int i_Index, out ByteInput o_ByteInput)
    {
        if (!IsValidByteIndex(i_Index))
        {
            o_ByteInput = new ByteInput(0, 0);
            return false;
        }

        ByteInput byteInput = m_Bytes[i_Index];
        o_ByteInput = new ByteInput(byteInput);
        return true;
    }

    // INTERNALS

    private void RemoveFP(byte i_Key)
    {
        int index = GetFPInputIndex(i_Key);
        RemoveFPByIndex(index);
    }

    private void RemoveInt(byte i_Key)
    {
        int index = GetIntInputIndex(i_Key);
        RemoveIntByIndex(index);
    }

    private void RemoveByte(byte i_Key)
    {
        int index = GetByteInputIndex(i_Key);
        RemoveByteByIndex(index);
    }

    private void RemoveFPByIndex(int i_Index)
    {
        if (!IsValidFPIndex(i_Index))
            return;

        m_FPs.RemoveAt(i_Index);
    }

    private void RemoveIntByIndex(int i_Index)
    {
        if (!IsValidIntIndex(i_Index))
            return;

        m_Ints.RemoveAt(i_Index);
    }

    private void RemoveByteByIndex(int i_Index)
    {
        if (!IsValidByteIndex(i_Index))
            return;

        m_Bytes.RemoveAt(i_Index);
    }

    private int GetFPInputIndex(byte i_Key)
    {
        for (int index = 0; index < m_FPs.Count; ++index)
        {
            FPInput fpInput = m_FPs[index];
            if (fpInput.key == i_Key)
            {
                return index;
            }
        }

        return -1;
    }

    private int GetIntInputIndex(byte i_Key)
    {
        for (int index = 0; index < m_Ints.Count; ++index)
        {
            IntInput intInput = m_Ints[index];
            if (intInput.key == i_Key)
            {
                return index;
            }
        }

        return -1;
    }

    private int GetByteInputIndex(byte i_Key)
    {
        for (int index = 0; index < m_Bytes.Count; ++index)
        {
            ByteInput byteInput = m_Bytes[index];
            if (byteInput.key == i_Key)
            {
                return index;
            }
        }

        return -1;
    }

    // UTILS

    private bool IsValidFPIndex(int i_Index)
    {
        if (i_Index < 0 || i_Index >= fpCount)
        {
            return false;
        }

        return true;
    }

    private bool IsValidIntIndex(int i_Index)
    {
        if (i_Index < 0 || i_Index >= intCount)
        {
            return false;
        }

        return true;
    }

    private bool IsValidByteIndex(int i_Index)
    {
        if (i_Index < 0 || i_Index >= byteCount)
        {
            return false;
        }

        return true;
    }

    // CTOR

    public tnInput()
    {
        m_FPs = new List<FPInput>();
        m_Ints = new List<IntInput>();
        m_Bytes = new List<ByteInput>();
    }
}

public interface tnISyncablePlayerInput
{
    void SyncedInput(tnInput o_Input);
}

public class tnPlayerSyncedInput
{
    // Fields

    private DictionaryList<GameObject, tnInput[]> m_RegisteredGameObjects = null;

    // LOGIC

    public void RegisterGameObject(GameObject i_Go, int i_Delay = 0)
    {
        if (i_Go == null)
            return;

        int delay = Mathf.Max(0, i_Delay);
        tnInput[] inputs = new tnInput[delay + 1];
        for (int index = 0; index < inputs.Length; ++index)
        {
            inputs[index] = new tnInput();
        }

        m_RegisteredGameObjects.Add(i_Go, inputs);
    }

    public void Step(tnInput o_Input)
    {
        for (int goIndex = 0; goIndex < m_RegisteredGameObjects.count; ++goIndex)
        {
            tnInput[] inputs = m_RegisteredGameObjects.GetItem(goIndex);

            tnInput first = inputs[0];
            first.Clear();

            for (int index = 1; index < inputs.Length; ++index)
            {
                inputs[index - 1] = inputs[index];
            }

            inputs[inputs.Length - 1] = first;
        }

        for (int goIndex = 0; goIndex < m_RegisteredGameObjects.count; ++goIndex)
        {
            GameObject go = m_RegisteredGameObjects.GetKey(goIndex);

            if (go == null)
                continue;

            tnInput[] inputs = m_RegisteredGameObjects.GetValue(go);

            if (inputs == null)
                continue;

            tnInput input = inputs[inputs.Length - 1];

            tnISyncablePlayerInput[] components = go.GetComponents<tnISyncablePlayerInput>();
            for (int componentIndex = 0; componentIndex < components.Length; ++componentIndex)
            {
                tnISyncablePlayerInput component = components[componentIndex];
                component.SyncedInput(input);
            }
        }

        FillNextInput(o_Input);
    }

    // INTERNALS

    private void FillNextInput(tnInput o_Input)
    {
        for (int goIndex = 0; goIndex < m_RegisteredGameObjects.count; ++goIndex)
        {
            tnInput[] inputs = m_RegisteredGameObjects.GetItem(goIndex);
            tnInput first = inputs[0];

            o_Input.AddRange(first);
        }
    }

    // CTOR

    public tnPlayerSyncedInput()
    {
        m_RegisteredGameObjects = new DictionaryList<GameObject, tnInput[]>();
    }
}