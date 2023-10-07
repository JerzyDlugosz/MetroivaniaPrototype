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
    }

    private void OnBossFightEnd()
    {
        StopAllCoroutines();
        DOTween.KillAll(false);
        door.transform.DOLocalMoveY(doorHideMoveAmmount, 1f);
        playerPush = false;
        GameManagerScript.instance.player.playerShooting.forceMultiplier = 1f;
    }

    private void Update()
    {
        if(playerPush)
            playerController.ForceApplyForce(pushForce);
    }
}
