// Event that signals that Luna was hit by the laser (of the spider).

public class LunaHitLaser : IAudioEvent
{
    public string Info; // String might be changed. Generic information for now, no usage. 

    public LunaHitLaser(string info)
    {
        this.Info = info;
    }
}
