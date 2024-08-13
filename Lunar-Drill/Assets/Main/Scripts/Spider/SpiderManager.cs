using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderManager : MonoBehaviour
{
    [SerializeField] SpiderController _spiderController;
    [SerializeField] GameManager _gameManager;
    
    public SpiderStateManager SpiderStateManager { get; private set; }
    public SpiderController SpiderController => _spiderController;
    public GameManager GameManager => _gameManager;
    
    public enum SpiderAbilityState { Level1, Level2, Level3, Level4 };
    
    void Awake()
    {
        SpiderStateManager = new SpiderStateManager();
    }

    void Start()
    {
        SpiderStateManager.SetState(new SpiderDecisionState(this));
    }

    void Update()
    {
        SpiderStateManager.UpdateState();
    }

    void FixedUpdate()
    {
        SpiderStateManager.FixedUpdateState();
    }
}
