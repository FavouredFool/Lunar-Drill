// Event that signals that Drillian hit the Spider BODY while it is invulnurable.

public class DrillianHitSpider : IAudioEvent
{
    public string Info; // String might be changed. Generic information for now, no usage. 

    public DrillianHitSpider(string info)
    {
            this.Info = info;
    }
}
