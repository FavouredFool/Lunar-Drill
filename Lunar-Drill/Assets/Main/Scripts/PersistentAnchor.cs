using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentAnchor : MonoBehaviour
{
    public void LoadNext() => SceneChanger.instance.LoadNext();
    public void Quit() => SceneChanger.instance.Quit();
    public void ReturnToMain() => SceneChanger.instance.LoadScene0_MainMenu();
    public void TogglePause() => OptionsMenu.Instance.Toggle();

}
