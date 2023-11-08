using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField]
    private Sprite offSprite;

    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private BoxCollider2D jumpPadCollider;
    [SerializeField]
    private CustomSpriteAnimation spriteAnimation;
    [SerializeField]
    private float additionalJumpPadModifier = 1f;

    public bool activeOnStart = false;
    private void Start()
    {
        jumpPadCollider.enabled = false;
        if (activeOnStart)
        {
            SwitchState(true);
        }
    }


    public void SwitchState(bool state)
    {
        spriteAnimation.stopAnimation = !state;
        if (state)
        {

        }
        else
        {
            spriteRenderer.sprite = offSprite;
        }

        jumpPadCollider.enabled = state;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.GetComponentInParent<Player>().onJumpPad = true;
            collision.GetComponentInParent<Player>().characterController.additionalJumpPadModifier = additionalJumpPadModifier;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponentInParent<Player>().onJumpPad = false;
            collision.GetComponentInParent<Player>().characterController.additionalJumpPadModifier = 1f;
        }
    }
}
