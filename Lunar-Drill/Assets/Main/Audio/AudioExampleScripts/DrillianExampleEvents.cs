using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Jsut a gathering of how the events should be used
public class DrillianEvents : MonoBehaviour
{
    int pitch = 0;
    public void OreCrackedAudioEvent()
    {
        AudioController.Fire(new OreCrackedAudioEvent(pitch));
        pitch++;
        pitch %= 6;
    }

    public void TriggerDrillianLaserHit()
    {
        AudioController.Fire(new DrillianHitLaser("Caused by Button"));
    }

    public void TriggerDrillianSpiderHit()
    {
        AudioController.Fire(new DrillianHitSpider("Caused by Button"));
    }

    public void DrillingStarting()
    {
        AudioController.Fire(new DrillianDrilling(DrillianDrilling.DrillState.DrillingStarted));
    }
    public void DrillingStopping()
    {
        AudioController.Fire(new DrillianDrilling(DrillianDrilling.DrillState.DrillingStopped));
    }

    public void DrillianModeChange()
    {
        AudioController.Fire(new DrillianChangeMode(""));
    }
}
