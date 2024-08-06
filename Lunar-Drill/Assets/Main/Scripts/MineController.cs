using DG.Tweening;
using UnityEngine;

public class MineController : MonoBehaviour
{
    #region --- Exposed Fields ---
    [Header("Layers")]
    [SerializeField] LayerMask _destroyLayer; // Luna laser
    [SerializeField] LayerMask _damageLayer; // Drillian

    [Header("Visuals")]
    [SerializeField] SpriteRenderer _mineVisuals;
    #endregion

    #region --- Private Fields ---
    Tween _moveTween;
    bool _active = false; // TODO Should it be active mid air or when hitting the planet surface
    DrillianController _drillian;
    #endregion

    #region --- Public Fields ---
    #endregion

    #region --- Unity Methods ---
    private void Awake()
    {
        _drillian = FindObjectOfType<DrillianController>();
        float size = transform.localScale.x;
        transform.localScale = Vector3.zero;
        _moveTween = transform.DOScale(size, 0.33f).SetEase(Ease.OutBack).OnComplete(() => _active = true);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        // TODO Should exploding mines have a damage zone affecting every character?
        // TODO Does something happen if spider touches mines?

        if (_active) // Make sure collisions are just checked while mine is active
        {
            if (_destroyLayer == (_destroyLayer | 1 << collision.gameObject.layer))
            {
                DestroyMine();
                _active = false;

                // TODO Figure out if this is good when letting mine explode -> makes sense, but also same effect as getting damaged, might be confusing
                CamShake.Instance.ShakeCamera();
                Rumble.instance?.RumbleLuna(4, 2, 0.2f);
            }
            else if (_damageLayer == (_damageLayer | 1 << collision.gameObject.layer))
            {
                DestroyMine();
                _active = false;

                if (!_drillian.IsInvincible) // TODO Should Drillian be able to let the mines explode while invincible??
                    _drillian.GetHit(collision);
            }
        }
    }
    #endregion

    #region --- Private Methods ---
    private void DestroyMine()
    {
        // Removing mine from spawner
        MineSpawner spawner = transform.parent.GetComponent<MineSpawner>();
        if (spawner)
            spawner.RemoveMine(this);

        _moveTween = transform.DOScale(0, .5f).SetEase(Ease.InBack);

        Destroy(gameObject, 5f);
    }
    #endregion

    #region --- Public Methods ---
    #endregion
}
