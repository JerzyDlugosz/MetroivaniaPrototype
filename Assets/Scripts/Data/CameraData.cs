using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraData : MonoBehaviour
{
    /// <summary>
    /// Min - Max
    /// </summary>
    public Vector2 CameraXBoundaryAdditionalOffset;
    public Vector2 CameraYBoundaryAdditionalOffset;

    public Vector2 baseCameraXBoundaryAdditionalOffset = new Vector2(0, 0);
    public Vector2 baseCameraYBoundaryAdditionalOffset = new Vector2(0, 0);

    public int baseAssetsPPU = 30;
}
