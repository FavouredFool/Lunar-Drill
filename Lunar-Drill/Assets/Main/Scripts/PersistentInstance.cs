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
    public Rumble rumbleManager;

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
            Rumble.instance = rumbleManager;

            optionsMenu.SetUp();

            preparationInterface.SetScene(SceneIdentity.MainMenu);

            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
            Destroy(gameObject);
    }
}
