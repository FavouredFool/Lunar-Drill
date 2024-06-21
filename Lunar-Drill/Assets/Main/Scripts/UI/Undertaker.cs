using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;

public class Undertaker : MonoBehaviour
{
    [SerializeField] GameObject[] disabledObjects;

    [SerializeField] Animator anim;

    [SerializeField] SpriteRenderer topBar, bottomBar;
    [SerializeField] GameObject[] playerText, enemyText;
    [SerializeField] Color col_cyan, col_yellow, col_lilac, col_magenta;

    [SerializeField] TMP_Text timer;

    [SerializeField] AudioSource[] sources;

    public void Awake()
    {
        timer.text = "";
    }

    public bool isActive { get; private set; } = false;

    public void Activate(GameObject target, bool isPlayer)
    {
        //Pause / mute audio
        AudioController.Fire(new EndSceneStateChange(EndSceneStateChange.State.EndScreenActive));

        Rumble.instance?.ClearAndStopAllRumble();


        TimeManager.main.Freeze();

        transform.position = target.transform.position;
        gameObject.SetActive(true);
        target.transform.GetChild(0).localRotation = Quaternion.Euler(0, 0, (isPlayer ? 180 : 0) + 20 * (Random.value < 0.5f ? -1 : 1));
        target.transform.localScale = Vector3.one * 1.5f;

        foreach (GameObject g in disabledObjects)
            g.SetActive(target == g);

        if (isPlayer)
        {
            foreach (GameObject g in playerText)
                g.SetActive(true);
            foreach (GameObject g in enemyText)
                g.SetActive(false);

            topBar.color = col_cyan;
            bottomBar.color = col_yellow;
        }
        else
        {
            foreach (GameObject g in playerText)
                g.SetActive(false);
            foreach (GameObject g in enemyText)
                g.SetActive(true);

            topBar.color = col_lilac;
            bottomBar.color = col_magenta;
        }

        anim.SetTrigger("GO");

        DOVirtual.DelayedCall(0.2f, () =>
        {
            if (isPlayer) AudioController.Fire(new EndSceneLunar(""));
            else AudioController.Fire(new EndSceneGame(""));
            Rumble.instance?.RumbleBoth(4, 2, 0.33f);
        });
        DOVirtual.DelayedCall(0.3f, () =>
        {
            AudioController.Fire(new EndSceneShing(""));
        });
        DOVirtual.DelayedCall(0.5f, () => { 
            AudioController.Fire(new EndSceneShing(""));
        });
        DOVirtual.DelayedCall(1.1f, () =>
        {
            if (isPlayer) AudioController.Fire(new EndSceneDrill(""));
            else AudioController.Fire(new EndSceneOver(""));
            Rumble.instance?.RumbleBoth(6, 2, 0.33f);
        });

        DOVirtual.DelayedCall(2f, () =>
        {
            GameManager gameManager = FindObjectOfType<GameManager>();
            string time = System.TimeSpan.FromSeconds(GameManager.PlayTime).ToString("mm\\:ss\\:ff");
            timer.text = time;
        });

        DOVirtual.DelayedCall(6f, () => Continue());
    }

    public void Continue()
    {
        AudioController.Fire(new EndSceneStateChange(EndSceneStateChange.State.EndScreenInactive));
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("MainMenuScene");
    }
}
