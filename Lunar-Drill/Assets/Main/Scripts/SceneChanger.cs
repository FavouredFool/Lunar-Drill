using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//DontDestroyOnLoad via Persistent Prefab
public class SceneChanger : MonoBehaviour
{
    public static SceneChanger instance;
    public static SceneIdentity currentScene;

    [SerializeField] PreparationInterface consistentCanvas;

    private void Awake()
    {
        instance = this;
    }

    public void LoadScene0_MainMenu() => LoadScene(SceneIdentity.MainMenu);
    public void LoadScene1_PlayerConnect() => LoadScene(SceneIdentity.PlayerConnect);
    public void LoadScene2_PlayerSelect() => LoadScene(SceneIdentity.PlayerSelect);
    public void LoadScene3_GameTutorial() => LoadScene(SceneIdentity.GameTutorial);
    public void LoadScene4_GameMain() => LoadScene(SceneIdentity.GameMain);
    public void LoadScene5_Leaderboard() => LoadScene(SceneIdentity.Leaderboard);
    public void LoadNext()
    {
        SceneIdentity identity = (SceneIdentity)((int)currentScene + 1);
        if ((int)identity >= System.Enum.GetValues(typeof(SceneIdentity)).Length)
            identity = (SceneIdentity)0;

        if (identity == SceneIdentity.PlayerSelect && PlayerConnectController.isSolo)
            identity = SceneIdentity.GameTutorial;

        LoadScene(identity);
    }
    public void LoadScene(SceneIdentity scene)
    {
        StartCoroutine(LoadSceneCoroutine(scene));
    }
    private IEnumerator LoadSceneCoroutine(SceneIdentity scene)
    {

        float delay = consistentCanvas.SetScene(scene);
        InputBus.Fire(new Signal_SceneChange(scene, delay));

        float initTime = Time.unscaledTime;

        while (Time.unscaledTime - initTime < delay)
            yield return null;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync((int)scene);
        currentScene = scene;
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
    MainMenu, PlayerConnect, PlayerSelect, GameTutorial, GameMain, Leaderboard
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