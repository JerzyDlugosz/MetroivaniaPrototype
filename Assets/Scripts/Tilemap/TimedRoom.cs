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
        if(puzzleState)
        {
            return;
        }
        platformCoroutine = StartCoroutine(TimedPlatforms());
        puzzleState = true;
        remainingRoomTime = roomTime;
        doors.transform.DOLocalMoveY(doorMovementAmmount, 1f);
        //clockHand.transform.DOLocalRotate(new Vector3(0, 0, -360f), roomTime, RotateMode.FastBeyond360).SetRelative().SetEase(Ease.Linear);
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
        onPlatforms.transform.GetChild(0).gameObject.SetActive(state);
        offPlatforms.transform.GetChild(0).gameObject.SetActive(!state);
        if(!state)
        {
            offPlatforms.GetComponent<Tilemap>().color = new Color(onPlatforms.GetComponent<Tilemap>().color.r, onPlatforms.GetComponent<Tilemap>().color.g, onPlatforms.GetComponent<Tilemap>().color.b, 1f);
            onPlatforms.GetComponent<Tilemap>().color = new Color(onPlatforms.GetComponent<Tilemap>().color.r, onPlatforms.GetComponent<Tilemap>().color.g, onPlatforms.GetComponent<Tilemap>().color.b, 0.1f);
        }
        else
        {
            offPlatforms.GetComponent<Tilemap>().color = new Color(onPlatforms.GetComponent<Tilemap>().color.r, onPlatforms.GetComponent<Tilemap>().color.g, onPlatforms.GetComponent<Tilemap>().color.b, 0.1f);
            onPlatforms.GetComponent<Tilemap>().color = new Color(onPlatforms.GetComponent<Tilemap>().color.r, onPlatforms.GetComponent<Tilemap>().color.g, onPlatforms.GetComponent<Tilemap>().color.b, 1f);

        }


    }

    private void PlatformFlash(bool state)
    {
        if(!platformState)
        {
            if (state)
            {
                onPlatforms.GetComponent<Tilemap>().color = new Color(onPlatforms.GetComponent<Tilemap>().color.r, onPlatforms.GetComponent<Tilemap>().color.g, onPlatforms.GetComponent<Tilemap>().color.b, 0.1f);
                offPlatforms.GetComponent<Tilemap>().color = new Color(offPlatforms.GetComponent<Tilemap>().color.r, offPlatforms.GetComponent<Tilemap>().color.g, offPlatforms.GetComponent<Tilemap>().color.b, 1f);

            }
            else
            {
                onPlatforms.GetComponent<Tilemap>().color = new Color(onPlatforms.GetComponent<Tilemap>().color.r, onPlatforms.GetComponent<Tilemap>().color.g, onPlatforms.GetComponent<Tilemap>().color.b, 0.4f);
                offPlatforms.GetComponent<Tilemap>().color = new Color(offPlatforms.GetComponent<Tilemap>().color.r, offPlatforms.GetComponent<Tilemap>().color.g, offPlatforms.GetComponent<Tilemap>().color.b, 0.7f);

            }
        }
        else
        {
            if (state)
            {
                offPlatforms.GetComponent<Tilemap>().color = new Color(offPlatforms.GetComponent<Tilemap>().color.r, offPlatforms.GetComponent<Tilemap>().color.g, offPlatforms.GetComponent<Tilemap>().color.b, 0.1f);
                onPlatforms.GetComponent<Tilemap>().color = new Color(onPlatforms.GetComponent<Tilemap>().color.r, onPlatforms.GetComponent<Tilemap>().color.g, onPlatforms.GetComponent<Tilemap>().color.b, 1f);

            }
            else
            {
                offPlatforms.GetComponent<Tilemap>().color = new Color(offPlatforms.GetComponent<Tilemap>().color.r, offPlatforms.GetComponent<Tilemap>().color.g, offPlatforms.GetComponent<Tilemap>().color.b, 0.4f);
                onPlatforms.GetComponent<Tilemap>().color = new Color(onPlatforms.GetComponent<Tilemap>().color.r, onPlatforms.GetComponent<Tilemap>().color.g, onPlatforms.GetComponent<Tilemap>().color.b, 0.7f);

            }
        }


        //onPlatforms.GetComponent<TilemapRenderer>().enabled = state;
        //offPlatforms.GetComponent<TilemapRenderer>().enabled = state;
    }


    private void HidePlatform(bool state)
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
            yield return new WaitForSeconds(0.2f);
            PlatformFlash(false);
            yield return new WaitForSeconds(0.2f);
            PlatformFlash(true);
        } while (true);
    }
}
