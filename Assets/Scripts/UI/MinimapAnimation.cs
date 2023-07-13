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
    private void Start()
    {
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
            yield return new WaitForSeconds(animationSpeed);
        } while (true);
    }
}
