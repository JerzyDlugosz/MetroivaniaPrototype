using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableProjectile : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Projectile"))
        {
            //Debug.Log("Works");
            Destroy(gameObject);
        }
    }
}
