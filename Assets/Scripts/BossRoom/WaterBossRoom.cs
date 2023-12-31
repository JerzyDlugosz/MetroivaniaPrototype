using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaterBossRoom : BossRoom
{
    [SerializeField]
    private ShootableButton shootableButton;
    [SerializeField]
    private WaterBossEnemy bossEnemy;
    [SerializeField]
    private float stunTime = 6f;

    [SerializeField]
    private List<GameObject> obstacleTilemaps;
    [SerializeField]
    private Transform obstaclesHolder;
    [SerializeField]
    private GameObject platform;

    [SerializeField]
    private Transform playerPushbackPosition;
    [SerializeField]
    private Transform bossStunPosition;

    [SerializeField]
    private Transform waterGameobject;

    private GameObject currentObstacle;
    private bool isInWater = false;

    public override void OnBossRoomEnter()
    {
        base.OnBossRoomEnter();

        if (GameManagerScript.instance.player.progressTracker.CheckBossID(bossEnemy.bossData))
        {
            Destroy(bossEnemy.gameObject);
            Destroy(bossEnterTrigger);
            Destroy(shootableButton.gameObject);
            //Destroy(waterGameobject.gameObject);
            return;
        }

    }

    private void HideObstacle()
    {
        currentObstacle.GetComponent<Collider2D>().enabled = false;
        GameManagerScript.instance.cameraHolder.DOShakePosition(1.5f, 1);
        shootableButton.gameObject.SetActive(false);
        obstaclesHolder.transform.DOLocalMoveY(-7, 2).OnComplete(() => 
        {
            currentObstacle.SetActive(false);
        });
        platform.SetActive(true);

        waterGameobject.DOLocalMoveY(-5f, 2f);
    }

    public void ShowObstacle()
    {
        GetRandomObstacle();
        GameManagerScript.instance.cameraHolder.DOShakePosition(1.5f, 1);
        shootableButton.gameObject.SetActive(true);

        currentObstacle.GetComponent<Collider2D>().enabled = true;
        currentObstacle.SetActive(true);
        obstaclesHolder.DOLocalMoveY(0, 2);

        platform.SetActive(false);
    }

    public void OnButtonHit()
    {
        StartCoroutine(WaterFillTimer(stunTime));
    }

    IEnumerator WaterFillTimer(float time)
    {
        //Hide water and show platforms
        bossEnemy.Invincibility(false);

        bossEnemy.OnStun(true);
        HideObstacle();
        bossEnemy.transform.DOLocalRotate(new Vector3(0, 0, 0), 1.5f);
        bossEnemy.transform.DOMove(bossStunPosition.position, 2f);

        bossEnemy.neckSpriteRenderer.sprite = bossEnemy.onStunSprites[0];

        yield return new WaitForSeconds(1);

        bossEnemy.neckSpriteRenderer.sprite = bossEnemy.onStunSprites[1];


        yield return new WaitForSeconds(time - 2);


        bossEnemy.RemoveAllDebuffs();

        //Show water and hide platforms
        bossEnemy.transform.DOLocalRotate(new Vector3(0, 0, -90), 1.5f);
        bossEnemy.transform.DOMove(bossEnemy.headPositions[0].position, 2f);

        GameManagerScript.instance.player.characterController.StopMovement(true);

        waterGameobject.transform.localPosition = new Vector3(84, 12, -0.5f);
        waterGameobject.transform.DOLocalMoveX(24, 4);

        bossEnemy.neckSpriteRenderer.sprite = bossEnemy.onStunSprites[2];

        yield return new WaitForSeconds(1);

        bossEnemy.neckSpriteRenderer.sprite = bossEnemy.onStunSprites[3];

        yield return new WaitForSeconds(1);

        bossEnemy.neckSpriteRenderer.sprite = bossEnemy.neckSprites[0];

    }

    private void GetRandomObstacle()
    {
        int rand = Random.Range(0, obstacleTilemaps.Count);
        currentObstacle = obstacleTilemaps[rand];
    }

    private IEnumerator LerpObstacleAlpha(Tilemap obstacleTilemap, bool fade)
    {
        float t = 0;
        Color color;

        if (fade)
        {
            do
            {
                color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), t);
                obstacleTilemap.color = color;
                t += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            } while (t < 1);
        }
        else
        {
            do
            {
                color = Color.Lerp(new Color(1, 1, 1, 0), Color.white, t);
                obstacleTilemap.color = color;
                t += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            } while (t < 1);
        }

        obstacleTilemap.gameObject.SetActive(!fade);
    }

    public void OnBossFightStart()
    {
        bossEnemy.SetAlpha(0f);
        ShowObstacle();
        door.transform.DOLocalMoveY(0, 1f);
        bossEnemy.gameObject.SetActive(true);
        bossEnemy.Invincibility(true);
        bossEnemy.onNPCDeath.AddListener(OnBossFightEnd);
        GameStateManager.instance.audioManager.ChangeAudio(bossMusic);

        bossEnemy.FadeIn();
    }

    private void OnBossFightEnd()
    {
        StopAllCoroutines();
        DOTween.KillAll(false);
        door.transform.DOLocalMoveY(doorHideMoveAmmount, 1f);
        shootableButton.gameObject.SetActive(false);
        GameStateManager.instance.audioManager.RemoveAudio();
        GameStateManager.instance.audioManager.musicAudioSource.PlayOneShot(VictoryMusic);
    }

    public void OnWaterTriggerEnter()
    {
        if(!isInWater)
        {
            isInWater = true;
            GameManagerScript.instance.player.transform.DOMove(playerPushbackPosition.position, 2f).OnComplete(() => {
                ShowObstacle();
                GameManagerScript.instance.player.characterController.StopMovement(false);
                bossEnemy.Invincibility(true);
                bossEnemy.OnStun(false);
                isInWater = false;
            }).SetEase(Ease.Linear);
        }
    }
}
