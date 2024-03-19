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
        _fillRect.Height = _drillian.RotationControlT;
    }

}
