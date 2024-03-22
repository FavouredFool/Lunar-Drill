// This event signals that Drillian was hit by the Laser

public class DrillianHitLaser : IAudioEvent
{
    public string Info; // String might be changed. Generic information for now, no usage. 

    public DrillianHitLaser(string info)
    {
        this.Info= info;
    }
}
