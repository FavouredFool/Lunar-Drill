using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDGameStatsInfo : MonoBehaviour
{
    [SerializeField] TMP_Text
        nameText, timeText;

    private void Start()
    {
        nameText.text = NameManager.instance.ToString();
    }
    void Update()
    {
        timeText.text = GameManager.PlayTime.ToString("00.0") + "s";
    }
}
