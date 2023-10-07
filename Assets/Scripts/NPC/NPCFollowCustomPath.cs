using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFollowCustomPath : MonoBehaviour
{
    [SerializeField]
    private List<Transform> waypoints;
    [SerializeField]
    private BaseNPC baseNPC;
    [SerializeField]
    private int currentWaypoint;

    private void Start()
    {
        currentWaypoint = 0;

        TweenMovement();
    }

    private void TweenMovement()
    {
        Vector3 targetWaypoint = new Vector3(waypoints[currentWaypoint].position.x, waypoints[currentWaypoint].position.y, transform.position.z);
        Transform tempTransform = waypoints[currentWaypoint];

        transform.DOMove(targetWaypoint, Vector2.Distance(transform.position, tempTransform.position) / 2 / baseNPC.speed).OnComplete(() =>
        {
            SetNextWaypoint();
        }).SetEase(Ease.Linear);
    }

    private void SetNextWaypoint()
    {
        currentWaypoint += 1;
        currentWaypoint %= waypoints.Count;
        TweenMovement();
    }
}
