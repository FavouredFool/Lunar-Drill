using DG.Tweening;
using UnityEngine;

public class MineController : MonoBehaviour
{
    #region --- Exposed Fields ---
    [Header("Layers")]
    [SerializeField] LayerMask _destroyLayer; // Luna laser
    [SerializeField] LayerMask _damageLayerDrillian; // Drillian
    [SerializeField] LayerMask _damageLayerLuna; // Luna

    [Header("Visuals")]
    [SerializeField] SpriteRenderer _mineVisuals;
    #endregion

    #region --- Private Fields ---
    DrillianController _drillian;
    LunaController _luna;
    #endregion

    #region --- Public Fields ---
    public bool Active { get; set; } = false;
    public Tween MoveTween { get; set; }
    #endregion

    #region --- Unity Methods ---
    private void Awake()
    {
        _drillian = FindObjectOfType<DrillianController>();
        _luna = FindObjectOfType<LunaController>();
        float size = transform.localScale.x;
        transform.localScale = Vector3.zero;
        MoveTween = transform.DOScale(size, 0.33f).SetEase(Ease.OutBack).OnComplete(() => Active = true);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        // TODO Should exploding mines have a damage zone affecting every character?
        // TODO Does something happen if spider touches mines?
        // TODO Should Luna/Drillian also make the mine explode while they hit it but are invincible? (hits with them are handled in respective controllers)

        if (Active) // Make sure collisions are just checked while mine is active
        {
            if (_destroyLayer == (_destroyLayer | 1 << collision.gameObject.layer))
            {
                DestroyMine();  
                // TODO: Rumble?
            }
        }
    }
    #endregion

    #region --- Private Methods ---

    #endregion

    #region --- Public Methods ---
    public void DestroyMine()
    {
        Active = false;

        // Removing mine from spawner
        MineSpawner spawner = transform.parent.GetComponent<MineSpawner>();
        if (spawner)
            spawner.RemoveMine(this);

        MoveTween.Kill();
        MoveTween = transform.DOScale(0, .5f).SetEase(Ease.InBack);

        Destroy(gameObject, 5f);
    }

    #endregion
}
