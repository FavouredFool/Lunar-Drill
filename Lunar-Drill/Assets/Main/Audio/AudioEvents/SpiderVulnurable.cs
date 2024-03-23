using System;
// Event that signals that Luna was hit by the laser (of the spider).

public class SpiderVulnurable : IAudioEvent
{
    public enum VulnurableState
    {
        SpiderVulnurable, // State when the Spider became Vulnurable.
        SpiderInvulnurable // When the laser became Spider became Invulnurable
    }

    public VulnurableState CurrentState;

    public SpiderVulnurable(VulnurableState state)
    {
        this.CurrentState = state;
    }
}
