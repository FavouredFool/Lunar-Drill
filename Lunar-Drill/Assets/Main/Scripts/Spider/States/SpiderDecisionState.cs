using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SpiderManager.SpiderAbilityState;



public class SpiderDecisionState : SpiderState
{
    public SpiderDecisionState(SpiderManager spiderManager, Stack<SpiderState> stateStack = null, bool doEntryAttackChain = false) : base(spiderManager)
    {
        _doEntryAttackChain = doEntryAttackChain;
        _stateStack = stateStack;
    }

    bool _doEntryAttackChain;
    Stack<SpiderState> _stateStack;
    
    public override void StartState()
    {
        Debug.Log("DecisionState");

        SpiderState nextState;
        
        if (_stateStack is { Count: > 0 })
        {
            nextState = _stateStack.Pop();
        }
        else
        {
            nextState = GetNextAbility(GetSpiderState(_gameManager), _doEntryAttackChain);
        }

        _spiderManager.SpiderStateManager.SetState(nextState);
    }
    
    SpiderManager.SpiderAbilityState GetSpiderState(GameManager gameManager)
    {
        SpiderManager.SpiderAbilityState spiderState;

        int spiderHP = gameManager.SpiderHP;

        if (spiderHP == gameManager.SpiderMaxHP)
        {
            spiderState = Level3;
            //spiderState = Level1;
        }
        else if (spiderHP == gameManager.SpiderMaxHP - 1)
        {
            spiderState = Level3;
            //spiderState = Level2;
        }
        else if (spiderHP == gameManager.SpiderMaxHP - 2)
        {
            spiderState = Level3;
        }
        else
        {
            spiderState = Level4;
        }

        return spiderState;
    }

    SpiderState GetNextAbility(SpiderManager.SpiderAbilityState spiderState, bool doEntryAttackChain)
    {
        switch (spiderState)
        {
            case Level1:
                return GetLevel1Ability(doEntryAttackChain);
            case Level2:
                return GetLevel2Ability(doEntryAttackChain);
            case Level3:
                return GetLevel3Ability(doEntryAttackChain);
            case Level4:
                return GetLevel4Ability(doEntryAttackChain);
            default:
                return new SpiderVoidState(_spiderManager);
        }
    }
    
    SpiderState GetLevel1Ability(bool doEntryAttackChain)
    {
        // Stand: 30%
        // Movement: 70%
        
        if (doEntryAttackChain)
        {
            Stack<SpiderState> attackStack = new();
            attackStack.Push(new SpiderMovementState(_spiderManager, attackStack));
            attackStack.Push(new SpiderWaitState(_spiderManager, attackStack));
            attackStack.Push(new SpiderMovementState(_spiderManager, attackStack));
            attackStack.Push(new SpiderWaitState(_spiderManager, attackStack, 0.5f));
            
            return attackStack.Pop();
        }
        
        float randomT = Random.Range(0f, 1f);
        
        if (randomT > 0.7f)
        {
            return new SpiderWaitState(_spiderManager);
        }
        else
        {
            return new SpiderMovementState(_spiderManager);
        }
    }

    SpiderState GetLevel2Ability(bool doEntryAttackChain)
    {
        // Stand: 20% Chance
        // Movement: 40% Chance
        // RandomLaserShort: 40% Chance

        if (doEntryAttackChain)
        {
            Stack<SpiderState> attackStack = new();
            attackStack.Push(new SpiderWaitState(_spiderManager, attackStack, 1.5f));
            attackStack.Push(new SpiderLaserShortState(_spiderManager, attackStack));
            attackStack.Push(new SpiderMovementState(_spiderManager, attackStack, 150, 170));
            
            return attackStack.Pop();
        }
        
        float randomT = Random.Range(0f, 1f);

        if (randomT > 0.8f)
        {
            return new SpiderWaitState(_spiderManager);
        }
        else if (randomT > 0.4f)
        {
            return new SpiderMovementState(_spiderManager);
        }
        else
        {
            return new SpiderLaserShortState(_spiderManager);
        }
    }

    SpiderState GetLevel3Ability(bool doEntryAttackChain)
    {
        // Wait: 15% Chance
        // Movement: 35% Chance
        // RandomLaserShort: 20% Chance
        // Digging: 30%

        if (doEntryAttackChain)
        {
            Stack<SpiderState> attackStack = new();
            attackStack.Push(new SpiderMovementState(_spiderManager, attackStack));
            attackStack.Push(new SpiderLaserShortState(_spiderManager, attackStack));
            attackStack.Push(new SpiderWaitState(_spiderManager, attackStack, 1.5f));
            attackStack.Push(new SpiderDiggingState(_spiderManager, attackStack));
            attackStack.Push(new SpiderMovementState(_spiderManager, attackStack, 150, 170));
            
            return attackStack.Pop();
        }
        
        float randomT = Random.Range(0f, 1f);

        if (randomT > 0.85f)
        {
            return new SpiderWaitState(_spiderManager);
        }
        else if (randomT > 0.5f)
        {
            return new SpiderMovementState(_spiderManager);
        }
        else if (randomT > 0.3f)
        {
            return new SpiderLaserShortState(_spiderManager);
        }
        else
        {
            return new SpiderDiggingState(_spiderManager);
        }
    }


    SpiderState GetLevel4Ability(bool doEntryAttackChain)
    {
        // Stand: 10% Chance
        // Movement: 30% Chance
        // RandomLaserShort: 25% Chance
        // Digging: 25% Chance
        // RandomLaserLong: 10%

        if (doEntryAttackChain)
        {
            Stack<SpiderState> attackStack = new();
            attackStack.Push(new SpiderMovementState(_spiderManager, attackStack));
            attackStack.Push(new SpiderWaitState(_spiderManager, attackStack, 2.5f));
            attackStack.Push(new SpiderLaserLongState(_spiderManager, attackStack));
            attackStack.Push(new SpiderDiggingState(_spiderManager, attackStack));
            attackStack.Push(new SpiderMovementState(_spiderManager, attackStack, 10, 35));
            attackStack.Push(new SpiderDiggingState(_spiderManager, attackStack));
            attackStack.Push(new SpiderMovementState(_spiderManager, attackStack, 10, 35));
            attackStack.Push(new SpiderDiggingState(_spiderManager, attackStack));
            attackStack.Push(new SpiderMovementState(_spiderManager, attackStack, 150, 170));
            
            return attackStack.Pop();
        }
        
        float randomT = Random.Range(0f, 1f);

        if (randomT > 0.5f)
        {
            return new SpiderWaitState(_spiderManager);
        }
        else if (randomT > 0.65f)
        {
            return new SpiderMovementState(_spiderManager);
        }
        else if (randomT > 0.4f)
        {
            return new SpiderLaserShortState(_spiderManager);
        }
        else if (randomT > 0.1f)
        {
            return new SpiderDiggingState(_spiderManager);
        }
        else
        {
            return new SpiderLaserLongState(_spiderManager);
        }
    }
    
}
