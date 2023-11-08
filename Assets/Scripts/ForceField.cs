using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    [SerializeField, Range(0,1)]
    private float forceFieldSpeedReduction;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponentInParent<Player>().inForceField = true;
            collision.GetComponentInParent<Player>().inForceFieldModifier = forceFieldSpeedReduction;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponentInParent<Player>().inForceField = false;
            collision.GetComponentInParent<Player>().inForceFieldModifier = 1;
        }
    }
}
