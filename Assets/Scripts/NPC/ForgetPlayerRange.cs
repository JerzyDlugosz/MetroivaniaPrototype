using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgetPlayerRange : MonoBehaviour
{
    [SerializeField]
    private AIDestinationSetter destinationSetter;
    [SerializeField]
    private bool isStationary;
    private Transform baseTransform;
    private GameObject basePos;

    private void Start()
    {
        basePos = new GameObject("basePos");
        basePos.transform.position = transform.position;
    }

    public void OnDestroy()
    {
        Destroy(basePos);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (isStationary)
            {
            }
            else
            {
                destinationSetter.target = basePos.transform;
                destinationSetter.GetComponent<AIPath>().endReachedDistance = 0.2f;
            }
            
        }
    }
}
