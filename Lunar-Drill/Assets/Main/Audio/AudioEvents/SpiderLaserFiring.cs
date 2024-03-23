using System;
// Event that signals that the laser (of the spider) changed its state.

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
