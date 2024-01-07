using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAttackCollision : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log("hit: " + collision.name);
        if (collision.CompareTag("Player"))
        {
            //Debug.Log("HIT!");
            collision.GetComponentInParent<Player>().damageTakenEvent.Invoke(GetComponentInParent<BaseNPC>().damage);
        }
    }
}
