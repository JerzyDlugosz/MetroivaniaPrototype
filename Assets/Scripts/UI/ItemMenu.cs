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
    [SerializeField]
    private TextMeshProUGUI arrowDamageCount;
    [SerializeField]
    private TextMeshProUGUI ReloadSpeedCount;

    public override void OnMenuSwap()
    {
        UpdateMenu();
    }

    private void UpdateMenu()
    {
        //This is just a temporary solution. A better one would be to make each of the upgrades in its own spot and then enable/disable them depending on what was collected

        ClearMenu();

        ProgressTracker progressTracker = GameManagerScript.instance.player.progressTracker;


        foreach (var item in progressTracker.GetCollectibles())
        {
            if (item.collectibleType == CollectibleType.Spirit)
            {
                GameObject prefab = Instantiate(itemPrefab, upgradeItemHolder.transform);
                prefab.GetComponent<Image>().sprite = progressTracker.CollectibleSprites[(int)item.spriteId];
            }
            else if(item.collectibleType == CollectibleType.ArrowType)
            {
                GameObject prefab = Instantiate(itemPrefab, arrowTypeHolder.transform);
                prefab.GetComponent<Image>().sprite = progressTracker.CollectibleSprites[(int)item.spriteId];
            }
        }


        hpCount.text = progressTracker.cHealthUpCount.ToString();
        arrowCount.text = progressTracker.cArrowCapacityCount.ToString();
        arrowDamageCount.text = progressTracker.cArrowDamageCount.ToString();
        ReloadSpeedCount.text = progressTracker.cArrowReloadSpeedCount.ToString();
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
