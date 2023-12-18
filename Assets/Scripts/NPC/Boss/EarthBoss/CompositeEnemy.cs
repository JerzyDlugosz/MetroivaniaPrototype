using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompositeEnemy : MonoBehaviour
{
    public List<SnakeFollowPath> enemyParts = new List<SnakeFollowPath>();

    public int currentWaypoint = 0;

    public float compositeEnemyHealth = 100;

    public float compositeEnemyMaxHealth = 100;

    public BossData bossData;


    public void StartEncounter()
    {
        StartCoroutine(StartBossMovement());
    }

    IEnumerator StartBossMovement()
    {
        yield return new WaitForSeconds(0.2f);
        enemyParts[0].StartMovement(true, false);
        enemyParts[0].MovementState(true);

        for (int i = 1; i < enemyParts.Count; i++)
        {
            yield return new WaitForSeconds(0.2f);
            enemyParts[i].StartMovement(false, false);
            enemyParts[i].MovementState(true);
        }  
    }

    public void StopAndStartMovement(float time)
    {
        StartCoroutine(Timer(time));
    }

    public void MovementState(bool state)
    {
        foreach (var part in enemyParts)
        {
            part.Invisibility(state);
            part.MovementState(state);
        }
    }

    IEnumerator Timer(float time)
    {


        foreach (var part in enemyParts)
        {
            part.Invisibility(false);
            part.MovementState(false);
        }
        yield return new WaitForSeconds(time);

        if (enemyParts[0] == null)
            yield break;

        enemyParts[0].StartNextMovement();

        foreach (var part in enemyParts)
        {
            part.MovementState(true);
            part.Invisibility(true);
        }
        if (Vector2.Distance(enemyParts[0].transform.position, enemyParts[1].transform.position) > 0.9f)
        {
            enemyParts[0].MovementState(false);
            yield return new WaitForSeconds(0.1f);
            enemyParts[0].MovementState(true);
        }
    }

    public void OnCompositeEnemyDeath()
    {
        GameManagerScript.instance.player.progressTracker.AddBoss(bossData);
    }
}
