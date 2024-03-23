// Event that signals that the end screen "Luna" sound should be played.

public class EndSceneLunar : IAudioEvent
{
    public string Info; // String might be changed. Generic information for now, no usage. 

    public EndSceneLunar(string info)
    {
            this.Info = info;
    }
}
