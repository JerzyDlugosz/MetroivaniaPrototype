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
        GameManagerScript.instance.player.playerShooting.forceMultiplier = 1.3f;
        if (GameManagerScript.instance.player.progressTracker.CheckBossID(bossEnemyParts.bossData))
        {
            Destroy(bossEnemyParts.gameObject);
            Destroy(bossEnterTrigger.gameObject);
            GameManagerScript.instance.player.playerShooting.forceMultiplier = 1f;
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
            GameStateManager.instance.audioManager.ChangeAudio(bossMusic);
        }
    }
    private void OnBossFightEnd()
    {
        StopAllCoroutines();
        DOTween.KillAll(false);
        door.transform.DOLocalMoveY(doorHideMoveAmmount, 1f);
        GameStateManager.instance.audioManager.RemoveAudio();
        GameStateManager.instance.audioManager.musicAudioSource.PlayOneShot(VictoryMusic);
        GameManagerScript.instance.entitiesManager.RemoveAllEntities();
    }

}
