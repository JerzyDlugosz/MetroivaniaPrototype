using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectedItems : MonoBehaviour
{
    public List<Collectible> collectibles = new List<Collectible>();

    public void AddCollectilbe(int id, int posX, int posY)
    {
        Collectible collectible = new Collectible();
        collectible.collectibleId = id;
        collectible.collectiblePosX = posX;
        collectible.collectiblePosY = posY;
        
        collectibles.Add(collectible);
    }
}
