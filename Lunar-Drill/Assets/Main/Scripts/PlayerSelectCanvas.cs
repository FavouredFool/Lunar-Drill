using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerSelectCanvas : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [SerializeField][Range(0f, 100f)] float _sideMovementDelta;
    [SerializeField]string _nextScene; // the Scene to be loaded after the players selected their characters

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
        if( IsConnectState )
        {
            StartCoroutine(LoadPlayScene());

        }
    }

    // Function to start the loading of another Scene
    IEnumerator LoadPlayScene()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(_nextScene);
    }
}
