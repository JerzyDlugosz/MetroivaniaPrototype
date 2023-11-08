using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    private CameraData cameraData;
    private Vector2 centerOfCamera;

    private int xSizeMul = 1;
    private int ySizeMul = 1;

    public float smoothTime = 0.3F;

    [SerializeField]
    private bool disableCameraBoundaries;

    public bool stopCamera = false;

    public Image blackout;
    

    private void Start()
    {
        cameraData = GetComponent<CameraData>();
        blackout.DOFade(0, 0.5f);

    }

    private void LateUpdate()
    {
        if(!stopCamera)
        {
            SmoothFollowPlayer();
            //FollowPlayer();
            if (!disableCameraBoundaries)
                CheckBoundaries();
        }
    }

    public void SnapCameraPosition()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
    }

    void FollowPlayer()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
    }

    void SmoothFollowPlayer()
    {
        Vector3 velocity = Vector3.zero;
        Vector3 targetPosition = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    void CheckBoundaries()
    {
        float cameraXPos = transform.position.x;
        float cameraYPos = transform.position.y;

        Vector2Int CameraXBoundary = new Vector2Int(-(int)(GlobalData.maxTimemaps - 16) / 2, (int)(GlobalData.maxTimemaps - 16) / 2);
        Vector2Int CameraYBoundary = new Vector2Int((-(int)(GlobalData.maxTimemaps - 16) / 2) - 1, ((int)(GlobalData.maxTimemaps - 16) / 2) + 1);

        float maxA = CameraXBoundary.x - ((GlobalData.maxTimemaps / 2) * (xSizeMul - 1)) + centerOfCamera.x + cameraData.CameraXBoundaryAdditionalOffset.x;
        float minA = CameraXBoundary.y + ((GlobalData.maxTimemaps / 2) * (xSizeMul - 1)) + centerOfCamera.x + cameraData.CameraXBoundaryAdditionalOffset.y;
        float maxB = CameraYBoundary.x - ((GlobalData.maxTimemaps / 2) * (ySizeMul - 1)) + centerOfCamera.y + cameraData.CameraYBoundaryAdditionalOffset.x;
        float minB = CameraYBoundary.y + ((GlobalData.maxTimemaps / 2) * (ySizeMul - 1)) + centerOfCamera.y + cameraData.CameraYBoundaryAdditionalOffset.y;

        Vector3 pos = new Vector3(
            Mathf.Max(maxA, Mathf.Min(minA, cameraXPos)),
            Mathf.Max(maxB, Mathf.Min(minB, cameraYPos)),
            transform.position.z);

        transform.position = pos;
    }

    public void MoveCamera(Vector2 targetPosition)
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        //transform.position = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
    }

    public void SetCameraCenter(Vector2 offset)
    {
        centerOfCamera = offset;
    }

    public void SetCameraBoundary(int _xSizeMul, int _ySizeMul)
    {
        xSizeMul = _xSizeMul;
        ySizeMul = _ySizeMul;
    }

    public void MoveCameraBetweenZones(Vector2 targetPosition)
    {
        


        //stopCamera = true;

        //Vector3 temp1 = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        //Vector3 temp2 = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);

        //transform.DOMove(temp1 + temp2, moveSpeed).onComplete = ResumeCameraMovement;
    }

    public void ResumeCameraMovement()
    {
        stopCamera = false;
    }

    public void ShakeCamera(float time, float strenght)
    {
        transform.DOShakePosition(time, strenght);
    }
}
