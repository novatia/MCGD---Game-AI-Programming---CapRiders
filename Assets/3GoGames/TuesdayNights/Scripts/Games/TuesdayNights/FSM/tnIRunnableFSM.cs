public delegate void OnFsmReturn();

public interface tnIRunnableFSM
{
    event OnFsmReturn fsmReturnedEvent;
    void StartFSM();
}
