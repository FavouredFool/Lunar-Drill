using DG.Tweening;
using Shapes;
using UnityEngine;

public class OverheatBar : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [SerializeField] Disc _bar;
    [SerializeField][Range(0, 359)] float _barStartValue;
    [SerializeField][Range(0, 359)] float _barEndValue;


    //--- Private Fields ------------------------

    SpiderController _spiderController;

    //--- Unity Methods ------------------------

    public void Start()
    {
        _spiderController = FindObjectOfType<SpiderController>();
    }

    public void LateUpdate()
    {
        if (_spiderController == null) throw new System.Exception();

        _bar.AngRadiansStart = DOVirtual.EasedValue(Mathf.Deg2Rad * _barStartValue, Mathf.Deg2Rad * _barEndValue, _spiderController.OverheatT, Ease.Linear);

        _bar.enabled = !Mathf.Approximately(_bar.AngRadiansStart, _bar.AngRadiansEnd);
    }

    //--- Public Methods ------------------------


    //--- Private Methods ------------------------
}
