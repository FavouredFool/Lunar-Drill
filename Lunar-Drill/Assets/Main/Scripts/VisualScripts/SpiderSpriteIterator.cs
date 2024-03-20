using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Update()
    {
        lastMoveSign = controller.MoveSign;
        animator.SetInteger("moveDirection", lastMoveSign);

        timer += Time.deltaTime;
        if (timer >= fraction && lastMoveSign!=0)
        {
            timer = 0;

            crest.Rotate(Vector3.forward, angleStep);
        }

    }
}
