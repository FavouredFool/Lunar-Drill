using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class LunaExampleEvents : MonoBehaviour
{
    public float _energy = 1f;
    [SerializeField] float energy_decay = 1;

    public void TriggerLunaLaserHit()
    {
        AudioController.Fire(new LunaHitLaser($"Triggered by {this}"));
    }
    public void TriggerLunaDrillianHit()
    {
        AudioController.Fire(new LunaHitDrillian($"Triggered by {this}"));
    }
    public void TriggerLunaEnergyPickup()
    {
        AudioController.Fire(new LunaEnergyPickup($"Triggered by {this}"));
    }

    public void TriggerLunaShoot(bool isActive, float energy)
    {
        LunaLaserFiring.LaserState state = isActive ? LunaLaserFiring.LaserState.LaserFiring : LunaLaserFiring.LaserState.LaserStopped;
        AudioController.Fire(new LunaLaserFiring(state,energy));
    }

    public void ChangeEnergy(float energy)
    {
        _energy = energy;
    }

    public void LaserStarting()
    {
        StartCoroutine(FireLaserFull());

    }
    public void LaserStopping()
    {
        AudioController.Fire(new LunaLaserFiring(LunaLaserFiring.LaserState.LaserStopped, _energy));
    }

    IEnumerator FireLaserFull()
    {
        while(_energy >=0)
        {
            AudioController.Fire(new LunaLaserFiring(LunaLaserFiring.LaserState.LaserFiring, _energy));
            yield return new WaitForEndOfFrame();
            _energy -=  Time.deltaTime/ energy_decay;
        }
        _energy = 0;
        yield return new WaitForSeconds(1);
        _energy = 1;
        AudioController.Fire(new LunaLaserFiring(LunaLaserFiring.LaserState.LaserStopped, _energy));
        
    }
}
