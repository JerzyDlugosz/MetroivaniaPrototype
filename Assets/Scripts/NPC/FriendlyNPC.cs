using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyNPC : BaseNPC
{
    [SerializeField]
    private bool isUsingVelocityForAnimation = false;

    private void Update()
    {
        UpdateSpriteRotation(false);
        if (!isUsingVelocityForAnimation)
        {
            spriteAnimation.UpdateAnimationFrame();
        }
        else
        {
            if (NPCRigidbody != null)
            {
                spriteAnimation.UpdateAnimationFrame(NPCRigidbody.velocity.x);
                return;
            }
            spriteAnimation.UpdateAnimationFrame(path.velocity.x);
        }

        if (path.reachedEndOfPath)
        {
            gameObject.GetComponent<SpriteRenderer>().DOFade(0f, 1f).onComplete += OnDestinationReached;
        }
    }

    private void OnDestinationReached()
    {
        gameObject.GetComponent<SpriteRenderer>().DOKill();
        Destroy(gameObject);
    }
}
