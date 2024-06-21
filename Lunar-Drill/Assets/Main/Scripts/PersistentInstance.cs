using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentInstance : MonoBehaviour
{
    static PersistentInstance instance;

    public SceneChanger sceneChanger;
    public PreparationInterface preparationInterface;
    public OptionsMenu optionsMenu;
    public NameManager nameManager;
    public LeaderboardManager leaderboardManager;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
            SceneChanger.instance = sceneChanger;
            PreparationInterface.instance = preparationInterface;
            OptionsMenu.instance = optionsMenu;
            NameManager.instance = nameManager;

            preparationInterface.SetScene(SceneIdentity.MainMenu);
        }
        else
            Destroy(gameObject);
    }
}
