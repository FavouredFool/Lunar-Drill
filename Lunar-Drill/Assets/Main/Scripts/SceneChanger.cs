using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger instance;

    private void Awake()
    {
        instance = this;
    }

    public void LoadScene0_MainMenu() => LoadScene(SceneIdentity.MainMenu);
    public void LoadScene1_PlayerConnect() => LoadScene(SceneIdentity.PlayerConnect);
    public void LoadScene2_PlayerSelect() => LoadScene(SceneIdentity.PlayerSelect);
    public void LoadScene3_GameTutorial() => LoadScene(SceneIdentity.GameTutorial);
    public void LoadScene4_GameMain() => LoadScene(SceneIdentity.GameMain);
    public void LoadScene(SceneIdentity scene)
    {
        SceneManager.LoadScene((int)scene);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
public enum SceneIdentity
{
    MainMenu, PlayerConnect, PlayerSelect, GameTutorial, GameMain
}