using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardEntryUI : MonoBehaviour
{
    [SerializeField] TMP_Text _placeText,_nameText,_timeText;
    [SerializeField] GameObject _lastMarker;
    public bool isLast;

    public void Init(int place,LeaderboardManager.LeaderboardEntry entry, bool isLast)
    {
        (string,string) data= entry.ToStrings();

        this.isLast = isLast;

        _placeText.text = place + ".";
        _nameText.text = data.Item1;
        _timeText.text = data.Item2;

        if (isLast)
        {
            Color spider = new Color(162 / 255f, 0, 254f / 255f);
            _nameText.color = spider;
        }
    }
}
