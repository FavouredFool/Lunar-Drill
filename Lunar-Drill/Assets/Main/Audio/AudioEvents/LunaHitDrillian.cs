//Event that signals that Luna was hit by Drillian

public class LunaHitDrillian : IAudioEvent
{
    public string Info; // String might be changed. Generic information for now, no usage. 

    public LunaHitDrillian(string info)
    {
        this.Info = info;
    }
}
