using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PBumerangArrow : PlayerProjectile
{
    [SerializeField]
    private Collider2D platformCollider;
    [SerializeField]
    private float bumerangForce;

    private Vector2 opposingForceMagnitude;

    public override void Start()
    {
        base.Start();
        wallCollisionEvent.AddListener(() => projectileParticleController.OnCollision());
        recallEvent.AddListener(() => projectileParticleController.OnRecall());
        endOfLifetimeEvent.AddListener(() => projectileParticleController.OnBreak());

        wallCollisionEvent.AddListener(OnWallCollision);
        wallBounceEvent.AddListener(OnWallCollision);
        beforeEndOfLifetimeEvent.AddListener(() => ShakeArrow());
    }

    private void FixedUpdate()
    {
        if(MainUpdate())
        {
            rb.AddForce(new Vector2(-opposingForceMagnitude.x, 0f) * bumerangForce);
        }
    }

    private void Update()
    {
        if (MainUpdate())
        {
            if (isStuckInWall)
            {
                if (projectileLifetime < 1 && !isShaking)
                {
                    isShaking = true;
                    beforeEndOfLifetimeEvent.Invoke();
                }
            }
        }

        //Bumerang with only opposing x force;
        //rb.AddForce(new Vector2(-opposingForceMagnitude.x, 0f) * bumerangForce);
        //Bumerang with all opposing forces;
        //rb.AddForce(-opposingForceMagnitude * bumerangForce);
    }

    public void OnWallCollision()
    {
        rb.bodyType = RigidbodyType2D.Static;
        projectileLifetime += 5f;
        platformCollider.transform.localRotation = Quaternion.Euler(0f, 0f, -gameObject.transform.localEulerAngles.z);
        platformCollider.enabled = true;
        isStuckInWall = true;
        GameStateManager.instance.audioManager.effectsAudioSoruce.PlayOneShot(OnWallCollisionAudio);
    }

    public override void OnInstantiate(float angle, float distance, float multiplier, float _damageModifier)
    {
        base.OnInstantiate(angle, distance, 1f, _damageModifier);

        opposingForceMagnitude = MathExtensions.GetAngleMagnitude(angle, true);
        Debug.Log($"{opposingForceMagnitude.x},{opposingForceMagnitude.y}");
    }
}
