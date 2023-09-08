using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemainingArrowScript : MonoBehaviour
{
    [SerializeField]
    private GameObject arrowImagePrefab;

    private List<Image> arrowImages = new List<Image>();
    private List<bool> arrowActiveStates = new List<bool>(); 

    private void Start()
    {
        GameManagerScript.instance.player.arrowCapacityIncreaseEvent.AddListener(CreateArrowImage);
        GameManagerScript.instance.player.arrowUsedEvent.AddListener(HideArrowImage);
        GameManagerScript.instance.player.arrowRenewEvent.AddListener(ShowArrowImage);
        GameManagerScript.instance.player.arrowChangeLeftEvent.AddListener(() => ChangeImagePrefab());
        GameManagerScript.instance.player.arrowChangeRightEvent.AddListener(() => ChangeImagePrefab());
    }

    private void ChangeImagePrefab()
    {
        foreach (var item in arrowImages)
        {
            item.sprite = GameManagerScript.instance.player.playerWeaponSwap.currentWeaponImageSprite;
        }
    }

    private void CreateArrowImage()
    {
        GameObject arrowObject = Instantiate(arrowImagePrefab, transform);
        arrowObject.GetComponent<Image>().sprite = GameManagerScript.instance.player.playerWeaponSwap.currentWeaponImageSprite;
        arrowImages.Add(arrowObject.GetComponent<Image>());
        arrowActiveStates.Add(true);
        RefreshImages();
    }

    private void HideArrowImage()
    {
        arrowImages[GetLastEnabledImage(-1)].enabled = false;
        arrowActiveStates[GetLastEnabledImage(-1)] = false;
    }

    private void ShowArrowImage()
    {
        arrowImages[GetLastEnabledImage(0)].enabled = true;
        arrowActiveStates[GetLastEnabledImage(0)] = true;
    }

    private int GetLastEnabledImage(int _index)
    {
        int index = _index;
        foreach (bool activeState in arrowActiveStates)
        {
            if (activeState)
            {
                index++;
            }
        }
        if(index < 0)
        {
            Debug.LogError("Trying to get an arrow image but there are no arrow images left!");
            return 0;
        }
        return index;
    }

    private void RefreshImages()
    {
        int activeAmmount = 0;
        for (int i = 0; i < arrowActiveStates.Count; i++)
        {
            if (arrowActiveStates[i])
            {
                arrowActiveStates[i] = false;
                activeAmmount++;
            }
        }

        foreach (Image image in arrowImages)
        {
            image.enabled = false;
        }

        for (int i = 0; i < activeAmmount; i++)
        {
            arrowActiveStates[i] = true;
            arrowImages[i].enabled = true;
        }
    }
}
