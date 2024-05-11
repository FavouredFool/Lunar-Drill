using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderExampleEvents : MonoBehaviour
{
    public void TriggerSpiderHit()
    {
        AudioController.Fire(new SpiderHit($"Triggered by {this}"));
    }

    public void LaserStarting()
    {
        AudioController.Fire(new SpiderLaserFiring(SpiderLaserFiring.LaserState.LaserFiring));
    }
    public void LaserStopping()
    {
        AudioController.Fire(new SpiderLaserFiring(SpiderLaserFiring.LaserState.LaserStopped));
    }
    public void VulnurableStarting()
    {
        AudioController.Fire(new SpiderVulnurable(SpiderVulnurable.VulnurableState.SpiderVulnurable));
    }
    public void VulnurableStopping()
    {
        AudioController.Fire(new SpiderVulnurable(SpiderVulnurable.VulnurableState.SpiderInvulnurable));
    }
    public void ChargingStarting()
    {
        AudioController.Fire(new SpiderLaserCharging(SpiderLaserCharging.ChargeState.ChargingStarted));
    }
    public void ChargingStopping()
    {
        AudioController.Fire(new SpiderLaserCharging(SpiderLaserCharging.ChargeState.ChargingStopped));
    }
}
