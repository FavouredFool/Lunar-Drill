using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMenuUIManager : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [Header("Animated Laser")]
    [SerializeField] private GameObject _laser;
    [SerializeField] private float _normalLaserHeight = 250;
    [SerializeField] private float _laserShrinkTimeAwake = 1f;

    [Header("Select Menu")]
    [SerializeField] private List<GameObject> _mainMenuButtons;



    // --- Public Fields ------------------------

    //--- Private Fields ------------------------
    private RectTransform _laserRect;

    //--- Unity Methods ------------------------

    private void Awake()
    {
        _laserRect = _laser.GetComponent<RectTransform>();
        _laserRect.DOSizeDelta(new Vector2(_laserRect.sizeDelta.x, _normalLaserHeight), _laserShrinkTimeAwake).SetDelay(.2f).SetUpdate(true); // Scale down
    }

    //--- Public Methods ------------------------


    //--- Private Methods ------------------------
}
