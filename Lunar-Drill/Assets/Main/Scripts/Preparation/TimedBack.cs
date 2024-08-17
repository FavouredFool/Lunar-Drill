using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TimedBack : MonoBehaviour
{
    public PreparationManager manager;

    [SerializeField] TMP_Text timerText;
    [SerializeField] Image barImage;

    public float maxTime;
    float timer;

    private void OnEnable()
    {
        timer = maxTime;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        timerText.text = Mathf.CeilToInt(timer).ToString();
        barImage.fillAmount = timer / maxTime;

        if (timer<0)
            manager.Back();
    }

}
