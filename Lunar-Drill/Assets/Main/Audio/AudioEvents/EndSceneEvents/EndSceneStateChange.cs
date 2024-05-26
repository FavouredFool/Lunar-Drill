// Event that signals that the end screen "shing" sound should be played.

public class EndSceneStateChange : IAudioEvent
{
    public enum State
    {
        EndScreenActive,
        EndScreenInactive
    }
    public State state;

    public EndSceneStateChange(State state)
    {
            this.state = state;
    }
}
