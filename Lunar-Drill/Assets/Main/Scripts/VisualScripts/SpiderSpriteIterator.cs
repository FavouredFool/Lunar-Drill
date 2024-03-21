using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpiderSpriteIterator : MonoBehaviour
{
    [SerializeField] SpiderController controller;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Transform crest;
    [SerializeField] float fps = 6;
    [SerializeField] float rotTime = 2;

    float timer = 0;

    int lastMoveSign = 1;

    float fraction => 1f / fps;
    float angleStep => rotTime/fraction;

    Tween scaleTween;
    float initialScale;

    private void Awake()
    {
        initialScale = spriteRenderer.transform.localScale.x;
    }
    private void Update()
    {
        lastMoveSign = controller.MoveSign;
        animator.SetInteger("moveDirection", lastMoveSign);

        crest.gameObject.SetActive(!isControlled);

        timer += Time.deltaTime;
        if (timer >= fraction && lastMoveSign != 0 && !isControlled)
        {
            timer = 0;

            crest.Rotate(Vector3.forward, angleStep);
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

    [SerializeField] Sprite _hitSprite, _stunSprite;
    public void Hit()
    {
        Control(_hitSprite, TimeManager.hitFrameTime);

        scaleTween.Kill();
        scaleTween = spriteRenderer.transform.DOScale(initialScale*1.33f, TimeManager.hitFrameTime * 0.33f).SetUpdate(true).SetEase(Ease.OutBack).OnComplete(() =>
        {
            scaleTween = spriteRenderer.transform.DOScale(initialScale, 0.1f).SetUpdate(true).SetEase(Ease.OutSine);
        });
    }
    public void Stun(float time)
    {
        Control(_hitSprite, time);

        float pulseTime = 1/3;
        int loops = Mathf.FloorToInt(time / pulseTime);
        if (loops % 2 == 0) loops += 1;

        scaleTween.Kill();
        scaleTween = spriteRenderer.transform.DOScale(initialScale*1.33f, pulseTime).SetEase(Ease.OutBack).SetLoops(loops, LoopType.Yoyo);
    }
}
