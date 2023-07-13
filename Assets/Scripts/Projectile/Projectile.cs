using System;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public class OnWallCollisionEvent : UnityEvent { }

[Serializable]
public class OnWallBounceEvent : UnityEvent { }

public class Projectile : MonoBehaviour
{
    [SerializeField]
    protected float projectileSpeed;
    [SerializeField]
    protected float projectileGravity;
    [SerializeField]
    protected Vector2 projectileForce;
    [SerializeField]
    protected string projectileName;
    [SerializeField]
    protected float projectileLifetime;

    public float projectileAttackSpeed;
    [SerializeField]
    protected Rigidbody2D rb;

    public OnWallCollisionEvent collisionEvent;
    public OnWallBounceEvent bounceEvent;

    ContactPoint2D[] contacts = new ContactPoint2D[10];

    public void OnInstantiate(float angle)
    {
        //rb.AddTorque(angle, ForceMode2D.Impulse);
        rb.AddRelativeForce(projectileForce);
    }

    private void Update()
    {
        if(rb.velocity.magnitude > 0)
        {
            transform.right = rb.velocity.normalized;
        }


        projectileLifetime -= Time.deltaTime;
        if(projectileLifetime < 0)
        {
            Destroy(gameObject);
        }
        Debug.DrawRay(transform.position, rb.velocity, Color.white);
        Debug.DrawRay(transform.position, transform.forward, Color.red);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
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

            int contactsLength = collision.GetContacts(contacts);

            for (int i = 0; i < contactsLength; i++)
            {
                float angle = Vector2.Angle(-(Vector2)transform.right, collision.contacts[i].normal);

                if (angle < 20f)
                {
                    collisionEvent.Invoke();
                    break;
                }
                else
                {
                    bounceEvent.Invoke();
                }
            }
        }
    }
}
