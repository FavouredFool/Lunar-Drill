using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//DontDestroyOnLoad via Persistent Prefab
public class SceneChanger : MonoBehaviour
{
    public static SceneChanger instance;
    public static SceneIdentity currentScene;

    public void LoadScene0_MainMenu() => LoadScene(SceneIdentity.MainMenu);
    public void LoadScene1_Preparation() => LoadScene(SceneIdentity.PlayerPreparation);
    public void LoadScene2_GameTutorial() => LoadScene(SceneIdentity.GameMain); //CHANGE THIS ONCE TUTORIAL IS SET
    public void LoadScene3_GameMain() => LoadScene(SceneIdentity.GameMain);
    public void LoadScene4_Stats() => LoadScene(SceneIdentity.Stats);
    public void LoadNext()
    {
        SceneIdentity identity = (SceneIdentity)((int)currentScene + 1);
        if ((int)identity >= System.Enum.GetValues(typeof(SceneIdentity)).Length)
            identity = (SceneIdentity)0;

        LoadScene(identity);
    }
    public void LoadScene(SceneIdentity scene)
    {
        StartCoroutine(LoadSceneCoroutine(scene));
    }
    private IEnumerator LoadSceneCoroutine(SceneIdentity scene)
    {
        float delay = 0.33f;
        InputBus.Fire(new Signal_SceneChange(scene, delay * 0.99f));

        float initTime = Time.unscaledTime;

        while (Time.unscaledTime - initTime < delay)
            yield return null;
        yield return null;

        DG.Tweening.DOTween.KillAll();
        Rumble.instance?.ClearAndStopAllRumble();
        Time.timeScale = 1;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync((int)scene);
        currentScene = scene;
    }

    public void Retry()
    {
        LoadScene3_GameMain();
    }
    public void Quit()
    {
        if (currentScene == SceneIdentity.MainMenu)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
        else
        {
            LoadScene0_MainMenu();
        }
    }
}
public enum SceneIdentity
{
    MainMenu, PlayerPreparation, GameTutorial, GameMain, Stats
}

public class Signal_SceneChange : IInputSignal
{
    public float delay;
    public SceneIdentity scene;

    public Signal_SceneChange(SceneIdentity s, float d)
    {
        scene = s;
        delay = d;
    }
}