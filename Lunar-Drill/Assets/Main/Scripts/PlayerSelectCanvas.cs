using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class PlayerSelectCanvas : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [SerializeField][Range(0f, 100f)] float _sideMovementDelta;

    public enum PlayerSelectState { CONNECT, PREPARE };

    //--- Properties ------------------------

    public bool IsConnectState { get; private set; } = true;


    //--- Private Fields ------------------------

    RectTransform rectTransform;


    //--- Unity Methods ------------------------

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SwapStates();
            
        }
    }

    //--- Public Methods ------------------------

    public void MoveToSide()
    {
        float endPosition = IsConnectState ? _sideMovementDelta : 0;

        rectTransform.DOLocalMoveX(endPosition, 1).SetEase(Ease.InOutQuint);
    }

    public void SwapStates()
    {
        IsConnectState = !IsConnectState;
        MoveToSide();
    }
}
