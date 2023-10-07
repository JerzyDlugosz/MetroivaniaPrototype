using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBossRoom : BossRoom
{
    [SerializeField]
    private CompositeEnemy bossEnemyParts;
    public override void OnBossRoomEnter()
    {
        base.OnBossRoomEnter();

        if (GameManagerScript.instance.player.progressTracker.CheckBossID(bossEnemyParts.bossData))
        {
            Destroy(bossEnemyParts.gameObject);
            Destroy(bossEnterTrigger.gameObject);
            return;
        }
    }

    public void OnBossFightStart()
    {
        door.transform.DOLocalMoveY(0, 1f);

        //bossEnemyParts.gameObject.SetActive(true);
        foreach (var part in bossEnemyParts.enemyParts)
        {
            Debug.Log(part.bossEnemy);
            part.bossEnemy.Invincibility(true);
            part.bossEnemy.onNPCDeath.AddListener(OnBossFightEnd);
        }
    }
    private void OnBossFightEnd()
    {
        StopAllCoroutines();
        DOTween.KillAll(false);
        door.transform.DOLocalMoveY(doorHideMoveAmmount, 1f);
    }

}
