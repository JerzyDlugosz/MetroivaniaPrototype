using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretBossRoom : BossRoom
{
    [SerializeField]
    private SecretBoss secretBoss;

    [SerializeField]
    private List<GameObject> wallTilemaps;
    [SerializeField]
    private MapBorderCollision rightBorderCollision;
    [SerializeField]
    private MapBorderCollision downBorderCollision;
    [SerializeField]
    private Transform platforms;
    [SerializeField]
    private HiddenWall hiddenWall;
    [SerializeField]
    private Transform playerJumpPosition;


    public override void OnBossRoomEnter()
    {
        base.OnBossRoomEnter();

        GameManagerScript.instance.player.playerShooting.forceMultiplier = 1.3f;
        if (GameManagerScript.instance.player.progressTracker.CheckBossID(secretBoss.bossData))
        {
            Destroy(secretBoss.gameObject);
            //Destroy(bossEnterTrigger.gameObject);
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
        secretBoss.SetAlpha(0f);
        rightBorderCollision.GetComponent<Thorns>().isEnabled = true;
        rightBorderCollision.isManuallyDisabled = true;
        secretBoss.gameObject.SetActive(true);
        secretBoss.onNPCDeath.AddListener(OnBossFightEnd);
        GameStateManager.instance.audioManager.ChangeAudio(bossMusic);
        secretBoss.FadeIn();
        secretBoss.canAttack = false;
        platforms.DOLocalMoveY(0, 2f);
        platforms.GetComponent<CustomPlatformParent>().SetColliderState(false);
        platforms.DOLocalMoveY(0, 2f).OnComplete(() =>
        {
            platforms.GetComponent<CustomPlatformParent>().SetColliderState(true);
        });

        secretBoss.transform.DOLocalMoveY(6, 4f).SetEase(Ease.Linear).OnComplete(() =>
        {
            secretBoss.canAttack = true;
        });
        //AdditionalOnBossFightStart();
    }

    private void OnBossFightEnd()
    {
        StopAllCoroutines();
        DOTween.KillAll(false);
        //MoveBossDoor();
        GameManagerScript.instance.player.playerShooting.forceMultiplier = 1f;
        GameStateManager.instance.audioManager.RemoveAudio();
        GameStateManager.instance.audioManager.musicAudioSource.PlayOneShot(VictoryMusic);
        //AdditionalOnBossFightEnd();
        GameManagerScript.instance.player.ChangeInput(InputMode.None);
        GameManagerScript.instance.cameraHolder.transform.DOShakePosition(3, 1).SetEase(Ease.Linear).OnComplete(() => 
        {
            GameManagerScript.instance.player.transform.DOMoveX(playerJumpPosition.position.x, 2).SetEase(Ease.Linear);
            GameManagerScript.instance.player.transform.DOMoveY(playerJumpPosition.position.y, 2).SetEase(Ease.InBack);


            GameManagerScript.instance.cameraHolder.transform.DOShakePosition(3, 1).SetEase(Ease.Linear);
            downBorderCollision.GetComponent<Thorns>().isEnabled = false;
            downBorderCollision.isManuallyDisabled = false;
            wallTilemaps[2].SetActive(false);
            platforms.gameObject.SetActive(false);
        });
    }

    //public void MoveBossDoor()
    //{
    //    door.transform.DOLocalMoveY(doorHideMoveAmmount, 1f);
    //}

    public void ChangeWallTilemap1()
    {
        wallTilemaps[0].SetActive(false);
        wallTilemaps[1].SetActive(true);
    }

    public void ChangeWallTilemap2()
    {
        wallTilemaps[1].SetActive(false);
        wallTilemaps[2].SetActive(true);
        StartCoroutine(timedBossFightStart());
    }

    IEnumerator timedBossFightStart()
    {
        yield return new WaitForSeconds(2f);
        GameManagerScript.instance.player.HiddenWallMask.transform.DOScale(12, 4).OnComplete(DisableMask);
        OnBossFightStart();
        
    }

    public void DisableMask()
    {
        hiddenWall.gameObject.SetActive(false);
        GameManagerScript.instance.player.HiddenWallMask.transform.localScale = new Vector3(1, 1, 1);
    }
}
