using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreenEvents : MonoBehaviour
{
    public void TriggerDrillianSpiderHit()
    {
        AudioController.Fire(new DrillianHitSpider("Caused by Button"));
    }
}
