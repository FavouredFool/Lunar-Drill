// Event that signals that the end screen "Over" sound should be played.

public class EndSceneOver : IAudioEvent
{
    public string Info; // String might be changed. Generic information for now, no usage. 

    public EndSceneOver(string info)
    {
            this.Info = info;
    }
}
