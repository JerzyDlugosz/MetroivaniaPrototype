using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private Vector2 additionalOffset;

    private void Update()
    {
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
        targetPosition += new Vector3(additionalOffset.x, additionalOffset.y, 0f);
        transform.position = targetPosition;
    }
}
