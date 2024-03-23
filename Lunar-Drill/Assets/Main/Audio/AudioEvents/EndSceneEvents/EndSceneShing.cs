// Event that signals that the end screen "shing" sound should be played.

public class EndSceneShing : IAudioEvent
{
    public string Info; // String might be changed. Generic information for now, no usage. 

    public EndSceneShing(string info)
    {
            this.Info = info;
    }
}
