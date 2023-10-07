using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out BaseEntity comp))
        {
            comp.inWater = true;
            return;
        }
        collision.GetComponentInParent<BaseEntity>().inWater = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out BaseEntity comp))
        {
            comp.inWater = false;
            return;
        }
        collision.GetComponentInParent<BaseEntity>().inWater = false;
    }
}
