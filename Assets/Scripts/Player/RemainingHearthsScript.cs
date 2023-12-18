using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemainingHearthsScript : MonoBehaviour
{
    [SerializeField]
    private GameObject hearthImagePrefab;
    [SerializeField]
    private List<GameObject> hearthImages = new List<GameObject>();

    public void SetHearthImages(int _maxHealth)
    {
        int maxHealth = _maxHealth;
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
            hearthImages[i].GetComponent<HearthIcon>().damageFlash.fillAmount = 1f;
            hearthImages[i].GetComponent<HearthIcon>().healingFlash.fillAmount = 1f;
        }
        hearthImages[targetedHearth].GetComponent<HearthIcon>().hearth.fillAmount = healthFillAmmount / 4f;
        hearthImages[targetedHearth].GetComponent<HearthIcon>().damageFlash.fillAmount = healthFillAmmount / 4f;
        hearthImages[targetedHearth].GetComponent<HearthIcon>().healingFlash.fillAmount = healthFillAmmount / 4f;
    }

    void ResetHearthImages()
    {
        foreach (var item in hearthImages)
        {
            item.GetComponent<HearthIcon>().hearth.fillAmount = 0f;
            item.GetComponent<HearthIcon>().damageFlash.fillAmount = 0f;
            item.GetComponent<HearthIcon>().healingFlash.fillAmount = 0f;
        }
    }

    public void AddHearthImage()
    {
        hearthImages.Add(Instantiate(hearthImagePrefab, transform));
    }

    public void FlashHearths(int flashType, bool state)
    {
        if(flashType == 0)
        {
            foreach (var item in hearthImages)
            {
                item.GetComponent<HearthIcon>().hearth.gameObject.SetActive(!state);
                item.GetComponent<HearthIcon>().damageFlash.gameObject.SetActive(state);
                item.GetComponent<HearthIcon>().healingFlash.gameObject.SetActive(!state);
            }
        }
        if(flashType == 1)
        {
            foreach (var item in hearthImages)
            {
                item.GetComponent<HearthIcon>().hearth.gameObject.SetActive(!state);
                item.GetComponent<HearthIcon>().damageFlash.gameObject.SetActive(!state);
                item.GetComponent<HearthIcon>().healingFlash.gameObject.SetActive(state);
            }
        }
    }
}
