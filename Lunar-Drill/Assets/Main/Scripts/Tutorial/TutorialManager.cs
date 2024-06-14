using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] List<TutorialEntry> entries = new List<TutorialEntry>();

    [SerializeField]
    TutorialSpeechBubble[] bubbles;

    CoopButton PersistentButton;
    [SerializeField] CoopButton HiddenButton;

    int entryIndex;
    private void Start()
    {
        PersistentButton = PreparationInterface.ContinueButton;
        PersistentButton._OnInputPerformedEvents.RemoveAllListeners();
        PersistentButton._OnInputPerformedEvents = new Button.ButtonClickedEvent();

        entryIndex = -1;
        Continue();
    }

    public void Continue()
    {
        entryIndex++;
        if (entryIndex >= entries.Count)
        {
            Finish();
            return;
        }

        TutorialEntry entry = entries[entryIndex];

        PersistentButton._inputCharacter = entry.character;
        PersistentButton._requiredPressTime = 0f;
        HiddenButton._inputCharacter = entry.character;
        HiddenButton._requiredPressTime = 0f;

        DisplayEntry(entry);
    }
    public void DisplayEntry(TutorialEntry entry)
    {
        foreach (TutorialSpeechBubble spb in bubbles)
        {
            if (spb.character == entry.character)
                spb.Display(entry);
            else
                spb.Hide();
        }
    }

    public void Finish()
    {
        PersistentButton._OnInputPerformedEvents.AddListener(() => SceneChanger.instance.LoadNext());
        PersistentButton._inputCharacter = ChosenCharacter.both;
        PersistentButton._requiredPressTime = 1;

        foreach (TutorialSpeechBubble spb in bubbles)
            spb.Hide();

        SceneChanger.instance.LoadNext();
    }
}

[System.Serializable]public class TutorialEntry
{
    public ChosenCharacter character;
    [TextArea] public string message;
}