using Shapes;
using UnityEngine;

public class DrillianControlMarker : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [SerializeField] DrillianController _drillian;
    [SerializeField] Rectangle _fillRect;


    //--- Unity Methods ------------------------

    public void LateUpdate()
    {
        Debug.Log(_drillian.RotationControlT);
        _fillRect.Height = _drillian.RotationControlT;
    }

}
