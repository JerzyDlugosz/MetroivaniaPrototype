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
    public bool oneShot = false;

    private int oneShotCount = -1;

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

        if(oneShot)
            oneShotCount = 0;
    }

    private void Update()
    {
        if (automaticAnimationLoop)
        {
            UpdateAnimationFrame();

            if(oneShot)
            {
                if (oneShotCount < (int)animationFrame + 1)
                    oneShotCount = (int)animationFrame + 1;

                if (oneShotCount >= sprites.Count)
                    Destroy(gameObject);
            }
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
