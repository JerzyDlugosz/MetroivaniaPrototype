using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class RemainingArrowScript : MonoBehaviour
{
    [SerializeField]
    private Transform arrowImagesTransform;
    [SerializeField]
    private GameObject arrowImagePrefab;

    private List<Image> arrowImages = new List<Image>();
    private List<bool> arrowActiveStates = new List<bool>();

    [SerializeField]
    private TextMeshProUGUI remainingArrowtips;

    [SerializeField]
    private List<Image> arrowTypeImages = new List<Image>();

    private void Start()
    {
        GameManagerScript.instance.player.arrowCapacityIncreaseEvent.AddListener(CreateArrowImage);
        GameManagerScript.instance.player.arrowUsedEvent.AddListener(HideArrowImage);
        GameManagerScript.instance.player.arrowtipUsedEvent.AddListener(ChangeArrowtipsCount);
        GameManagerScript.instance.player.arrowtipPickupEvent.AddListener(ChangeArrowtipsCount);
        GameManagerScript.instance.player.arrowRenewEvent.AddListener(ShowArrowImage);
        GameManagerScript.instance.player.arrowsRenewEvent.AddListener(ShowArrowImages);

        foreach (var item in arrowTypeImages)
        {
            item.color = new Color(item.color.r, item.color.g, item.color.b, 0.5f);
        }
        arrowTypeImages[0].color = new Color(arrowTypeImages[0].color.r, arrowTypeImages[0].color.g, arrowTypeImages[0].color.b, 1f);

    }

    public void ChangeImagePrefab()
    {
        foreach (var item in arrowImages)
        {
            item.transform.GetChild(0).GetComponent<Image>().sprite = GameManagerScript.instance.player.playerWeaponSwap.currentWeaponImageSprite;
        }
        SwapArrowTypeUI(GameManagerScript.instance.player.playerWeaponSwap.weaponIndex);
    }

    private void SwapArrowTypeUI(int weaponNumber)
    {
        foreach (var item in arrowTypeImages)
        {
            item.color = new Color(item.color.r, item.color.g, item.color.b, 0.5f);
            foreach (Transform child in item.transform)
            {
                Image image = child.GetComponent<Image>();
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0.5f);
            }
        }
        arrowTypeImages[weaponNumber].color = new Color(arrowTypeImages[weaponNumber].color.r, arrowTypeImages[weaponNumber].color.g, arrowTypeImages[weaponNumber].color.b, 1f);
        foreach (Transform child in arrowTypeImages[weaponNumber].transform)
        {
            Image image = child.GetComponent<Image>();
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
        }
    }

    public void EnableArrowTypesUI(int unlockedWeapons)
    {
        foreach (var item in arrowTypeImages)
        {
            item.gameObject.SetActive(false);
        }

        for (int i = 0; i < unlockedWeapons; i++)
        {
            arrowTypeImages[i].gameObject.SetActive(true);
        }
    }

    private void CreateArrowImage()
    {
        GameObject arrowObject = Instantiate(arrowImagePrefab, arrowImagesTransform);
        arrowObject.transform.GetChild(0).GetComponent<Image>().sprite = GameManagerScript.instance.player.playerWeaponSwap.currentWeaponImageSprite;
        arrowImages.Add(arrowObject.GetComponent<Image>());
        arrowActiveStates.Add(true);
        RefreshImages();
    }

    public void OnGameLoad(int arrowCount, byte unlockedArrows)
    {
        for (int i = 0; i < arrowCount; i++)
        {
            CreateArrowImage();
        }
        EnableArrowTypesUI(unlockedArrows);
    }

    private void HideArrowImage()
    {
        arrowImages[GetLastEnabledImage(-1)].fillAmount = 0;
        //arrowImages[GetLastEnabledImage(-1)].enabled = false;
        arrowActiveStates[GetLastEnabledImage(-1)] = false;
    }

    private void ShowArrowImage()
    {
        arrowImages[GetLastEnabledImage(0)].fillAmount = 1;
        //arrowImages[GetLastEnabledImage(0)].enabled = true;
        arrowActiveStates[GetLastEnabledImage(0)] = true;
    }

    private void ShowArrowImages()
    {
        for (int i = 0; i < arrowImages.Count; i++)
        {
            arrowImages[i].fillAmount = 1;
            //arrowImages[i].enabled = true;
            arrowActiveStates[i] = true;
        }
    }

    public void ChangeArrowtipsCount()
    {
        remainingArrowtips.text = $"x{GameManagerScript.instance.player.playerData.currentArrowtipsCount}";
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
            return -1;
        }
        return index;
    }

    public void ArrowRefreshIcon(float refreshValue)
    {
        arrowImages[GetLastEnabledImage(0)].fillAmount = refreshValue;
    }

    public void OnArrowStoppedRefreshing()
    {
        if(GetLastEnabledImage(0) >= arrowImages.Count)
        {
            return;
        }
        arrowImages[GetLastEnabledImage(0)].fillAmount = 0;
    }

    public void OnArrowRefreshed()
    {
        arrowImages[GetLastEnabledImage(-1)].fillAmount = 1;
        //for (int i = 0; i < arrowImages.Count; i++)
        //{
        //    arrowImages[i].fillAmount = 0;
        //}
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
            image.fillAmount = 0;
            //image.enabled = false;
        }

        for (int i = 0; i < activeAmmount; i++)
        {
            arrowImages[i].fillAmount = 1;
            arrowActiveStates[i] = true;
            //arrowImages[i].enabled = true;
        }
    }
}
