using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LunaSpriteIterator : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] LunaController controller;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite[] sprites;
    [SerializeField] float fps=6;

    float timer=0;

    int lastMoveSign = 1;

    float fraction => (1f / fps);
    int index = 0;

    Tween stateLerpTween;
    Tween scaleTween;
    float initialScale;

    private void Awake()
    {
        initialScale = spriteRenderer.transform.localScale.x;
    }

    private void Update()
    {
        if (controller.MoveSign != 0) lastMoveSign = controller.MoveSign;

        timer += Time.deltaTime;
        if (timer >= fraction&&!isControlled)
        {
            timer = 0;

            index = (index + lastMoveSign);
            if (index < 0) index += sprites.Length;
            index = index % sprites.Length;

            spriteRenderer.sprite = sprites[index];
        }
    }

    bool isControlled => Time.time < controlTime;
    float controlTime = 0;
    public void Control(Sprite sprite, float time)
    {
        controlTime = Time.time + time;
        timer = fraction;

        spriteRenderer.sprite = sprite;
    }

    [SerializeField] Sprite _hitSprite;
    public void Hit()
    {
        Control(_hitSprite, TimeManager.hitFrameTime);

        scaleTween.Kill();
        scaleTween = spriteRenderer.transform.DOScale(initialScale * 1.33f, TimeManager.hitFrameTime * 0.33f).SetUpdate(true).SetEase(Ease.OutBack).OnComplete(() =>
        {
            scaleTween = spriteRenderer.transform.DOScale(initialScale, 0.1f).SetUpdate(true).SetEase(Ease.OutSine);
        });
    }
}
