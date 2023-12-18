using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendlyNPC : BaseNPC
{
    [SerializeField]
    private bool isUsingVelocityForAnimation = false;

    public override void Start()
    {
        base.Start();
        stoppedEvent.AddListener(OnStop);
    }

    private void Update()
    {
        if (isStopped)
        {
            return;
        }
        UpdateSpriteDirection(false);
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

    public void OnStop(bool state)
    {
        path.canMove = !state;
    }

    private void OnDestinationReached()
    {
        gameObject.GetComponent<SpriteRenderer>().DOKill();
        Destroy(gameObject);
    }
}
