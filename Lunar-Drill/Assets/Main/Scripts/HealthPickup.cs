using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField][Range(0.1f, 5f)] float _timeTillOrbit;

    Tween pulseTween;
    public bool HasBeenPickedUp = false;
    public bool PickupableByDrillian = false;
    
    public void Start()
    {
        DOTween.To(() => transform.position, x => transform.position = x, transform.position.normalized * Utilities.OuterOrbit, _timeTillOrbit).SetEase(Ease.OutSine).SetUpdate(true);
        DOVirtual.DelayedCall(2f, () => PickupableByDrillian = true);
    }

    public void DestroyPickup()
    {
        Destroy(gameObject, 0.25f);
    }
}
