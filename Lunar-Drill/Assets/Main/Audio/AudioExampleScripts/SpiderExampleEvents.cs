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
}
