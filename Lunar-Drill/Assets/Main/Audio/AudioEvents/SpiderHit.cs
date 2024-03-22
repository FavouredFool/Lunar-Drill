// Event that signals that te Spider was hit.

public class SpiderHit : IAudioEvent
{
    public string Info; // String might be changed. Generic information for now, no usage. 

    public SpiderHit(string info)
    {
        this.Info = info;
    }
}
