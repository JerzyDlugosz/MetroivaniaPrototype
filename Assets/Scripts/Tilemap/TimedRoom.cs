using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TimedRoom : MonoBehaviour
{
    [SerializeField]
    private GameObject onPlatforms;
    [SerializeField]
    private GameObject offPlatforms;

    private bool platformState;

    [SerializeField]
    private float roomTime;
    private float remainingRoomTime = 0;
    private bool puzzleState = false;
    [SerializeField]
    private GameObject doors;
    [SerializeField]
    private float doorMovementAmmount;

    private Coroutine platformCoroutine;

    [SerializeField]
    private GameObject clockHand;

    public void OnActivate()
    {
        platformCoroutine = StartCoroutine(TimedPlatforms());
        puzzleState = true;
        remainingRoomTime = roomTime;
        doors.transform.DOLocalMoveY(doorMovementAmmount, 1f);
        clockHand.transform.DOLocalRotate(new Vector3(0, 0, -360f), roomTime, RotateMode.FastBeyond360).SetRelative().SetEase(Ease.Linear);
    }

    private void Update()
    {
        if(puzzleState)
        {
            if (remainingRoomTime > 0)
            {
                remainingRoomTime -= Time.deltaTime;
            }
            else
            {
                OnDeactivate();
            }
        }
    }

    public void OnDeactivate()
    {
        StopCoroutine(platformCoroutine);
        doors.transform.DOLocalMoveY(0, 1f);
        clockHand.transform.rotation = Quaternion.identity;
        onPlatforms.SetActive(false);
        offPlatforms.SetActive(false);
        puzzleState = false;

    }

    public void OnFinish()
    {
        StopCoroutine(platformCoroutine);

        onPlatforms.SetActive(true);
        offPlatforms.SetActive(true);
    }

    private void SwitchPlatforms(bool state)
    {
        onPlatforms.SetActive(state);
        offPlatforms.SetActive(!state);
    }

    private void PlatformFlash(bool state)
    {
        onPlatforms.GetComponent<TilemapRenderer>().enabled = state;
        offPlatforms.GetComponent<TilemapRenderer>().enabled = state;
    }

    IEnumerator TimedPlatforms()
    {
        do
        {
            platformState = !platformState;
            SwitchPlatforms(platformState);
            yield return new WaitForSeconds(1.2f);
            PlatformFlash(false);
            yield return new WaitForSeconds(0.2f);
            PlatformFlash(true);
            yield return new WaitForSeconds(0.2f);
            PlatformFlash(false);
            yield return new WaitForSeconds(0.2f);
            PlatformFlash(true);
        } while (true);
    }
}
