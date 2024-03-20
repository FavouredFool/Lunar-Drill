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
    [SerializeField] float fps;

    float timer=0;

    int lastMoveSign = 1;

    float fraction => (1f / fps);
    int index = 0;

    Tween stateLerpTween;

    private void Update()
    {
        if (controller.MoveSign != 0) lastMoveSign = controller.MoveSign;

        timer += Time.deltaTime;
        if (timer >= fraction)
        {
            timer = 0;

            index = (index + lastMoveSign);
            if (index < 0) index += sprites.Length;
            index = index % sprites.Length;

            spriteRenderer.sprite = sprites[index];
        }
    }
}
