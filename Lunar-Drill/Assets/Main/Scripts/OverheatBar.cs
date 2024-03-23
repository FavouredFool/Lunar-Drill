using DG.Tweening;
using Shapes;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

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

        float tValue;


        if (_spiderController.IsVulnerable)
        {
            tValue = _spiderController.RegenerateT;
        }
        else
        {
            tValue = 1 - _spiderController.OverheatT;
        }

        _bar.AngRadiansStart = DOVirtual.EasedValue(Mathf.Deg2Rad * _barStartValue, Mathf.Deg2Rad * _barEndValue, tValue, Ease.Linear);

        _bar.enabled = !Mathf.Approximately(_bar.AngRadiansStart, _bar.AngRadiansEnd);
    }

    //--- Public Methods ------------------------


    //--- Private Methods ------------------------
}
