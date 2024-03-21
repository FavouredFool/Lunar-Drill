using DG.Tweening;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField][Range(0.1f, 5f)] float _timeTillOrbit;
    
    public void Start()
    {
        DOTween.To(() => transform.position, x => transform.position = x, transform.position.normalized * Utilities.OuterOrbit, _timeTillOrbit).SetEase(Ease.OutSine);
    }

    public void DestroyPickup()
    {
        Destroy(gameObject);
    }
}
