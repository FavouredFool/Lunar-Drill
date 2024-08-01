using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentAnchor : MonoBehaviour
{
    public void LoadNext() => SceneChanger.instance.LoadNext();
    public void Retry() => SceneChanger.instance.Retry();
    public void Quit() => SceneChanger.instance.Quit();
    public void ReturnToMain() => SceneChanger.instance.LoadScene0_MainMenu();
    public void LoadStats() => SceneChanger.instance.LoadScene4_Stats();

    public void TogglePause() => OptionsMenu.instance.Toggle();

    public void RandomizeNameA(int i) => NameManager.instance.RandomizeLuna(i);
    public void RandomizeNameB(int i) => NameManager.instance.RandomizeDrillian(i);
    public void RandomizeName(int i) => NameManager.instance.RandomizeBoth(i);

    public void RumbleFeedback() => Rumble.instance?.RumbleFeedback();
    public void RumbleFeedback_Luna() => Rumble.instance?.RumbleFeedback_Luna();
    public void RumbleFeedback_Drillian() => Rumble.instance?.RumbleFeedback_Drillian();
}
