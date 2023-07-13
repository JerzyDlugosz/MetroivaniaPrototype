using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomSpriteAnimation : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> sprites;
    [SerializeField]
    private float animationSpeed;
    private float animationFrame = 0f;
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    public void UpdateAnimationFrame()
    {
        animationFrame += Time.deltaTime * animationSpeed;
        if ((int)animationFrame > sprites.Count - 1)
        {
            animationFrame = 0f;
        }

        spriteRenderer.sprite = sprites[(int)animationFrame];
    }

    public void UpdateAnimationFrame(float xVelocity)
    {
        animationFrame += Mathf.Abs(xVelocity) * animationSpeed;
        if ((int)animationFrame > sprites.Count - 1)
        {
            animationFrame = 0f;
        }

        spriteRenderer.sprite = sprites[(int)animationFrame];
    }
}
