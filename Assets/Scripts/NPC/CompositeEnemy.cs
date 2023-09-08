using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CompositeEnemy : MonoBehaviour
{
    public List<NPCFollowCustomPath> enemyParts = new List<NPCFollowCustomPath>();

    public int currentWaypoint = 0;

    public float compositeEnemyHealth = 100;

    public float compositeEnemyMaxHealth = 100;

    public BossData bossData;



    private void Start()
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

    IEnumerator Timer(float time)
    {


        foreach (var part in enemyParts)
        {
            part.Invisibility(false);
            part.MovementState(false);
        }
        yield return new WaitForSeconds(time);

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
