using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomEnemy : MonoBehaviour
{
    [SerializeField]
    private AIPath path;
    public Rigidbody2D enemyRigidbody;
    public CustomAiTargeter customAiTargeter;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private CustomSpriteAnimation spriteAnimation;
    [SerializeField]
    private FacingSide facingSide;
    [SerializeField]
    private bool isUsingVelocityForAnimation = false;

    private void Update()
    {
        UpdateSpriteRotation();
        if(!isUsingVelocityForAnimation)
        {
            spriteAnimation.UpdateAnimationFrame();
        }
        else
        {
            if(enemyRigidbody != null)
            {
                spriteAnimation.UpdateAnimationFrame(enemyRigidbody.velocity.x);
                return;
            }
            spriteAnimation.UpdateAnimationFrame(path.velocity.x);
        }
    }

    void UpdateSpriteRotation()
    {
        if (enemyRigidbody != null)
        {
            if (enemyRigidbody.velocity.x * (int)facingSide > 0f)
            {
                if (spriteRenderer.flipX)
                {
                    spriteRenderer.flipX = false;
                }
            }
            if (enemyRigidbody.velocity.x * (int)facingSide < 0f)
            {
                if (!spriteRenderer.flipX)
                {
                    spriteRenderer.flipX = true;
                }
            }
            return;
        }
            
        if (path.desiredVelocity.x * (int)facingSide > 0f)
        {
            if (spriteRenderer.flipX)
            {
                spriteRenderer.flipX = false;
            }
        }
        if (path.desiredVelocity.x * (int)facingSide < 0f)
        {
            if (!spriteRenderer.flipX)
            {
                spriteRenderer.flipX = true;
            }
        }
    }
}

public enum FacingSide
{
    Right = 1,
    Left = -1,
}
