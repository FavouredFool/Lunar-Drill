using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TutorialManager : MonoBehaviour, IInputSubscriber<Signal_SceneChange>
{
    [SerializeField] List<TutorialEntry> entries = new List<TutorialEntry>();

    [SerializeField]
    TutorialSpeechBubble[] bubbles;

    CoopButton PersistentButton;
    [SerializeField] CoopButton TemporaryButton;

    int entryIndex;
    private void Start()
    {
        InputBus.Subscribe(this);

        PersistentButton = PreparationInterface.instance._continueButton;
        PersistentButton.blocked = true;
        PersistentButton.gameObject.SetActive(false);
        TemporaryButton.blocked = false;
        TemporaryButton.gameObject.SetActive(true);

        entryIndex = -1;
        Continue();
    }

    public void Continue()
    {
        entryIndex++;

        TutorialEntry entry = entries[entryIndex];

        TemporaryButton._inputCharacter = entry.character;
        TemporaryButton._requiredPressTime = 0f;

        DisplayEntry(entry);

        PreparationInterface.instance.NextUp_Text.text = "Continue";

        if (entryIndex + 1 == entries.Count)
            Finish();
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
        PreparationInterface.instance.NextUp_Text.text = "LETS GO!";

        PersistentButton.blocked = false;
        PersistentButton.gameObject.SetActive(true);
        TemporaryButton.blocked = true;
        TemporaryButton.gameObject.SetActive(false);
    }

    public void OnEventHappened(Signal_SceneChange e)
    {
        foreach (TutorialSpeechBubble spb in bubbles)
            spb.Hide();
    }
}

[System.Serializable]public class TutorialEntry
{
    public ChosenCharacter character;
    [TextArea] public string message;
}