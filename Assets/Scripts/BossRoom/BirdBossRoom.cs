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

        //OnDropPickupEvent.AddListener(() => { MoveBossDoor(); });

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
        door.transform.DOLocalMoveY(0, 1f);
        birdBoss.gameObject.SetActive(true);
        birdBoss.onNPCDeath.AddListener(OnBossFightEnd);
        GameStateManager.instance.audioManager.ChangeAudio(bossMusic);
    }

    private void OnBossFightEnd()
    {
        StopAllCoroutines();
        DOTween.KillAll(false);
        MoveBossDoor();
        GameManagerScript.instance.player.playerShooting.forceMultiplier = 1f;
        GameStateManager.instance.audioManager.RemoveAudio();
        GameStateManager.instance.audioManager.musicAudioSource.PlayOneShot(VictoryMusic);
    }

    public void MoveBossDoor()
    {
        door.transform.DOLocalMoveY(doorHideMoveAmmount, 1f);
    }
}
