using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EyeLaserEnemy : BaseNPC
{
    [SerializeField]
    private Transform reticle;

    public float moveSpeed = 0.2f;

    [SerializeField]
    private Collider2D laserCollider;
    [SerializeField]
    private GameObject laserPivot;

    private bool currentState = false;
    private bool previousState = false;

    private float baseZRotation;

    public override void Start()
    {
        base.Start();
        onNPCHit.AddListener(OnHit);
        onNPCDeath.AddListener(OnDeath);

        stoppedEvent.AddListener(OnStop);
        baseZRotation = transform.eulerAngles.z;
    }

    private void FixedUpdate()
    {
        if (isStopped)
        {
            return;
        }

        if(canAttack)
        {
            var angle = MathExtensions.GetAngle(transform.position, GameManagerScript.instance.player.transform.position);
            var aimRotation = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.LerpAngle(transform.eulerAngles.z, angle, moveSpeed));
            transform.eulerAngles = aimRotation;
            UpdateReticlePosition(GameManagerScript.instance.player.transform.position);

            currentState = true;
            if(previousState != currentState)
            {
                laserCollider.enabled = true;
                previousState = true;
                laserPivot.SetActive(true);
            }
        }
        else
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.LerpAngle(transform.eulerAngles.z, baseZRotation, moveSpeed));

            currentState = false;
            if (previousState != currentState)
            {
                laserCollider.enabled = false;
                previousState = false;
                laserPivot.SetActive(false);
            }
        }
    }

    void UpdateReticlePosition(Vector3 playerPos)
    {
        var direction = (transform.right - transform.position).normalized;
        var laserDirection = (playerPos - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 50f, LayerMask.GetMask("Wall"));


        var distance = Vector2.Distance(transform.position, hit.point);
        laserPivot.transform.localScale = new Vector3(distance, 1f, 1f);

        if (hit.collider != null)
        {
            reticle.position = hit.point;
        }
        else
        {
            reticle.position = playerPos;
        }
    }

    public void OnStop(bool state)
    {
        path.canMove = !state;
    }

    private void OnHit(float damage)
    {
        health -= damage;

        if (health <= 0f)
        {
            onNPCDeath.Invoke();
        }

        StartCoroutine(DamageTimer());
    }

    public void OnDeath()
    {
        enemyParticleController.OnDeath();
    }
}
