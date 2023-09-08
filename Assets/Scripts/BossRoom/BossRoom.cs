using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class BossRoom : MonoBehaviour
{
    [SerializeField]
    protected Vector2 CameraXOffset = new Vector2(4, -4);
    [SerializeField]
    protected Vector2 CameraYOffset = new Vector2(4, -4);
    [SerializeField]
    protected int assetsPPU = 20;
    [SerializeField]
    protected GameObject bossEnterTrigger;
    [SerializeField]
    protected GameObject door;
    [SerializeField]
    protected float doorHideMoveAmmount;

    public virtual void OnBossRoomEnter()
    {
        CameraData cameraData = GameManagerScript.instance.player.mainCamera.GetComponent<CameraData>();
        cameraData.CameraXBoundaryAdditionalOffset = CameraXOffset;
        cameraData.CameraYBoundaryAdditionalOffset = CameraYOffset;

        GameManagerScript.instance.player.mainCamera.GetComponent<PixelPerfectCamera>().assetsPPU = assetsPPU;
    }
}
