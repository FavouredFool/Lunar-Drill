using DG.Tweening;
using Shapes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class OreController : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [Header("Layers")]
    [SerializeField] LayerMask _pickUpLayer;
    [SerializeField] LayerMask _destroyLayer;

    [Header("Radius")]
    [SerializeField][Range(0.1f, 5f)] float _outerRadius;
    [SerializeField][Range(0.1f, 5f)] float _planetRadius;

    [Header("Configuration")]
    [SerializeField][Range(0.1f, 5f)] float _durationTillOnOuterRadius;
    [SerializeField][Range(0.1f, 90f)] float _shootOutAngle;

    [Header("Visuals")]
    [SerializeField] SpriteRenderer _oreVisuals;
    [SerializeField] Color _burrowedColor;
    [SerializeField] Color _flyingColor;

    [SerializeField] Sprite
        _embedded,
        _collected,
        _energy;

    [SerializeField] float fps = 6;
    [SerializeField] float rotTime = 2;

    float timer = 0;
    float fraction => 1f / fps;
    float angleStep => rotTime / fraction;


    public enum OreState { BURROWED, FOLLOWING, FLYING  };

    public bool Collected { get; set; } = false;


    //--- Private Fields ------------------------

    DrillianController _followDrillian;
    OreState _oreState = OreState.BURROWED;
    Rigidbody2D _rigidbody;
    Tween _moveTween;

    float decayTime;
    float starTime = float.PositiveInfinity;


    //--- Unity Methods ------------------------

    public void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _oreVisuals.sprite = _embedded;
        _oreVisuals.transform.Rotate(Vector3.forward, Random.Range(0,360));
    }

    private void Update()
    {
        if (_oreState != OreState.BURROWED)
        {
            timer += Time.deltaTime;

            if (timer >= fraction)
            {
                timer = 0;

                _oreVisuals.transform.Rotate(Vector3.forward, angleStep);
            }
        }
        if (_oreState == OreState.FLYING)
        {
            if (Time.time - starTime > 45)
            {
                DestroyOre();
            }
        }
    }

    //public void LateUpdate()
    //{
    //    if (_oreState == OreState.FOLLOWING)
    //    {
    //        MoveToFollow();
    //    }
    //}

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (_pickUpLayer == (_pickUpLayer | 1 << collision.gameObject.layer))
        {
            if (_oreState == OreState.BURROWED)
            {
                SetDrillian(collision);
            }
        }
        else if (_destroyLayer == (_destroyLayer | 1 << collision.gameObject.layer))
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

        OreSpawner spawner = transform.parent.GetComponent<OreSpawner>();
        Assert.IsNotNull(spawner);
        spawner.RemoveOre(this);

        // Doesnt use rigidbodies so that i can tween it properly
        transform.position = _followDrillian.transform.position;

        Vector2 goalDirection = Quaternion.Euler(0,0,Random.Range(-_shootOutAngle, _shootOutAngle)) * _followDrillian.transform.up;
        Vector2 goalPosition = ((Vector2)_followDrillian.transform.position + goalDirection * (_outerRadius - _planetRadius)).normalized * _outerRadius;

        _moveTween = DOTween.To(() => (Vector2)transform.position, x => transform.position = x, goalPosition, _durationTillOnOuterRadius).SetEase(Ease.OutSine)
            .OnComplete(() => {
                _oreVisuals.sprite = _energy;
                starTime = Time.time;
            });
    }

    public void DestroyOre()
    {
        OreSpawner spawner = transform.parent.GetComponent<OreSpawner>();

        if (spawner)
            spawner.RemoveOre(this);

        _moveTween = transform.DOScale(0, 0.5f).SetEase(Ease.InBack);

        Destroy(gameObject, 5f);
    }


    //--- Private Methods ------------------------

    void SetDrillian(Collider2D collision)
    {
        _followDrillian = collision.gameObject.GetComponent<DrillianController>();

        Assert.IsNotNull(_followDrillian);

        AudioController.Fire(new OreCrackedAudioEvent(_followDrillian.FollowingOres.Count));

        _oreVisuals.sprite = _collected;

        _oreState = OreState.FOLLOWING;
        _followDrillian.FollowingOres.Add(this);

        Rumble.main?.AddRumble(ChosenCharacter.drillian, new Vector2(0.5f, 0.6f), 0.1f);
    }

    //void MoveToFollow()
    //{
    //    List<OreController> ores = _followDrillian.FollowingOres;
    //    int index= ores.IndexOf(this);
    //    Transform
    //    Vector2 oreOffset = (_followDrillian.FollowingOres.IndexOf(this) + 1) * _followDrillian.OreDistance * -_followDrillian.transform.up;

    //    _rigidbody.MovePosition(((Vector2)_followDrillian.transform.position) + oreOffset);
    //}
}
