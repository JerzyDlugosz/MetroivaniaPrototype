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

    private void Start()
    {
        wallCollisionEvent.AddListener(() => projectileParticleController.OnCollision());
        recallEvent.AddListener(() => projectileParticleController.OnRecall());

        wallCollisionEvent.AddListener(OnWallCollision);
        wallBounceEvent.AddListener(OnWallCollision);

        endOfLifetimeEvent.AddListener(() => recallEvent.Invoke());
    }

    private void Update()
    {
        MainUpdate();
        //Bumerang with only opposing x force;
        rb.AddForce(new Vector2(-opposingForceMagnitude.x, 0f) * bumerangForce);
        //Bumerang with all opposing forces;
        //rb.AddForce(-opposingForceMagnitude * bumerangForce);
    }

    public void OnWallCollision()
    {
        rb.bodyType = RigidbodyType2D.Static;
        projectileLifetime += 5f;
        platformCollider.transform.localRotation = Quaternion.Euler(0f, 0f, -gameObject.transform.localEulerAngles.z);
        platformCollider.enabled = true;
    }

    public override void OnInstantiate(float angle)
    {
        base.OnInstantiate(angle);

        opposingForceMagnitude = MathExtensions.GetAngleMagnitude(angle, true);
        Debug.Log($"{opposingForceMagnitude.x},{opposingForceMagnitude.y}");
    }
}
