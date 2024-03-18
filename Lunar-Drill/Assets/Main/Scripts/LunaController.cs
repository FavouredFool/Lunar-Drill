using UnityEngine;

public class LunaController : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [SerializeField][Range(2,5)] float _innerOrbitRange;
    [SerializeField][Range(2, 5)] float _outerOrbitRange;
    [SerializeField][Range(0.1f, 10f)] float _rotationSpeed;
    [SerializeField][Range(0.1f, 10f)] float _graviationSpeed;

    //--- Private Fields ------------------------

    float _orbitRotationT;
    float _orbitDistanceT;


    //--- Unity Methods ------------------------

    public void OnValidate()
    {
        _innerOrbitRange = Mathf.Min(_innerOrbitRange, _outerOrbitRange);
    }

    public void Start()
    {
        _orbitRotationT = 0;
        _orbitDistanceT = 1;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            _orbitDistanceT += 0.1f;
        }
    }

    public void FixedUpdate()
    {
        CalculateOrbitDistance();
        CalculateOrbitRotation();
        SetLunaPosition();
    }

    public void LateUpdate()
    {
        SetLunaRotation();
    }


    //--- Public Methods ------------------------


    //--- Private Methods ------------------------

    public void CalculateOrbitDistance()
    {
        // decrease
        _orbitDistanceT -= _graviationSpeed * Time.deltaTime;
        // guard
        _orbitDistanceT = Mathf.Clamp01(_orbitDistanceT);
    }

    public void CalculateOrbitRotation()
    {
        // increase
        _orbitRotationT += _rotationSpeed * Time.deltaTime;
        // guard
        if (_orbitRotationT >= 1)
        {
            _orbitRotationT -= 1;
        }
    }

    public void SetLunaPosition()
    {
        float angle = _orbitRotationT.Remap(0, 1, 0, 360);
        float distance = _orbitDistanceT.Remap(0, 1, _innerOrbitRange, _outerOrbitRange);

        Vector2 rotatedVector = Quaternion.Euler(0f, 0f, angle) * Vector2.up;
        Vector2 position = rotatedVector * distance;

        transform.position = position;
    }

    public void SetLunaRotation()
    {
        transform.rotation = Quaternion.LookRotation(Vector3.forward, -transform.position);
    }

}
