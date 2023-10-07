using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonBossRoom : BossRoom
{
    [SerializeField]
    private DragonComposite dragonComposite;
    public override void OnBossRoomEnter()
    {
        base.OnBossRoomEnter();

        GameManagerScript.instance.player.playerShooting.forceMultiplier = 2f;
        if (GameManagerScript.instance.player.progressTracker.CheckBossID(dragonComposite.dragonParts[0].bossData))
        {
            Destroy(dragonComposite.gameObject);
            Destroy(bossEnterTrigger.gameObject);
            return;
        }
    }

    public void OnBossFightStart()
    {
        door.transform.DOLocalMoveY(0, 1f);
        dragonComposite.gameObject.SetActive(true);
        dragonComposite.dragonParts[0].onNPCDeath.AddListener(OnBossFightEnd);
    }

    private void OnBossFightEnd()
    {
        StopAllCoroutines();
        DOTween.KillAll(false);
        door.transform.DOLocalMoveY(doorHideMoveAmmount, 1f);

        GameManagerScript.instance.player.playerShooting.forceMultiplier = 1f;
    }
}
