// Event that signals that Luna has picked up energy

public class LunaEnergyPickup : IAudioEvent
{
    public string Info; // String might be changed. Generic information for now, no usage. 

    public LunaEnergyPickup(string info)
    {
        this.Info = info;
    }
}
