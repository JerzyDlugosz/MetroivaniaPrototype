using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonBossRoom : BossRoom
{
    [SerializeField]
    private DragonComposite dragonComposite;
    [SerializeField]
    private GameObject Ladder;
    public override void OnBossRoomEnter()
    {
        base.OnBossRoomEnter();

        GameManagerScript.instance.player.playerShooting.forceMultiplier = 2f;
        if (GameManagerScript.instance.player.progressTracker.CheckBossID(dragonComposite.bossData))
        {
            Destroy(dragonComposite.gameObject);
            Destroy(bossEnterTrigger.gameObject);
            Ladder.SetActive(true);
            GameManagerScript.instance.player.playerShooting.forceMultiplier = 1f;
            return;
        }
    }

    public void OnBossFightStart()
    {
        dragonComposite.SetAlpha(0f);
        door.transform.DOLocalMoveY(0, 1f);
        dragonComposite.gameObject.SetActive(true);
        dragonComposite.dragonParts[0].onNPCDeath.AddListener(OnBossFightEnd);
        GameStateManager.instance.audioManager.ChangeAudio(bossMusic);
        dragonComposite.FadeIn();
    }

    private void OnBossFightEnd()
    {
        StopAllCoroutines();
        DOTween.KillAll(false);
        door.transform.DOLocalMoveY(doorHideMoveAmmount, 1f);

        GameManagerScript.instance.player.playerShooting.forceMultiplier = 1f;
        GameStateManager.instance.audioManager.RemoveAudio();
        GameStateManager.instance.audioManager.musicAudioSource.PlayOneShot(VictoryMusic);
        Ladder.SetActive(true);
    }
}
