using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DrillianSpriteIterator : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] DrillianController controller;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite[] sprites;
    [SerializeField] float fps=18;

    float timer=0;
    float velocityLerp => rb.velocity.magnitude / 6f;
    float stateLerp = 1;

    float fraction => (1f / (fps*stateLerp*velocityLerp));
    int index = 0;

    Tween stateLerpTween;

    private void Update()
    {
        if (controller.LastFrameIsBurrowed!=controller.IsBurrowed)
        {
            stateLerpTween.Kill();

            if (controller.IsBurrowed) stateLerp = 1;
            else stateLerpTween = DOTween.To(() => stateLerp, x => stateLerp = x, 0, 0.5f).SetEase(Ease.OutQuad);
        }

        timer += Time.deltaTime;
        if (timer>=fraction)
        {
            timer = 0;
            index=(index+1)%sprites.Length;
            spriteRenderer.sprite = sprites[index];
        }

    }
}
