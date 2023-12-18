using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapAnimation : MonoBehaviour
{
    [SerializeField]
    private Image image;
    [SerializeField]
    private List<Sprite> sprites;
    [SerializeField]
    private float animationSpeed;
    [SerializeField]
    private bool OnStart = true;
    private void Start()
    {
        if(OnStart)
            StartCoroutine(TileAnimation());
    }

    public void ManuallyStartCoroutine()
    {
        StartCoroutine(TileAnimation());
    }

    public void RestartCoroutine()
    {
        StopCoroutine(TileAnimation());
        StartCoroutine(TileAnimation());
    }

    IEnumerator TileAnimation()
    {
        int i = 0;
        do
        {
            image.sprite = sprites[i];
            i++;
            if (i >= sprites.Count) i = 0;
            yield return new WaitForSecondsRealtime(animationSpeed);
        } while (true);
    }
}
