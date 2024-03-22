using System;
// Event that signals that Luna was hit by the laser (of the spider).

public class SpiderLaserFiring : IAudioEvent
{
    public enum LaserState
    {
        LaserFiring, // State when the Laser is playing
        LaserStopped // When the laser is being stopped
    }

    public LaserState CurrentState;

    public SpiderLaserFiring(LaserState state)
    {
        this.CurrentState = state;
    }
}
