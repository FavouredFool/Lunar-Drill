// Event that signals that the end screen "Game" sound should be played.

public class EndSceneGame : IAudioEvent
{
    public string Info; // String might be changed. Generic information for now, no usage. 

    public EndSceneGame(string info)
    {
            this.Info = info;
    }
}
