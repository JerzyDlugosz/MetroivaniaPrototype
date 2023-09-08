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
    private GameObject platform;

    [SerializeField]
    private Transform playerPushbackPosition;
    [SerializeField]
    private Transform bossStunPosition;
    [SerializeField]
    private Transform bossAttackPosition;

    [SerializeField]
    private Transform waterGameobject;

    private GameObject currentObstacle;

    public override void OnBossRoomEnter()
    {
        base.OnBossRoomEnter();

        if (GameManagerScript.instance.player.progressTracker.CheckBossID(bossEnemy.bossData))
        {
            Destroy(bossEnemy.gameObject);
            Destroy(bossEnterTrigger.gameObject);
            //Destroy(waterGameobject.gameObject);
            return;
        }

        shootableButton.onButtonHit.AddListener(OnButtonHit);
    }

    private void HideObstacle()
    {
        currentObstacle.GetComponent<Collider2D>().enabled = false;
        currentObstacle.transform.DOLocalMoveY(-7, 2).OnComplete(() => 
        {
            currentObstacle.SetActive(false);
        });
        platform.SetActive(true);

        waterGameobject.DOLocalMoveY(-5f, 2f);
    }

    public void ShowObstacle()
    {
        GetRandomObstacle();

        currentObstacle.GetComponent<Collider2D>().enabled = true;
        currentObstacle.SetActive(true);
        currentObstacle.transform.DOLocalMoveY(0, 2);

        platform.SetActive(false);
    }

    private void OnButtonHit()
    {
        StartCoroutine(WaterFillTimer(stunTime));
    }

    IEnumerator WaterFillTimer(float time)
    {
        //Hide water and show platforms
        bossEnemy.Invisibility(false);

        //bossEnemy.OnStun(time + 3f);
        bossEnemy.OnStun(true);
        HideObstacle();
        bossEnemy.transform.DOLocalRotate(new Vector3(0, 0, 0), 1.5f);
        bossEnemy.transform.DOMove(bossStunPosition.position, 2f);

        yield return new WaitForSeconds(time);


        //Show water and hide platforms
        bossEnemy.transform.DOLocalRotate(new Vector3(0, 0, -90), 1.5f);
        bossEnemy.transform.DOMove(bossAttackPosition.position, 2f);

        GameManagerScript.instance.player.characterController.StopMovement(true);

        waterGameobject.transform.localPosition = new Vector3(60, 12, -0.5f);
        waterGameobject.transform.DOLocalMoveX(0, 4);
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
        ShowObstacle();
        door.transform.DOLocalMoveY(0, 1f);
        bossEnemy.gameObject.SetActive(true);
        bossEnemy.Invisibility(true);
        bossEnemy.onNPCDeath.AddListener(OnBossFightEnd);
    }

    private void OnBossFightEnd()
    {
        StopAllCoroutines();
        DOTween.KillAll(false);
        door.transform.DOLocalMoveY(doorHideMoveAmmount, 1f);
    }

    public void OnWaterTriggerEnter()
    {

        GameManagerScript.instance.player.transform.DOMove(playerPushbackPosition.position, 2f).OnComplete(() => {
            ShowObstacle();
            GameManagerScript.instance.player.characterController.StopMovement(false);
            bossEnemy.Invisibility(true);
            bossEnemy.OnStun(false);
        }).SetEase(Ease.Linear);
    }
}
