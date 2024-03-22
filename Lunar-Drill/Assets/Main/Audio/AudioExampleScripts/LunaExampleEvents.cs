using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LunaExampleEvents : MonoBehaviour
{
    public void TriggerLunaLaserHit()
    {
        AudioController.Fire(new LunaHitLaser($"Triggered by {this}"));
    }
    public void TriggerLunaDrillianHit()
    {
        AudioController.Fire(new LunaHitDrillian($"Triggered by {this}"));
    }
}
