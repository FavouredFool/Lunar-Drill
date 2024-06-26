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
    [SerializeField] CoopButton ContinueButton, SkipButton;
    [SerializeField] LoadSceneReaction SkipRetreat;

    int entryIndex;
    private void Start()
    {
        PersistentButton = PreparationInterface.instance._continueButton;
        PersistentButton.blocked = true;
        PersistentButton.transform.parent.localScale = Vector3.zero;
        ContinueButton.blocked = false;
        ContinueButton.transform.parent.gameObject.SetActive(true);
        ContinueButton.transform.parent.localScale = Vector3.one* 1.5f;
        SkipButton.blocked = false;

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
        entryIndex++;

        TutorialEntry entry = entries[entryIndex];

        ContinueButton.Refresh(entry.character);

        DisplayEntry(entry);

        PreparationInterface.instance.NextUp_Text.text = "Skip Tutorial";

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
        PreparationInterface.instance.NextUp_Text.text = "LETS GO!";

        PersistentButton.blocked = false;
        PersistentButton.transform.parent.localScale = Vector3.one* 1.5f;
        ContinueButton.blocked = true;
        ContinueButton.transform.parent.localScale = Vector3.zero;

        SkipRetreat.Return(0.33f);
        SkipButton.blocked = true;

    }
    public void Skip()
    {
        Finish();
        SceneChanger.instance.LoadNext();
    }

    public void OnEventHappened(Signal_SceneChange e)
    {
        
        PersistentButton = PreparationInterface.instance._continueButton;
        PersistentButton.blocked = false;
        PersistentButton.transform.parent.localScale = Vector3.one*1.5f;
        ContinueButton.blocked = true;
        ContinueButton.transform.parent.localScale = Vector3.zero;
        SkipButton.blocked = true;

        foreach (TutorialSpeechBubble spb in bubbles)
            spb.Hide();
    }
}

[System.Serializable]public class TutorialEntry
{
    public ChosenCharacter character;
    [TextArea] public string message;
}