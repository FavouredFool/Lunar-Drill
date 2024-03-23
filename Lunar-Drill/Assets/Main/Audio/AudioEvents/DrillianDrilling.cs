using System;
// Event that signals that the Drill state of Drillian changed.

public class DrillianDrilling : IAudioEvent
{
    public enum DrillState
    {
        DrillingStarted, // State when the Drill is started
        DrillingStopped // When the Drill is stopped
    }

    public DrillState CurrentState;

    public DrillianDrilling(DrillState state)
    {
        this.CurrentState = state;
    }
}
