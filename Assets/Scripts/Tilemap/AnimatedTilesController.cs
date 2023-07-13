using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class AnimatedTilesController : MonoBehaviour
{
    private Tilemap tilemap;
    [SerializeField]
    private float noiseXScale;
    [SerializeField]
    private float noiseYScale;

    private float xPos;
    private float yPos;

    [SerializeField]
    private float animationSpeed;

    private float noiseLimit = 100;
    private bool noiseLimitReached = false;

    public bool isUsingNoise = true;
    public bool isUsingRandomNumbers = false;

    [SerializeField]
    private float randomMinTime;
    [SerializeField]
    private float randomMaxTime;

    private int mapXPos;
    private int mapYPos;

    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        mapXPos = (int)transform.parent.position.x;
        mapYPos = (int)transform.parent.position.y;

        if (isUsingNoise)
            StartCoroutine(NoiseAnim());

        if (isUsingRandomNumbers)
            StartCoroutine(RandomNumberAnim());
    }

    private void UpdateNoisePosition()
    {
        if(noiseLimitReached)
        {
            xPos -= Time.deltaTime * animationSpeed;
            yPos -= Time.deltaTime * animationSpeed;
        }
        else
        {
            xPos += Time.deltaTime * animationSpeed;
            yPos += Time.deltaTime * animationSpeed;
        }

        if(xPos > noiseLimit)
        {
            noiseLimitReached = true;
        }
        if(xPos < 0)
        {
            noiseLimitReached = false;
        }
    }

    IEnumerator NoiseAnim()
    {
        do
        {
            UpdateNoisePosition();

            //for (int i = tilemap.cellBounds.xMin + mapXPos; i < tilemap.cellBounds.xMax + mapXPos; i++)
            //{
            //    for (int j = tilemap.cellBounds.yMin + mapYPos; j < tilemap.cellBounds.yMax + mapYPos; j++)
            //    {
            //        Vector3Int temp = new Vector3Int(i, j, (int)tilemap.GetComponent<Transform>().position.y);
            //        if (tilemap.HasTile(temp))
            //        {
            //            tilemap.SetAnimationFrame(temp, (int)(Mathf.PerlinNoise(xPos + (noiseXScale * tilemap.GetAnimationFrameCount(temp) * i), yPos + (noiseYScale * tilemap.GetAnimationFrameCount(temp) * i)) * tilemap.GetAnimationFrameCount(temp)));
            //        }
            //    }
            //}

            foreach (var pos in tilemap.cellBounds.allPositionsWithin)
            {
                if (tilemap.HasTile(pos))
                {
                    tilemap.SetAnimationFrame(pos, (int)(Mathf.PerlinNoise(xPos + (noiseXScale * tilemap.GetAnimationFrameCount(pos) * pos.x), yPos + (noiseYScale * tilemap.GetAnimationFrameCount(pos) * pos.y)) * tilemap.GetAnimationFrameCount(pos)));    
                }
            }

            yield return new WaitForEndOfFrame();
        } while (true);
    }

    IEnumerator RandomNumberAnim()
    {
        do
        {
            foreach (var pos in tilemap.cellBounds.allPositionsWithin)
            {
                if (tilemap.HasTile(pos))
                {
                    tilemap.SetAnimationFrame(pos, Random.Range(0, tilemap.GetAnimationFrameCount(pos)));
                }
            }

            float timeToWait = Random.Range(randomMinTime, randomMaxTime);
            Debug.Log(timeToWait);
            yield return new WaitForSeconds(timeToWait);
        } while (true);
    }
}
