using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardEntryUI : MonoBehaviour
{
    [SerializeField] TMP_Text _text;

    public void Init(LeaderboardManager.LeaderboardEntry entry)
    {
        _text.text = entry.ToString();
    }
}
