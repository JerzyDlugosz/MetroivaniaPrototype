using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class AdditionalEventsOnTriggerEnter : UnityEvent { }

public class MovingWalls : BaseEntity
{

    public AdditionalEventsOnTriggerEnter additionalEvents;
    [SerializeField]
    private Transform movingWall;
    private List<Transform> waypoints = new List<Transform>();
    [SerializeField]
    private float speed;

    [SerializeField]
    private bool isLinear = true;
    [SerializeField]
    private bool isCircular = false;
    [SerializeField]
    private float circularRotationTime;
    [SerializeField]
    private bool reversedRotation = false;
    private int reversedRotationInt = 1;

    private Sequence sequence;

    [SerializeField]
    private bool startMovementOnTriggerEnter = false;
    [SerializeField]
    private bool isOneShotRunning = false;

    [SerializeField]
    private bool parentPlayer = true;

    public override void Start()
    {
        base.Start();
        stoppedEvent.AddListener(OnStop);
        if (reversedRotation)
        {
            reversedRotationInt = -1;
        }

        if(startMovementOnTriggerEnter)
        {
            return;
        }

        if (isLinear)
        {
            waypoints.Clear();
            foreach (Transform child in transform.GetChild(1).transform)
            {
                waypoints.Add(child);
            }
            LinearMovement();
        }
        if (isCircular)
        {
            CircularMovement();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(startMovementOnTriggerEnter && !isOneShotRunning)
        {
            if (collision.CompareTag("Player"))
            {
                waypoints.Clear();
                foreach (Transform child in transform.GetChild(1).transform)
                {
                    waypoints.Add(child);
                }
                LinearOneShotMovement();
                additionalEvents.Invoke();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (parentPlayer)
        {
            if (collision.CompareTag("Player"))
            {
                collision.transform.parent.SetParent(movingWall);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (parentPlayer)
        {
            if (collision.CompareTag("Player"))
            {
                collision.transform.parent.SetParent(null);
            }
        }
    }

    public void ManualMovementStart()
    {
        waypoints.Clear();
        foreach (Transform child in transform.GetChild(1).transform)
        {
            waypoints.Add(child);
        }
        LinearOneShotMovement();
        additionalEvents.Invoke();
    }

    public void ResetLinearOneShotMovement()
    {
        sequence.Kill();
        isOneShotRunning = false;
        movingWall.transform.localPosition = waypoints[0].localPosition;
    }

    public void StartMovementAsAGroup()
    {
        waypoints.Clear();
        foreach (Transform child in transform.GetChild(1).transform)
        {
            waypoints.Add(child);
        }
        LinearOneShotMovement();
    }

    private void LinearOneShotMovement()
    {
        isOneShotRunning = true;
        sequence = DOTween.Sequence();
        int j = 0;
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            j = (i + 1) % waypoints.Count;
            sequence.Append(movingWall.DOLocalMove(waypoints[j].localPosition, Vector2.Distance(waypoints[i].localPosition, waypoints[j].localPosition) / speed).SetEase(Ease.Linear));
        }
    }

    private void LinearMovement()
    {
        sequence = DOTween.Sequence();
        int j = 0;
        for (int i = 0; i < waypoints.Count; i++)
        {
            j = (i + 1) % waypoints.Count;
            sequence.Append(movingWall.DOLocalMove(waypoints[j].localPosition, Vector2.Distance(waypoints[i].localPosition, waypoints[j].localPosition) / speed).SetEase(Ease.Linear));
        }
        sequence.SetLoops(-1);
    }

    private void CircularMovement()
    {
        transform.DOLocalRotate(new Vector3(0, 0, -360f * reversedRotationInt), circularRotationTime / speed, RotateMode.FastBeyond360).SetRelative().SetEase(Ease.Linear).SetLoops(-1);
        movingWall.DOLocalRotate(new Vector3(0, 0, 360f * reversedRotationInt), circularRotationTime / speed, RotateMode.FastBeyond360).SetRelative().SetEase(Ease.Linear).SetLoops(-1);
    }

    private void OnDestroy()
    {
        sequence.Kill();
        transform.DOKill();
        movingWall.DOKill();
    }

    private void OnStop(bool state)
    {
        if(state)
        {
            sequence.Pause();
            transform.DOPause();
            movingWall.DOPause();
        }
        else
        {
            sequence.Play();
            transform.DOPlay();
            movingWall.DOPlay();
        }
    }
}
