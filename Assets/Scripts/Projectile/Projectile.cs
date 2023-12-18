using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class OnWallCollisionEvent : UnityEvent { }

[Serializable]
public class OnWallBounceEvent : UnityEvent { }

[Serializable]
public class OnEntityCollisionEvent : UnityEvent<BaseNPC> { }

[Serializable]
public class OnEndOfLifetimeEvent : UnityEvent { }

public class Projectile : BaseEntity
{
    public WeaponType projectileType;
    [SerializeField]
    protected Vector2 projectileVelocity;
    [SerializeField]
    protected Vector2 projectileForce;
    [SerializeField]
    protected float projectilePushBack;
    [SerializeField]
    protected string projectileName;
    [SerializeField]
    protected float projectileLifetime;
    [SerializeField]
    protected float bounceAngle;
    [SerializeField]
    protected float projectileDamage;
    [SerializeField]
    protected float inWaterDrag;
    [SerializeField]
    protected float baseGravity;

    protected float damageModifier = 1;

    protected float baseDrag;

    public int projectileStrenght;
    public int projectileStickiness;

    [SerializeField]
    protected ProjectileParticleController projectileParticleController;

    public float projectileAttackSpeed;
    [SerializeField]
    protected Rigidbody2D rb;

    protected ContactPoint2D[] contacts = new ContactPoint2D[10];

    [HideInInspector]
    public OnWallCollisionEvent wallCollisionEvent;
    [HideInInspector]
    public OnWallBounceEvent wallBounceEvent;
    [HideInInspector]
    public OnEntityCollisionEvent entityCollisionEvent;
    [HideInInspector]
    public OnEndOfLifetimeEvent endOfLifetimeEvent;

    public void SetBounceAngle(float value)
    {
        bounceAngle = value;
    }

    public void Bounce()
    {
        Debug.Log("Rb vel: " + rb.velocity);
        Vector2 vel2 = Vector2.Reflect(rb.velocity, Vector2.up);
        Debug.Log("RB vel2: " + vel2);
        rb.velocity = vel2;
    }

    public virtual void OnInstantiate(float angle, float distance, float multiplier, float _damageModifier)
    {

    }
    public virtual void OnInstantiate()
    {

    }
    public virtual void OnInstantiate(float gravity)
    {
        rb.gravityScale = gravity;
    }

    protected void WaterCheck()
    {
        if (inWater)
        {
            rb.gravityScale = baseGravity / 2;
            rb.drag = inWaterDrag;
        }
        else
        {
            rb.gravityScale = baseGravity;
            rb.drag = baseDrag;
        }
    }


    protected void debugRays()
    {
        Debug.DrawRay(transform.position, rb.velocity, Color.white);
        Debug.DrawRay(transform.position, transform.forward, Color.red);
    }

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            DebugStuff(collision);

            int contactsLength = collision.GetContacts(contacts);

            for (int i = 0; i < contactsLength; i++)
            {
                float angle = Vector2.Angle(-(Vector2)transform.right, collision.contacts[i].normal);
                Debug.Log(angle);
                if (angle < bounceAngle)
                {
                    if(collision.collider.TryGetComponent(out WallData wallData))
                    {
                        if(wallData.wallStickiness <= projectileStickiness)
                        {
                            wallCollisionEvent.Invoke();
                            break;
                        }
                        else
                        {
                            endOfLifetimeEvent.Invoke();
                            break;
                        }
                    }
                    wallCollisionEvent.Invoke();
                    break;
                }
                else
                {
                    Debug.Log("bounce");
                    wallBounceEvent.Invoke();
                    break;
                }
            }
        }
        AdditionalOnCollisionEnter(collision);
    }

    protected virtual void AdditionalOnCollisionEnter(Collision2D collision)
    {

    }

    private void DebugStuff(Collision2D collision)
    {
        // Print how many points are colliding with this transform
        Debug.Log("Points colliding: " + collision.contacts.Length);

        // Print the normal of the first point in the collision.
        Debug.Log("Normal of the first point: " + collision.contacts[0].normal);

        // Draw a different colored ray for every normal in the collision
        foreach (var item in collision.contacts)
        {
            Debug.DrawRay(item.point, item.normal * 100, UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), 10f);
        }
    }
}
