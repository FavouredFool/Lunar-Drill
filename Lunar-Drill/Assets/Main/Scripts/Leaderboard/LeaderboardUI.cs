using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class LeaderboardUI : MonoBehaviour, IInputSubscriber<MenuMoveNorth>, IInputSubscriber<MenuMoveSouth>, IInputSubscriber<Signal_SceneChange>
{
    [SerializeField] LeaderboardEntryUI _entryBlueprint;
    [SerializeField] Transform _contentMother;
    [SerializeField] ScrollRect _scrollRect;
    [SerializeField] float speed;
    [SerializeField] Image blackfade;

    int lastIndex = 0;

    int northInput = 0, southInput = 0;
    float Scroll => Mathf.Clamp01(northInput) - Mathf.Clamp01(southInput);

    private void OnEnable()
    {
        InputBus.Subscribe<MenuMoveNorth>(this);
        InputBus.Subscribe<MenuMoveSouth>(this);
        InputBus.Subscribe<Signal_SceneChange>(this);
    }
    private void OnDisable()
    {
        InputBus.Unsubscribe<MenuMoveNorth>(this);
        InputBus.Unsubscribe<MenuMoveSouth>(this);
        InputBus.Unsubscribe<Signal_SceneChange>(this);
    }

    void Start()
    {
        KillChildren();
        Populate();
        //lastIndex= _contentMother.childCount;
        ScrollToIndex(lastIndex);

        blackfade.color = Color.black;
        blackfade.DOFade(0, 1f).SetEase(Ease.InOutSine).SetUpdate(true);
    }
    void Update()
    {
        ScrollByValue(Scroll);
    }

    void KillChildren()
    {
        foreach (Transform child in _contentMother)
        {
            Destroy(child.gameObject);
        }
    }
    void Populate()
    {
        lastIndex = 0;
        for (int i = 0; i < LeaderboardManager.EntryList.Entries.Count; i++)
        {
            LeaderboardManager.LeaderboardEntry entry = LeaderboardManager.EntryList.Entries[i];

            LeaderboardEntryUI entryUI = Instantiate(_entryBlueprint, _contentMother);
            entryUI.Init(i + 1, entry, entry.isLast);

            if (entry.isLast) lastIndex = i+1;
        }
    }

    public void ScrollByValue(float scroll)
    {
        if (scroll != 0)
        {
            float contentHeight = _scrollRect.content.sizeDelta.y;
            float contentShift = speed * scroll * Time.deltaTime;
            float scrollValue = contentShift / contentHeight;

            _scrollRect.verticalNormalizedPosition += scrollValue;
        }
    }
    public void ScrollToIndex(int index)
    {
        float scrollValue = (float)index / (float)_contentMother.childCount;
        _scrollRect.verticalNormalizedPosition = 1 - scrollValue;
    }

    public void OnEventHappened(MenuMoveNorth e)
    {
        if (e.context.phase == UnityEngine.InputSystem.InputActionPhase.Started)
            northInput++;
        else if (e.context.phase == UnityEngine.InputSystem.InputActionPhase.Canceled)
            northInput--;
    }
    public void OnEventHappened(MenuMoveSouth e)
    {
        if (e.context.phase == UnityEngine.InputSystem.InputActionPhase.Started)
            southInput++;
        else if (e.context.phase == UnityEngine.InputSystem.InputActionPhase.Canceled)
            southInput--;
    }

    public void FindLast()
    {

    }
    public void ClearAll()
    {
        if (!LeaderboardManager.LeaderboardIsEnabled) return;
        
        LeaderboardManager.Clear();
        // update Visuals
        KillChildren();
        Populate();
    }

    public void OnEventHappened(Signal_SceneChange e)
    {
        blackfade.DOFade(1, e.delay).SetEase(Ease.InSine).SetUpdate(true);
    }
}
