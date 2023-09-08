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
    private SpriteRenderer spriteRenderer;

    public bool stopAnimation = false;
    public bool automaticAnimationLoop = false;

    private void Start()
    {
        if(TryGetComponent(out BaseNPC baseNPC))
        {
            spriteRenderer = baseNPC.spriteRenderer;
        }
        else
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void Update()
    {
        if (automaticAnimationLoop)
        {
            UpdateAnimationFrame();
        }
    }

    public void UpdateAnimationFrame()
    {
        if (!stopAnimation)
        {
            animationFrame += Time.deltaTime * animationSpeed;
            if ((int)animationFrame > sprites.Count - 1)
            {
                animationFrame = 0f;
            }

            spriteRenderer.sprite = sprites[(int)animationFrame];
        }
    }

    public void UpdateAnimationFrame(float xVelocity)
    {
        if (!stopAnimation)
        {
            animationFrame += Mathf.Abs(xVelocity) * animationSpeed;
            if ((int)animationFrame > sprites.Count - 1)
            {
                animationFrame = 0f;
            }

            spriteRenderer.sprite = sprites[(int)animationFrame];
        }
    }
}
