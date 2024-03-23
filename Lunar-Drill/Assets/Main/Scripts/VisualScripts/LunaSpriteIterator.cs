using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Shapes;

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

    [SerializeField] Disc[] barDiscs;
    bool energyBarVisible => controller.CurrentlyLasering || controller.EnergyGained || controller.EnergyFull;
    float energyBarAlpha;

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

        energyBarAlpha += (energyBarVisible?2:-2)*Time.deltaTime;
        energyBarAlpha = Mathf.Clamp01(energyBarAlpha);
        UpdateEnergyBar();
    }
    void UpdateEnergyBar()
    {
        foreach (Disc d in barDiscs)
        {
            Color c = d.Color;
            c.a = energyBarAlpha;
            d.Color = c;
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

        energyBarAlpha = 0;
        UpdateEnergyBar();
    }
}
