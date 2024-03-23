using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreenEvents : MonoBehaviour
{
    public void TriggerGame()
    {
        AudioController.Fire(new EndSceneGame("Caused by Button"));
    }
    public void TriggerOver()
    {
        AudioController.Fire(new EndSceneOver("Caused by Button"));
    }
    public void TriggerLunar()
    {
        AudioController.Fire(new EndSceneLunar("Caused by Button"));
    }
    public void TriggerDrill()
    {
        AudioController.Fire(new EndSceneDrill("Caused by Button"));
    }
    public void TriggerSlish()
    {
        AudioController.Fire(new EndSceneShing("Caused by Button"));
    }
}
