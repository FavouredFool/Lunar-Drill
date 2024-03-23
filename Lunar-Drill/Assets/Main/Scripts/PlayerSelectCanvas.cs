using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerSelectCanvas : MonoBehaviour
{
    ////--- Exposed Fields ------------------------

    //[SerializeField][Range(0f, 100f)] float _sideMovementDelta;

    //[SerializeField] SelectScreenHandling _selectScreenHandling;

    //public enum PlayerSelectState { CONNECT, PREPARE };

    ////--- Properties ------------------------

    //public bool IsConnectState { get; private set; } = true;


    ////--- Private Fields ------------------------

    //RectTransform rectTransform;


    ////--- Unity Methods ------------------------

    //public void Awake()
    //{
    //    rectTransform = GetComponent<RectTransform>();
    //    _selectScreenHandling.Starting.AddListener(() => ChangeConnected(true));
    //    _selectScreenHandling.StoppingGameStart.AddListener(() => ChangeConnected(false));
    //}


    //public void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.P))
    //    {
    //        SwapStates();
            
    //    }
    //}

    ////--- Public Methods ------------------------

    //public void MoveToSide()
    //{
    //    float endPosition = IsConnectState ? _sideMovementDelta : 0;

    //    rectTransform.DOLocalMoveX(endPosition, 1).SetEase(Ease.InOutQuint);
    //}

    //public void SwapStates()
    //{
    //    IsConnectState = !IsConnectState;
    //    MoveToSide();
    //}

    //public void ChangeConnected(bool newState)
    //{
    //    IsConnectState = newState;
    //    MoveToSide();
    //}

}
