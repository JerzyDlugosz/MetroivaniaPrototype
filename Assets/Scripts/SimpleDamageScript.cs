using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDamageScript : MonoBehaviour
{
    public float damage;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponentInParent<Player>().damageTakenEvent.Invoke(damage);
        }
    }
}
