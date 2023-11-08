using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseBossRoom : BossRoom
{
    [SerializeField]
    private EyeBossComposite eyeBossComposite;
    [SerializeField]
    private MovingBackground movingBackground;
    private bool playerPush = false;
    [SerializeField]
    private Vector2 pushForce;
    private Custom2DCharacterController playerController;

    private void Start()
    {
        playerController = GameManagerScript.instance.player.characterController;
    }

    public override void OnBossRoomEnter()
    {

        base.OnBossRoomEnter();
        GameManagerScript.instance.player.playerShooting.forceMultiplier = 2f;
        if (GameManagerScript.instance.player.progressTracker.CheckBossID(eyeBossComposite.bossData))
        {
            Destroy(eyeBossComposite.gameObject);
            Destroy(bossEnterTrigger.gameObject);
            return;
        }
    }

    public void OnBossFightStart()
    {
        door.transform.DOLocalMoveY(0, 1f);
        movingBackground.MovementState(true);
        playerPush = true;
        eyeBossComposite.gameObject.SetActive(true);
        eyeBossComposite.compositeEyeBossDeathEvent.AddListener(OnBossFightEnd);
        GameStateManager.instance.audioManager.ChangeAudio(bossMusic);
    }

    private void OnBossFightEnd()
    {
        StopAllCoroutines();
        DOTween.KillAll(false);
        door.transform.DOLocalMoveY(doorHideMoveAmmount, 1f);
        playerPush = false;
        GameManagerScript.instance.player.playerShooting.forceMultiplier = 1f;

        GameEndScreen();
    }

    private void GameEndScreen()
    {
        playerPush = false;
        GameManagerScript.instance.entitiesManager.EntitiesPauseState(true);
        playerController.StopMovement(true);

        //Disable inputs here...

        GameManagerScript.instance.cameraMovement.transform.DOShakePosition(2f, 1f).OnComplete(() => 
        {
            GameManagerScript.instance.cameraMovement.transform.DOShakePosition(2f, 1f).SetLoops(-1);
            float moveAmmount = playerController.transform.position.x + 10;
            float moveAmmount2 = playerController.transform.position.x + 20;
            float animSpeed = 0.01f;
            playerController.transform.DOMoveX(moveAmmount, Vector2.Distance(playerController.transform.position, playerController.transform.position + new Vector3(125, 0, 0)) * animSpeed).SetEase(Ease.InSine).OnComplete(() =>
            {
                playerController.transform.DOMoveX(moveAmmount2, Vector2.Distance(playerController.transform.position, playerController.transform.position + new Vector3(125, 0, 0)) * animSpeed).SetEase(Ease.Linear);
                GameManagerScript.instance.cameraMovement.blackout.DOFade(1, Vector2.Distance(playerController.transform.position, playerController.transform.position + new Vector3(125, 0, 0)) * animSpeed).OnComplete(() => 
                {

                    //... and enable them here maybe
                    GameStateManager.instance.audioManager.RemoveAudio();
                    GameStateManager.instance.audioManager.ChangeAudio(VictoryMusic);
                    GameManagerScript.instance.endScreen.StartAnim();
                });
            });
        });
    }

    private void Update()
    {
        if(playerPush)
            playerController.ForceApplyForce(pushForce);
    }
}
