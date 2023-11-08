using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTrapController : MonoBehaviour
{
    [SerializeField]
    private List<LaserTrap> laserTraps;
    [SerializeField]
    private float laserStartOffset;
    [SerializeField]
    private float laserAttackDelay;

    private void Start()
    {
        for (int i = 0; i < laserTraps.Count; i++)
        {
            laserTraps[i].SetData(laserAttackDelay, laserStartOffset + (i * 0.2f));
        }
    }

    public void LaserState(bool state)
    {
        if(state)
        {
            StartLasers();
        }
        else
        {
            StopLasers();
        }    
    }

    private void StartLasers()
    {
        foreach (var item in laserTraps)
        {
            item.StartLaserCoroutine();
        }
    }

    private void StopLasers()
    {
        foreach (var item in laserTraps)
        {
            item.StopLaserCoroutine();
        }
    }

    public void DisableAllLasers()
    {
        foreach (var item in laserTraps)
        {
            item.laser.gameObject.SetActive(false);
        }
    }
}
