using UnityEngine;
using System.Collections;

public abstract class tnRunnableFSM : MonoBehaviour, tnIRunnableFSM
{
    public abstract event OnFsmReturn fsmReturnedEvent;
    public abstract void StartFSM();
}
