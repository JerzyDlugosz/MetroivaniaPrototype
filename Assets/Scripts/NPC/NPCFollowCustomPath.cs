using DG.Tweening;
using DG.Tweening.Core.Easing;
using Pathfinding;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class OnDestinationReached : UnityEvent { }


public class NPCFollowCustomPath : MonoBehaviour
{
    [SerializeField]
    private List<Transform> Waypoints;
    [SerializeField]
    private Rigidbody2D rb;
    [HideInInspector]
    public SnakeBossEnemy bossEnemy;

    public OnDestinationReached onDestinationReached;

    public bool canMove;

    [SerializeField]
    private float distanceRange = 0.1f;
    [SerializeField]
    private float moveSpeed = 1f;

    public float moveSpeedMultiplier = 1f;

    private Sequence mySequence;
    [SerializeField]
    private GameObject snakeProjectile;

    private CompositeEnemy compositeEnemy;

    protected static readonly int InvincibilityID = Shader.PropertyToID("_InvincibilityStrength");

    

    private void Start()
    {
        compositeEnemy = GetComponentInParent<CompositeEnemy>();
    }

    #region AstarAi
    [SerializeField]
    AIDestinationSetter aiDestination;
    [SerializeField]
    AIPath aiPath;

    public bool usesAstar = false;
    #endregion

    public void StartMovement(bool isLeader, bool useAstar)
    {
        bossEnemy = GetComponent<SnakeBossEnemy>();
        if (false)
        {
            //if (isLeader)
            //{
            //    AstarMovementLeader();
            //}
            //else
            //{
            //    AstarMovementFollower();
            //}

        }
        else
        {
            if (isLeader)
            {
                SetNextWaypoint();
                RotateTowardsWaypoint(Waypoints[compositeEnemy.currentWaypoint]);
                TweenMovementLeader();
            }
            else
            {
                RotateTowardsWaypoint(Waypoints[compositeEnemy.currentWaypoint]);
                TweenMovementFollower();
            }
        }
    }

    //private void TweenMovementLeader()
    //{
    //    mySequence = DOTween.Sequence(transform);
    //    int nextWaypoint = 1;

    //    foreach (var waypoint in Waypoints)
    //    {
    //        if (nextWaypoint > Waypoints.Count - 1)
    //        {
    //            nextWaypoint = 0;
    //        }

    //        Vector3 temp = new Vector3(waypoint.position.x, waypoint.position.y, transform.position.z);
    //        Transform tempTransform = Waypoints[nextWaypoint];

    //        mySequence.Append(transform.DOMove(temp, moveSpeed).OnComplete(() => {
    //            RotateTowardsWaypoint(tempTransform);
    //            onDestinationReached.Invoke();
    //            AttackPattern1();
    //        }).SetEase(Ease.Linear));

    //        nextWaypoint++;
    //    }

    //    mySequence.SetLoops(-1);

    //    mySequence.OnStepComplete(() => {
    //        Debug.Log("Ay");
    //        //mySequence.Kill(transform);
    //    });
    //}


    public void StartNextMovement()
    {
        TweenMovementLeader();
    }

    private void TweenMovementLeader()
    {
        Vector3 temp = new Vector3(Waypoints[compositeEnemy.currentWaypoint].position.x, Waypoints[compositeEnemy.currentWaypoint].position.y, transform.position.z);
        Transform tempTransform = Waypoints[compositeEnemy.currentWaypoint];
        RotateTowardsWaypoint(tempTransform);

        transform.DOMove(temp, Vector2.Distance(transform.position, tempTransform.position) / 2 / (moveSpeed * moveSpeedMultiplier)).OnComplete(() =>
        {
            SetNextWaypoint();
            GetComponentInParent<CompositeEnemy>().StopAndStartMovement(2f);
            AttackLogic();
        }).SetEase(Ease.Linear);
    }

    private void SetNextWaypoint()
    {
        int nextWaypoint;

        nextWaypoint = compositeEnemy.currentWaypoint;

        int temp2 = UnityEngine.Random.Range(1, Waypoints.Count - 1);
        nextWaypoint = (nextWaypoint + temp2) % Waypoints.Count;

        compositeEnemy.currentWaypoint = nextWaypoint;
    }

    //private void TweenMovementFollower()
    //{
    //    mySequence = DOTween.Sequence(transform);
    //    int nextWaypoint = 1;

    //    foreach (var waypoint in Waypoints)
    //    {
    //        if (nextWaypoint > Waypoints.Count - 1)
    //        {
    //            nextWaypoint = 0;
    //        }

    //        Vector3 temp = new Vector3(waypoint.position.x, waypoint.position.y, transform.position.z);
    //        Transform tempTransform = Waypoints[nextWaypoint];

    //        mySequence.Append(transform.DOMove(temp, moveSpeed).OnComplete(() => {
    //            RotateTowardsWaypoint(tempTransform);
    //        }).SetEase(Ease.Linear));
    //        nextWaypoint++;
    //    }

    //    mySequence.SetLoops(-1);

    //    mySequence.OnStepComplete(() => {
    //        Debug.Log("Ay");
    //        //mySequence.Kill(transform);
    //    });
    //}

    private void TweenMovementFollower()
    {
        Vector3 temp = new Vector3(Waypoints[compositeEnemy.currentWaypoint].position.x, Waypoints[compositeEnemy.currentWaypoint].position.y, transform.position.z);
        Transform tempTransform = Waypoints[compositeEnemy.currentWaypoint];
        RotateTowardsWaypoint(tempTransform);

        transform.DOMove(temp, Vector2.Distance(transform.position, tempTransform.position) / 2 / (moveSpeed * moveSpeedMultiplier)).OnComplete(() => {
            TweenMovementFollower();
        }).SetEase(Ease.Linear);
    }

    //private void AstarMovementLeader()
    //{
    //    onDestinationReached.AddListener(() => GetComponentInParent<CompositeEnemy>().StopAndStartMovement());
    //    aiDestination.target = Waypoints[0];
    //    nextWaypoint = 1;
    //    compositeEnemy.currentWaypoint = 0;
    //    usesAstar = true;
    //}
    //private void AstarMovementFollower()
    //{
    //    aiDestination.target = Waypoints[0];
    //    nextWaypoint = 1;
    //    usesAstar = true;
    //}

    private void Update()
    {
        //if (usesAstar)
        //{
        //    if (compositeEnemy.currentWaypoint > Waypoints.Count - 1)
        //    {
        //        compositeEnemy.currentWaypoint = 0;
        //    }
        //    Debug.Log(Vector2.Distance(transform.position, Waypoints[compositeEnemy.currentWaypoint].position));
        //    if (Vector2.Distance(transform.position, Waypoints[compositeEnemy.currentWaypoint].position) < distanceRange)
        //    {
        //        if (nextWaypoint > Waypoints.Count - 1)
        //        {
        //            nextWaypoint = 0;
        //        }
        //        Debug.Log("Reached Target");
        //        aiDestination.target = Waypoints[nextWaypoint];
        //        onDestinationReached.Invoke();
        //        nextWaypoint++;
        //        compositeEnemy.currentWaypoint++;
        //    }

        //    if(transform.rotation.z > 0f && transform.rotation.z < 180f)
        //    {
        //        customEnemy.spriteRenderer.flipY = true;
        //    }
        //    else
        //    {
        //        customEnemy.spriteRenderer.flipY = false;
        //    }
        //}
    }

    private void RotateTowardsWaypoint(Transform waypoint)
    {
        float angle = Mathf.Atan2(waypoint.position.y - transform.position.y, waypoint.position.x - transform.position.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.rotation = targetRotation;

        if(MathF.Abs(angle) > 90 && MathF.Abs(angle) < 270)
        {
            bossEnemy.spriteRenderer.flipY = true;
        }
        else
        {
            bossEnemy.spriteRenderer.flipY = false;
        }
    }

    public void MovementState(bool state)
    {
        Debug.Log(state);
        canMove = state;
       //aiPath.canMove = state;
        if (state == false)
        {
            StopTween();
        }
        else
        {
            StartTween();
        }
    }

    public void Invisibility(bool state)
    {
        if(state)
        {
            bossEnemy.spriteRenderer.material.SetFloat(InvincibilityID, 1);
            bossEnemy.isInvincible = true;
        }
        else
        {
            bossEnemy.spriteRenderer.material.SetFloat(InvincibilityID, 0);
            bossEnemy.isInvincible = false;
        }
    }

    public void StopTween()
    {
        DOTween.Pause(transform);
        bossEnemy.spriteAnimation.stopAnimation = true;
    }

    public void StartTween()
    {
        DOTween.Play(transform);
        bossEnemy.spriteAnimation.stopAnimation = false;
    }

    private void AttackLogic()
    {
        AttackPattern3();
    }

    private void AttackPattern1()
    {
        int projectileAmmount = 5;
        float projectileSpread = 15;

        for (int i = 0; i < projectileAmmount; i++)
        {
            float projectileRotation = -(projectileSpread * (projectileAmmount - 1) / 2);

            Vector3 position = new Vector3(transform.position.x, transform.position.y, 0f);

            Transform playerTransform = GameManagerScript.instance.player.transform;

            float angle = Mathf.Atan2(playerTransform.position.y - transform.position.y, playerTransform.position.x - transform.position.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle+ (projectileRotation + (projectileSpread * i))));
            transform.rotation = targetRotation;

            GameObject projectile = Instantiate(snakeProjectile, position, targetRotation);

            projectile.GetComponent<Projectile>().OnInstantiate();

        }
    }

    private void AttackPattern2()
    {
        int projectileAmmount = 1;
        float projectileSpread = 0;

        for (int i = 0; i < projectileAmmount; i++)
        {
            float projectileRotation = -(projectileSpread * (projectileAmmount - 1) / 2);

            Vector3 position = new Vector3(transform.position.x, transform.position.y, 0f);

            Transform playerTransform = GameManagerScript.instance.player.transform;

            float angle = Mathf.Atan2(playerTransform.position.y - transform.position.y, playerTransform.position.x - transform.position.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle + (projectileRotation + (projectileSpread * i))));
            transform.rotation = targetRotation;

            GameObject projectile = Instantiate(snakeProjectile, position, targetRotation);

            projectile.GetComponent<Projectile>().OnInstantiate();
            projectile.GetComponent<Projectile>().wallCollisionEvent.AddListener(() => SpreadExplosion(projectile.transform));

        }
    }

    private void AttackPattern3()
    {
        int projectileAmmount = 3;
        float projectileSpread = 30;

        for (int i = 0; i < projectileAmmount; i++)
        {
            float projectileRotation = -(projectileSpread * (projectileAmmount - 1) / 2);

            Vector3 position = new Vector3(transform.position.x, transform.position.y, 0f);

            Transform playerTransform = GameManagerScript.instance.player.transform;

            float angle = Mathf.Atan2(playerTransform.position.y - transform.position.y, playerTransform.position.x - transform.position.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle + (projectileRotation + (projectileSpread * i))));
            transform.rotation = targetRotation;

            GameObject projectileGameObject = Instantiate(snakeProjectile, position, targetRotation);
            Projectile projectile = projectileGameObject.GetComponent<Projectile>();

            projectile.OnInstantiate();

            projectile.SetBounceAngle(0);
        }
    }

    private void SpreadExplosion(Transform projectileTransform)
    {
        int projectileAmmount = 8;
        float projectileSpread = 30;

        for (int i = 0; i < projectileAmmount; i++)
        {
            float projectileRotation = -(projectileSpread * (projectileAmmount - 1) / 2);

            Vector3 position = new Vector3(projectileTransform.position.x, projectileTransform.position.y, 0f);

            Transform playerTransform = GameManagerScript.instance.player.transform;

            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, 90 + (projectileRotation + (projectileSpread * i))));
            transform.rotation = targetRotation;

            GameObject projectile = Instantiate(snakeProjectile, position, targetRotation);

            projectile.GetComponent<Projectile>().OnInstantiate();
            projectile.transform.localScale= new Vector3(0.4f,0.4f,1);
            projectile.GetComponent<HostileProjectile>().StopCollision(0.1f);
        }
    }
}
