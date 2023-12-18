using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapStation : MonoBehaviour
{
    [SerializeField]
    private string mapName;
    [SerializeField]
    private string mapDescription;
    [SerializeField]
    private Sprite mapSprite;

    public void ShowMap()
    {
        GameManagerScript.instance.player.OnSecretMapCollect();

        ItemPickupPanel.instance.ShowPanel(mapName, mapDescription, mapSprite);
    }
}
