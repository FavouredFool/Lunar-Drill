using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] LeaderboardEntryUI _entryBlueprint;
    
    void Start()
    {
        KillChildren();
        Populate();
    }

    void KillChildren()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    void Populate()
    {
        foreach (LeaderboardManager.LeaderboardEntry entry in LeaderboardManager.EntryList.Entries)
        {
            LeaderboardEntryUI entryUI = Instantiate(_entryBlueprint, transform);
            entryUI.Init(entry);
        }
    }
}
