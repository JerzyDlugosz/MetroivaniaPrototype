using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdBossRoom : BossRoom
{
    [SerializeField]
    private BirdBoss birdBoss;
    public override void OnBossRoomEnter()
    {
        base.OnBossRoomEnter();

        //GameManagerScript.instance.player.playerShooting.forceMultiplier = 2f;
        if (GameManagerScript.instance.player.progressTracker.CheckBossID(birdBoss.bossData))
        {
            Destroy(birdBoss.gameObject);
            Destroy(bossEnterTrigger.gameObject);
            return;
        }
    }

    public void OnBossFightStart()
    {
        door.transform.DOLocalMoveX(0, 1f);
        birdBoss.gameObject.SetActive(true);
        birdBoss.onNPCDeath.AddListener(OnBossFightEnd);
    }

    private void OnBossFightEnd()
    {
        StopAllCoroutines();
        DOTween.KillAll(false);
        door.transform.DOLocalMoveX(doorHideMoveAmmount, 1f);

        GameManagerScript.instance.player.playerShooting.forceMultiplier = 1f;
    }
}
