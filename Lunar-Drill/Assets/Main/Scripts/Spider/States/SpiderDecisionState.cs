using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SpiderManager.SpiderAbilityState;



public class SpiderDecisionState : SpiderState
{
    public SpiderDecisionState(SpiderManager spiderManager) : base (spiderManager) { }
    
    public override void StartState()
    {
        Debug.Log("DecisionState");
        GetNextAbility(GetSpiderState(_gameManager));
    }
    
    SpiderManager.SpiderAbilityState GetSpiderState(GameManager gameManager)
    {
        SpiderManager.SpiderAbilityState spiderState;

        int spiderHP = gameManager.SpiderHP;

        if (spiderHP == gameManager.SpiderMaxHP)
        {
            spiderState = Level1;
        }
        else if (spiderHP == gameManager.SpiderMaxHP - 1)
        {
            spiderState = Level2;
        }
        else if (spiderHP == gameManager.SpiderHP - 2)
        {
            spiderState = Level3;
        }
        else
        {
            spiderState = Level4;
        }

        return spiderState;
    }

    void GetNextAbility(SpiderManager.SpiderAbilityState spiderState)
    {
        switch (spiderState)
        {
            case Level1:
                GetLevel1Ability();
                break;
            case Level2:
                GetLevel2Ability();
                break;
            case Level3:
                GetLevel3Ability();
                break;
            case Level4:
                GetLevel4Ability();
                break;
        }
    }
    
    void GetLevel1Ability()
    {
        // Stand: 20%
        // Movement: 80%
        
        float randomT = Random.Range(0f, 1f);
        
        if (randomT > 0.5f)
        {
            _spiderManager.SpiderStateManager.SetState(new SpiderDiggingState(_spiderManager));
        }
        else if (randomT > 0.8f)
        {
            _spiderManager.SpiderStateManager.SetState(new SpiderWaitState(_spiderManager));
        }
        else
        {
            _spiderManager.SpiderStateManager.SetState(new SpiderMovementState(_spiderManager));
        }
    }

    void GetLevel2Ability()
    {
        // Stand: 20% Chance
        // Movement: 40% Chance
        // RandomLaserShort: 30% Chance
        // LunaLaser: 10% Chance

        float randomT = Random.Range(0f, 1f);

        if (randomT > 0.8f)
        {
            _spiderManager.SpiderStateManager.SetState(new SpiderWaitState(_spiderManager));
        }
        else if (randomT > 0.4f)
        {
            _spiderManager.SpiderStateManager.SetState(new SpiderMovementState(_spiderManager));
        }
        else
        {
            _spiderManager.SpiderStateManager.SetState(new SpiderLaserShortState(_spiderManager));
        }
    }

    void GetLevel3Ability()
    {
        // Wait: 10% Chance
        // Movement: 40% Chance
        // RandomLaserShort: 25% Chance
        // RandomLaserLong: 0% Chance
        // Digging: 25%
        // LunaLaser: 0% Chance

        float randomT = Random.Range(0f, 1f);

        if (randomT > 0.9f)
        {
            _spiderManager.SpiderStateManager.SetState(new SpiderWaitState(_spiderManager));
        }
        else if (randomT > 0.5f)
        {
            _spiderManager.SpiderStateManager.SetState(new SpiderMovementState(_spiderManager));
        }
        else if (randomT > 0.25f)
        {
            _spiderManager.SpiderStateManager.SetState(new SpiderLaserShortState(_spiderManager));
        }
        else
        {
            _spiderManager.SpiderStateManager.SetState(new SpiderDiggingState(_spiderManager));
        }
    }


    void GetLevel4Ability()
    {
        // Stand: 05% Chance
        // Movement: 25% Chance
        // RandomLaserShort: 35% Chance
        // RandomLaserLong: 0% Chance
        // LunaLaser: 0% Chance
        // Digging: 35% Chance

        float randomT = Random.Range(0f, 1f);

        if (randomT > 0.95f)
        {
            _spiderManager.SpiderStateManager.SetState(new SpiderWaitState(_spiderManager));
        }
        else if (randomT > 0.7f)
        {
            _spiderManager.SpiderStateManager.SetState(new SpiderMovementState(_spiderManager));
        }
        else if (randomT > 0.35f)
        {
            _spiderManager.SpiderStateManager.SetState(new SpiderLaserShortState(_spiderManager));
        }
        else
        {
            _spiderManager.SpiderStateManager.SetState(new SpiderDiggingState(_spiderManager));
        }
    }
    
}
