using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    private CameraMovement cameraMovement;
    private void Start()
    {
        cameraMovement = FindObjectOfType<CameraMovement>();
    }

    public void SmoothCameraMoveToTarget(Transform target)
    {
        //Temp Fix
        Vector2 vec = new Vector2(24, 24);

        cameraMovement.transform.DOMoveY(vec.y, 1f);
        cameraMovement.transform.DOMoveX(vec.x, 1f).OnComplete(() =>
        {
            cameraMovement.stopCamera = false;
        });
    }
}
