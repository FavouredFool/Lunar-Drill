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

    [SerializeField] CoopButton ContinueButton, SkipButton;

    int entryIndex;
    private void Start()
    {
        entryIndex = -1;
        Continue();
    }
    private void OnEnable()
    {
        InputBus.Subscribe(this);
    }
    private void OnDisable()
    {
        InputBus.Unsubscribe(this);
    }

    public void Continue()
    {
        if (entryIndex + 1 == entries.Count)
        {
            Finish();
            return;
        }

        entryIndex++;

        TutorialEntry entry = entries[entryIndex];

        ContinueButton.Refresh(entry.character);

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

        switch (entry.character)
        {
            case ChosenCharacter.drillian:
                Rumble.instance?.RumbleFeedback_Drillian();
                break;
            case ChosenCharacter.luna:
                Rumble.instance?.RumbleFeedback_Luna();
                break;
            case ChosenCharacter.both:
            case ChosenCharacter.any:
            default:
                Rumble.instance?.RumbleFeedback();
                break;
        }
    }

    public void Finish()
    {
        SkipButton.blocked = true;
        ContinueButton.blocked = true;

        SceneChanger.instance.LoadNext();
    }
    public void Skip() => Finish();

    public void OnEventHappened(Signal_SceneChange e)
    {
        SkipButton.blocked = true;
        ContinueButton.blocked = true;

        foreach (TutorialSpeechBubble spb in bubbles)
            spb.Hide();
    }
}

[System.Serializable]public class TutorialEntry
{
    public ChosenCharacter character;
    [TextArea] public string message;
}