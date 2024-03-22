using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Undertaker : MonoBehaviour
{
    [SerializeField] GameObject[] disabledObjects;

    [SerializeField] Animator anim;

    [SerializeField] SpriteRenderer topBar, bottomBar;
    [SerializeField] GameObject[] playerText, enemyText;
    [SerializeField] Color col_cyan, col_yellow, col_lilac, col_magenta;

    public bool isActive { get; private set; } = false;

    public void Activate(GameObject target, bool isPlayer)
    {
        transform.position = target.transform.position;
        gameObject.SetActive(true);
        target.transform.rotation= Quaternion.Euler(0,0,0);
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
    }
}

