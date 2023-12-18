using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WEyeBoss : EyeBoss
{
    [SerializeField]
    private List<GameObject> ObstaclePrefabs;

    [SerializeField]
    private Transform obstacleSpawnPosition;

    private int randomObstacle;

    public override void Start()
    {
        base.Start();
        onNPCHit.AddListener(OnHit);
        onNPCDeath.AddListener(OnDeath);

        attackCooldown = attackSpeed;
        stoppedEvent.AddListener(OnStop);

        ChooseNextMovementPoint();
        randomObstacle = UnityEngine.Random.Range(0, ObstaclePrefabs.Count);
    }

    private void Update()
    {
        if (isStopped)
            return;

        MovementAndRotationLogic();

        if (attackCooldown > 0)
            attackCooldown -= Time.deltaTime;

        if (attackCooldown <= 0)
        {
            attackCooldown = attackSpeed;
            AttackLogic();
        }

    }

    private void AttackLogic()
    {
        attackCooldown += 4f / eyeComposite.remainingPartsModifier;
        AttackPattern1();
        GameStateManager.instance.audioManager.PlaySoundEffect(onAttackAudioClip);
    }

    private void AttackPattern1()
    {
        Debug.Log(obstacleSpawnPosition.position);
        WEyeBossObstacle wEyeBossObstacle = Instantiate(ObstaclePrefabs[randomObstacle], obstacleSpawnPosition.position, Quaternion.identity).GetComponent<WEyeBossObstacle>();
        wEyeBossObstacle.destroyXPosition = obstacleSpawnPosition.position.x - 42f;
        wEyeBossObstacle.obstacleSpeed = 5f * eyeComposite.remainingPartsModifier;
        int rand = UnityEngine.Random.Range(-1, 2);

        randomObstacle += rand;

        if (randomObstacle < 0)
        {
            randomObstacle = 0;
        }
        if(randomObstacle >= ObstaclePrefabs.Count -1)
        {
            randomObstacle = ObstaclePrefabs.Count - 1;
        }
    }
}
