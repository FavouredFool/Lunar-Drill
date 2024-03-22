using System;
// Event that signals that Luna was hit by the laser (of the spider).

public class LunaLaserFiring : IAudioEvent
{
    public enum LaserState
    {
        LaserFiring, // State when the Laser is playing
        LaserStopped // When the laser is being stopped
    }

    public LaserState CurrentState;
    public float LaserEnergyRemaining; // Should be a float in between 0 and 1 where 1 is full energy and 0 is no Energy

    public LunaLaserFiring(LaserState state, float LaserEnergyRemaining )
    {
        if( LaserEnergyRemaining < 0 || LaserEnergyRemaining>1) 
        {
            throw (new ArgumentException("LaserEnergyRemaining should be a float value between 1 (full) and 0 (no Energy)"));
        }
        this.LaserEnergyRemaining = LaserEnergyRemaining;
        this.CurrentState = state;
    }
}
