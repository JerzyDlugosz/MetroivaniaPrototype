using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemainingHearthsScript : MonoBehaviour
{
    [SerializeField]
    private GameObject hearthImagePrefab;

    private List<GameObject> hearthImages = new List<GameObject>();

    private void Start()
    {
        SetHearthImages();

        GameManagerScript.instance.player.maxHealthUpdateEvent.AddListener(AddHearthImage);
    }

    private void SetHearthImages()
    {
        int maxHealth = GameManagerScript.instance.player.playerData.maxHealth;
        int ammount = maxHealth / 4;
        for (int i = 0; i < ammount; i++)
        {
            hearthImages.Add(Instantiate(hearthImagePrefab, transform));
        }
    }

    public void UpdateCurrentHealthOnImages(int currentHealth)
    {
        ResetHearthImages();

        int targetedHearth;
        if (currentHealth <= 1)
        {
            targetedHearth = 0;
        }
        else
        {
            targetedHearth = (currentHealth - 1) / 4;
        }

        int healthFillAmmount = currentHealth - (targetedHearth * 4);


        for (int i = 0; i < targetedHearth; i++)
        {
            hearthImages[i].GetComponent<HearthIcon>().hearth.fillAmount = 1f;
            hearthImages[i].GetComponent<HearthIcon>().flash.fillAmount = 1f;
        }

        hearthImages[targetedHearth].GetComponent<HearthIcon>().hearth.fillAmount = healthFillAmmount / 4f;
        hearthImages[targetedHearth].GetComponent<HearthIcon>().flash.fillAmount = healthFillAmmount / 4f;
    }

    void ResetHearthImages()
    {
        foreach (var item in hearthImages)
        {
            item.GetComponent<HearthIcon>().hearth.fillAmount = 0f;
            item.GetComponent<HearthIcon>().flash.fillAmount = 0f;
        }
    }

    private void AddHearthImage()
    {
        hearthImages.Add(Instantiate(hearthImagePrefab, transform));
    }

    public void FlashHearths(int flashID, bool state)
    {
        foreach (var item in hearthImages)
        {
            item.GetComponent<HearthIcon>().hearth.gameObject.SetActive(!state);
        }
    }
}
