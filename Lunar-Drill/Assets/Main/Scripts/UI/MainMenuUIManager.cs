using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIManager : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    //--- Private Fields ------------------------

    //--- Unity Methods ------------------------

    //--- Public Methods ------------------------

    /* Switches the scene to main game scene. */
    public void StartGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /* Quits the application. */
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

  //  public void Set

    //--- Private Methods ------------------------
}
