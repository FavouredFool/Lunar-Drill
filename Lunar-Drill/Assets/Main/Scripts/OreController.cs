using DG.Tweening;
using Shapes;
using UnityEngine;
using UnityEngine.Assertions;

public class OreController : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [Header("Layers")]
    [SerializeField] LayerMask _pickUpLayer;
    [SerializeField] LayerMask _consumeLayer;

    [Header("Radius")]
    [SerializeField][Range(0.1f, 5f)] float _outerRadius;
    [SerializeField][Range(0.1f, 5f)] float _planetRadius;

    [Header("Configuration")]
    [SerializeField][Range(0.1f, 5f)] float _durationTillOnOuterRadius;
    [SerializeField][Range(0.1f, 90f)] float _shootOutAngle;

    [Header("Visuals")]
    [SerializeField] Disc _oreVisuals;
    [SerializeField] Color _burrowedColor;
    [SerializeField] Color _flyingColor;


    public enum OreState { BURROWED, FOLLOWING, FLYING  };


    //--- Private Fields ------------------------

    DrillianController _followDrillian;
    OreState _oreState = OreState.BURROWED;
    Rigidbody2D _rigidbody;
    Tween _moveTween;


    //--- Unity Methods ------------------------

    public void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _oreVisuals.Color = _burrowedColor;
    }

    public void LateUpdate()
    {
        if (_oreState == OreState.FOLLOWING)
        {
            MoveToFollow();
        }
            
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (_pickUpLayer == (_pickUpLayer | 1 << collision.gameObject.layer))
        {
            if (_oreState == OreState.BURROWED)
            {
                SetDrillian(collision);
            }
        }
        else if (_consumeLayer == (_consumeLayer | 1 << collision.gameObject.layer))
        {
            if (_oreState == OreState.FLYING)
            {
                DestroyOre();
            }
        }
    }

    //--- Public Methods ------------------------

    public void ReleaseOre()
    {
        _oreState = OreState.FLYING;

        // Doesnt use rigidbodies so that i can tween it properly
        transform.position = _followDrillian.transform.position;
        _oreVisuals.Color = _flyingColor;

        Vector2 goalDirection = Quaternion.Euler(0,0,Random.Range(-_shootOutAngle, _shootOutAngle)) * _followDrillian.transform.up;
        Vector2 goalPosition = ((Vector2)_followDrillian.transform.position + goalDirection * (_outerRadius - _planetRadius)).normalized * _outerRadius;

        _moveTween = DOTween.To(() => (Vector2)transform.position, x => transform.position = x, goalPosition, _durationTillOnOuterRadius).SetEase(Ease.OutSine);
    }


    //--- Private Methods ------------------------

    void DestroyOre()
    {
        if (_moveTween != null && _moveTween.IsActive())
        {
            _moveTween.Kill();
        }

        Destroy(gameObject);
    }

    void SetDrillian(Collider2D collision)
    {
        _followDrillian = collision.gameObject.GetComponent<DrillianController>();

        Assert.IsNotNull(_followDrillian);

        _oreState = OreState.FOLLOWING;
        _followDrillian.FollowingOres.Add(this);
    }

    void MoveToFollow()
    {
        Vector2 oreOffset = (_followDrillian.FollowingOres.IndexOf(this) + 1) * _followDrillian.OreDistance * -_followDrillian.transform.up;

        _rigidbody.MovePosition(((Vector2)_followDrillian.transform.position) + oreOffset);
    }
}
