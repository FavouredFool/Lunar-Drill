using DG.Tweening;
using Shapes;
using UnityEngine;

public class EnergyBar : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [SerializeField] Disc _bar;
    [SerializeField][Range(0, 359)] float _barStartValue;
    [SerializeField][Range(0, 359)] float _barEndValue;


    //--- Private Fields ------------------------

    LunaController _lunaController;

    //--- Unity Methods ------------------------

    public void Start()
    {
        _lunaController = FindObjectOfType<LunaController>();
    }

    public void LateUpdate()
    {
        if (_lunaController == null) throw new System.Exception();

        _bar.AngRadiansEnd = DOVirtual.EasedValue(Mathf.Deg2Rad * _barStartValue, Mathf.Deg2Rad * _barEndValue, _lunaController.EnergyT, Ease.Linear);

        _bar.enabled = !Mathf.Approximately(_bar.AngRadiansStart, _bar.AngRadiansEnd);
    }

    //--- Public Methods ------------------------


    //--- Private Methods ------------------------
}
