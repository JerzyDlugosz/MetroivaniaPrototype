using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderCollision : MonoBehaviour
{
    [SerializeField]
    private Player player;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Ladder"))
        {
            player.onLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            player.onLadder = false;
        }
    }
}
