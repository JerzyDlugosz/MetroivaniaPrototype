using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomEnemyAi : MonoBehaviour
{
    //private CustomEnemy enemy;
    //private Rigidbody2D enemyRigidbody;
    //private Seeker seeker;

    //[SerializeField]
    //private float distanceToTarget;
    //private Path path;
    //private int currentWaypoint;
    //[SerializeField]
    //private Collider2D col;
    //private int WallLayer;
    //[SerializeField]
    //private float jumpSpeed;
    //[SerializeField]
    //private float speed;
    //[SerializeField]
    //private float targetHeightToJump;
    //[SerializeField]
    //private float nextWaypointDistance = 1f;

    //private void Start()
    //{
    //    WallLayer = LayerMask.NameToLayer("Wall");
    //    enemy = this.GetComponent<CustomEnemy>();
    //    enemyRigidbody = enemy.enemyRigidbody;
    //    seeker = GetComponent<Seeker>();


    //    InvokeRepeating("UpdatePath", 0f, 0.5f);
    //}

    //private void FixedUpdate()
    //{
    //    if(TargetInDistance())
    //    {
    //        FollowPath();
    //    }
    //}

    //private void UpdatePath()
    //{
    //    if(seeker.IsDone())
    //    {
    //        seeker.StartPath(enemyRigidbody.position, enemy.customAiTargeter.target.position, OnPathComplete);
    //    }
    //}

    //private void OnPathComplete(Path p)
    //{
    //    if(!p.error) 
    //    {
    //        path = p;
    //        currentWaypoint = 0;
    //    }
    //}

    //private void FollowPath()
    //{
    //    if(path == null)
    //    {
    //        return;
    //    }

    //    if(currentWaypoint >= path.vectorPath.Count) 
    //    {
    //        return;
    //    }

    //    RaycastHit2D isGrounded = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0, Vector2.down, 0.1f, WallLayer);

    //    Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - enemyRigidbody.position).normalized;
    //    Vector2 force = speed * Time.deltaTime * direction;
        
    //    //Debug.Log($"isGrounded: {isGrounded}");
    //    //Debug.Log($"enemy dir: {direction}");
    //    if (isGrounded.collider != null)
    //    {
    //        Debug.Log("1ayy");
    //        if (direction.y > targetHeightToJump)
    //        {
    //            Debug.Log("2ayy");
    //            enemyRigidbody.AddForce(Vector2.up * jumpSpeed);
    //        }
    //    }

    //    enemyRigidbody.AddForce(force);

    //    float distance = Vector2.Distance(enemyRigidbody.position, path.vectorPath[currentWaypoint]);
    //    if(distance < nextWaypointDistance)
    //    {
    //        currentWaypoint++;
    //    }
    //}

    //private bool TargetInDistance()
    //{
    //    return Vector2.Distance(transform.position, GameManagerScript.instance.player.transform.position) < distanceToTarget;
    //}
}
