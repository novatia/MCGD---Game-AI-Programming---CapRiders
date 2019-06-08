public interface IReplayable
{
    // Record

    void StartRecord();
    void StopRecord();

    void UpdateRecord(float i_DeltaTime);

    // Play

    void StartPlay(float i_StartTime);
    void StopPlay();

    void UpdatePlay(float i_LastPlayedTime, float i_PlayTime);
}
