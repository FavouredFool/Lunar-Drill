// Event that signals that the end screen "Drill" sound should be played.

public class EndSceneDrill : IAudioEvent
{
    public string Info; // String might be changed. Generic information for now, no usage. 

    public EndSceneDrill(string info)
    {
            this.Info = info;
    }
}
