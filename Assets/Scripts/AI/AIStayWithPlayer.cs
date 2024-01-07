using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AIStayWithPlayer : MonoBehaviour
{
    private AIPath AiPath;
    [SerializeField]
    private ShowText showText;

    private bool isTalking = false;
    private bool finishedTalking = false;

    private bool isPlayerNearby = false;

    private float xDistance;
    private void Awake()
    {
        if (AiPath == null)
        {
            AiPath = GetComponentInParent<AIPath>();
        }
    }

    private void Start()
    {
        if(showText != null)
        {
            showText.inGameText.onTimeElapsed.AddListener(EnableMovement);
        }
        else
        {
            EnableMovement();
        }
    }

    private void Update()
    {
        if(isPlayerNearby && finishedTalking)
        {
            xDistance = GameManagerScript.instance.player.transform.position.x - transform.position.x;
            if (xDistance > -2f)
            {
                AiPath.canMove = true;
            }
        }
    }

    private void EnableMovement()
    {
        finishedTalking = true;
        isTalking = false;

        GetComponent<Collider2D>().enabled = false;
        GetComponent<Collider2D>().enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        AiPath.enabled = true;
        isPlayerNearby = true;

        if (!finishedTalking)
        {
            isTalking = true;
            showText.TextShow();
        }

        if (!isTalking)
        {
            //Debug.Log($"Player entered {transform.parent.name}");

            if (collision.CompareTag("Player"))
                AiPath.canMove = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isTalking)
        {
            if (!AiPath.canMove)
            {
                if (collision.CompareTag("Player"))
                    AiPath.canMove = true;
            }
            //Debug.Log($"Player near {transform.parent.name}");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isTalking)
        {
            //Debug.Log($"Player escaped {transform.parent.name}");

            if (collision.CompareTag("Player"))
                AiPath.canMove = false;
        }
    }
}
