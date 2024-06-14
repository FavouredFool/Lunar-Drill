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
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync((int)scene);
        asyncOperation.allowSceneActivation = false;

        float delay = consistentCanvas.SetScene(scene);
        InputBus.Fire(new Signal_SceneChange(scene, delay));

        float initTime = Time.unscaledTime;

        while (!asyncOperation.isDone || Time.unscaledTime - initTime < delay)
        {
            if (asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true;
                currentScene = scene;
            }

            yield return null;
        }
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