using Shapes;
using UnityEngine;
using UnityEngine.InputSystem;

public class LunaControllerMarker : MonoBehaviour
{
    //--- Private Fields ------------------------

    Vector2 _goalDirection;
    Line _visual;


    //--- Unity Methods ------------------------

    public void Awake()
    {
        _goalDirection = Vector2.zero;
        _visual = GetComponent<Line>();
    }

    public void FixedUpdate()
    {
        SetMarkerPosition();
        SetMarkerRotation();
    }


    //--- Public Methods ------------------------

    public void SetGoalDirectionInput(Vector2 direction)
    {
        _goalDirection = direction;
    }


    //--- Private Methods ------------------------

    void SetMarkerPosition()
    {
        if (_goalDirection.magnitude < 0.1f)
        {
            _visual.enabled = false;
        }
        else
        {
            _visual.enabled = true;
        }

        float angle = Vector2.SignedAngle(Vector2.up, _goalDirection);

        Vector2 rotatedVector = Quaternion.Euler(0f, 0f, angle) * Vector2.up;
        Vector2 position = rotatedVector * Utilities.OuterOrbit;

        transform.position = position;
    }

    void SetMarkerRotation()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.forward, -transform.position);
    }
}
