using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingPool : MonoBehaviour
{
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponentInParent<Player>().RestoreHealth(1);
        }
    }
}
