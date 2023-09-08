using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    private CameraData cameraData;
    private Vector2 centerOfCamera;

    private int xSizeMul = 1;
    private int ySizeMul = 1;

    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private bool disableCameraBoundaries;

    private bool stopCamera = false;

    public SpriteRenderer blackout;
    

    private void Start()
    {
        cameraData = GetComponent<CameraData>();
    }

    private void Update()
    {
        if(!stopCamera)
        {
            FollowPlayer();
            if (!disableCameraBoundaries)
                CheckBoundaries();
        }
    }

    void FollowPlayer()
    {
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
    }

    void CheckBoundaries()
    {
        float cameraXPos = transform.position.x;
        float cameraYPos = transform.position.y;

        Vector2Int CameraXBoundary = new Vector2Int(-(int)(GlobalData.mapSize - 16) / 2, (int)(GlobalData.mapSize - 16) / 2);
        Vector2Int CameraYBoundary = new Vector2Int((-(int)(GlobalData.mapSize - 16) / 2) - 1, ((int)(GlobalData.mapSize - 16) / 2) + 1);

        float maxA = CameraXBoundary.x - ((GlobalData.mapSize / 2) * (xSizeMul - 1)) + centerOfCamera.x + cameraData.CameraXBoundaryAdditionalOffset.x;
        float minA = CameraXBoundary.y + ((GlobalData.mapSize / 2) * (xSizeMul - 1)) + centerOfCamera.x + cameraData.CameraXBoundaryAdditionalOffset.y;
        float maxB = CameraYBoundary.x - ((GlobalData.mapSize / 2) * (ySizeMul - 1)) + centerOfCamera.y + cameraData.CameraYBoundaryAdditionalOffset.x;
        float minB = CameraYBoundary.y + ((GlobalData.mapSize / 2) * (ySizeMul - 1)) + centerOfCamera.y + cameraData.CameraYBoundaryAdditionalOffset.y;

        Vector3 pos = new Vector3(
            Mathf.Max(maxA, Mathf.Min(minA, cameraXPos)),
            Mathf.Max(maxB, Mathf.Min(minB, cameraYPos)),
            transform.position.z);

        transform.position = pos;
    }

    public void MoveCamera(Vector2 targetPosition)
    {
        transform.position = new Vector3(targetPosition.x, targetPosition.y, transform.position.z);
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
}
