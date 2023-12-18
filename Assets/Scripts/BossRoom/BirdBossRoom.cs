using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class BirdBossRoom : BossRoom
{
    [SerializeField]
    private BirdBoss birdBoss;
    public override void OnBossRoomEnter()
    {
        base.OnBossRoomEnter();

        GameManagerScript.instance.player.playerShooting.forceMultiplier = 1.3f;
        if (GameManagerScript.instance.player.progressTracker.CheckBossID(birdBoss.bossData))
        {
            Destroy(birdBoss.gameObject);
            Destroy(bossEnterTrigger.gameObject);
            GameManagerScript.instance.player.playerShooting.forceMultiplier = 1f;
            return;
        }
    }

    //public void AdditionalOnBossFightStart()
    //{
    //    CameraData cameraData = GameManagerScript.instance.player.mainCamera.GetComponent<CameraData>();
    //    cameraData.CameraXBoundaryAdditionalOffset = new Vector2(16,-16);
    //    cameraData.CameraYBoundaryAdditionalOffset = new Vector2(16, -16);

    //    GameManagerScript.instance.player.mainCamera.GetComponent<PixelPerfectCamera>().assetsPPU = assetsPPU;
    //}

    //public void AdditionalOnBossFightEnd()
    //{
    //    CameraData cameraData = GameManagerScript.instance.player.mainCamera.GetComponent<CameraData>();
    //    cameraData.CameraXBoundaryAdditionalOffset = new Vector2(11, -16);
    //    cameraData.CameraYBoundaryAdditionalOffset = new Vector2(16, -16);

    //    GameManagerScript.instance.player.mainCamera.GetComponent<PixelPerfectCamera>().assetsPPU = assetsPPU;
    //}

    public void OnBossFightStart()
    {
        birdBoss.SetAlpha(0f);
        door.transform.DOLocalMoveY(0, 1f);
        birdBoss.gameObject.SetActive(true);
        birdBoss.onNPCDeath.AddListener(OnBossFightEnd);
        GameStateManager.instance.audioManager.ChangeAudio(bossMusic);
        birdBoss.FadeIn();
        //AdditionalOnBossFightStart();
    }

    private void OnBossFightEnd()
    {
        StopAllCoroutines();
        DOTween.KillAll(false);
        MoveBossDoor();
        GameManagerScript.instance.player.playerShooting.forceMultiplier = 1f;
        GameStateManager.instance.audioManager.RemoveAudio();
        GameStateManager.instance.audioManager.musicAudioSource.PlayOneShot(VictoryMusic);
        //AdditionalOnBossFightEnd();
    }

    public void MoveBossDoor()
    {
        door.transform.DOLocalMoveY(doorHideMoveAmmount, 1f);
    }
}
