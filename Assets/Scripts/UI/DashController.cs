using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashController : MonoBehaviour
{
    [SerializeField]
    private List<Image> dashImages = new List<Image>();
    private int currentDashImageIndex = 2;
    
    public void FillDashImage()
    {
        currentDashImageIndex++;
        if (currentDashImageIndex >= dashImages.Count)
        {
            currentDashImageIndex = dashImages.Count - 1;
        }
        dashImages[currentDashImageIndex].enabled = true;
    }

    public void EmptyDashImage()
    {
        if (currentDashImageIndex < 0)
        {
            currentDashImageIndex = 0;
        }
        dashImages[currentDashImageIndex].enabled = false;
        currentDashImageIndex--;
    }
}
