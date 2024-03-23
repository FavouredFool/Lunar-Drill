using System;
// Event that signals that the laser (of the spider) changed its state.

public class SpiderLaserCharging : IAudioEvent
{
    public enum ChargeState
    {
        ChargingStarted, // State when the Laser is charging
        ChargingStopped // When the laser charge is being stopped
    }

    public ChargeState CurrentState;

    public SpiderLaserCharging(ChargeState state)
    {
        this.CurrentState = state;
    }
}
