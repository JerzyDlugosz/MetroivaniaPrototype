using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class ZoomOutField : MonoBehaviour
{
    [SerializeField]
    protected Vector2 CameraXOffset = new Vector2(4, -4);
    [SerializeField]
    protected Vector2 CameraYOffset = new Vector2(4, -4);
    [SerializeField]
    protected int assetsPPU = 20;
    private CameraData cameraData;
    private PixelPerfectCamera pixelPerfectCamera;
    private float t = 0;

    private Coroutine lerpCoroutine;
    private void Start()
    {
        cameraData = GameManagerScript.instance.player.mainCamera.GetComponent<CameraData>();
        pixelPerfectCamera = GameManagerScript.instance.player.mainCamera.GetComponent<PixelPerfectCamera>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(lerpCoroutine != null)
            StopCoroutine(lerpCoroutine);
        if (collision.CompareTag("Player"))
        {
            lerpCoroutine = StartCoroutine(lerpValues(cameraData.baseCameraXBoundaryAdditionalOffset, CameraXOffset, cameraData.baseCameraYBoundaryAdditionalOffset, CameraYOffset, cameraData.baseAssetsPPU, assetsPPU, 1));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (lerpCoroutine != null)
            StopCoroutine(lerpCoroutine);
        if (collision.CompareTag("Player"))
        {
            lerpCoroutine = StartCoroutine(lerpValues(cameraData.baseCameraXBoundaryAdditionalOffset, CameraXOffset, cameraData.baseCameraYBoundaryAdditionalOffset, CameraYOffset, cameraData.baseAssetsPPU, assetsPPU, -1));

            //lerpCoroutine = StartCoroutine(lerpValues(CameraXOffset, cameraData.baseCameraXBoundaryAdditionalOffset, CameraYOffset, cameraData.baseCameraYBoundaryAdditionalOffset, assetsPPU, cameraData.baseAssetsPPU));
        }
    }

    IEnumerator lerpValues(Vector2 startingCameraXOffset, Vector2 endCameraXOffset, Vector2 startingCameraYOffset, Vector2 endCameraYOffset, int startingPPU, int endPPU, int direction)
    {
        if(direction == 1)
        {
            do
            {
                float xMinOffsetLerp = Mathf.Lerp(startingCameraXOffset.x, endCameraXOffset.x, t);
                float xMaxOffsetLerp = Mathf.Lerp(startingCameraXOffset.y, endCameraXOffset.y, t);

                float yMinOffsetLerp = Mathf.Lerp(startingCameraYOffset.x, endCameraYOffset.x, t);
                float yMaxOffsetLerp = Mathf.Lerp(startingCameraYOffset.y, endCameraYOffset.y, t);

                float PPU = Mathf.Lerp(startingPPU, endPPU, t);



                cameraData.CameraXBoundaryAdditionalOffset = new Vector2(xMinOffsetLerp, xMaxOffsetLerp);
                cameraData.CameraYBoundaryAdditionalOffset = new Vector2(yMinOffsetLerp, yMaxOffsetLerp);

                pixelPerfectCamera.assetsPPU = (int)PPU;


                t += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            } while (t < 1);
            t = 1;

            cameraData.CameraXBoundaryAdditionalOffset = endCameraXOffset;
            cameraData.CameraYBoundaryAdditionalOffset = endCameraYOffset;

            pixelPerfectCamera.assetsPPU = endPPU;
        }
        else
        {
            Debug.LogWarning(t);
            do
            {
                float xMinOffsetLerp = Mathf.Lerp(startingCameraXOffset.x, endCameraXOffset.x, t);
                float xMaxOffsetLerp = Mathf.Lerp(startingCameraXOffset.y, endCameraXOffset.y, t);

                float yMinOffsetLerp = Mathf.Lerp(startingCameraYOffset.x, endCameraYOffset.x, t);
                float yMaxOffsetLerp = Mathf.Lerp(startingCameraYOffset.y, endCameraYOffset.y, t);

                float PPU = Mathf.Lerp(startingPPU, endPPU, t);



                cameraData.CameraXBoundaryAdditionalOffset = new Vector2(xMinOffsetLerp, xMaxOffsetLerp);
                cameraData.CameraYBoundaryAdditionalOffset = new Vector2(yMinOffsetLerp, yMaxOffsetLerp);

                pixelPerfectCamera.assetsPPU = (int)PPU;


                t -= Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime);
            } while (t > 0);

            t = 0;

            cameraData.CameraXBoundaryAdditionalOffset = startingCameraYOffset;
            cameraData.CameraYBoundaryAdditionalOffset = startingCameraYOffset;

            pixelPerfectCamera.assetsPPU = startingPPU;
        }
    }
}
