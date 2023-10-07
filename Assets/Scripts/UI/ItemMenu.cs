using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemMenu : CustomUIMenu
{
    [SerializeField]
    private GameObject upgradeItemHolder;
    [SerializeField]
    private GameObject arrowTypeHolder;
    [SerializeField]
    private GameObject itemPrefab;
    [SerializeField]
    private TextMeshProUGUI hpCount;
    [SerializeField]
    private TextMeshProUGUI arrowCount;

    public override void OnMenuSwap()
    {
        UpdateMenu();
    }

    private void UpdateMenu()
    {
        //This is just a temporary solution. A better one would be to make each of the upgrades in its own spot and then enable/disable them depending on what was collected

        ClearMenu();

        PlayerData playerData = GameManagerScript.instance.player.playerData;

        foreach (var item in GameManagerScript.instance.player.progressTracker.collectibles)
        {
            if (item.collectibleType == CollectibleType.PermanentUpgrade)
            {
                GameObject prefab = Instantiate(itemPrefab, upgradeItemHolder.transform);
                prefab.GetComponent<Image>().sprite = item.sprite;
            }
            else if(item.collectibleType == CollectibleType.ArrowType)
            {
                GameObject prefab = Instantiate(itemPrefab, arrowTypeHolder.transform);
                prefab.GetComponent<Image>().sprite = item.sprite;
            }
        }


        hpCount.text = (playerData.maxHealth / 4).ToString();
        arrowCount.text = playerData.maxArrowCount.ToString();
    }

    private void ClearMenu()
    {
        foreach (Transform item in upgradeItemHolder.transform)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in arrowTypeHolder.transform)
        {
            Destroy(item.gameObject);
        }
    }
}
