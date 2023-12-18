using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class OnCollect : UnityEvent { }

public class CollectibleGameObject : MonoBehaviour
{
    [SerializeField]
    private Collider2D CCollider;
    public OnCollect collectEvent;

    public int collectibleId;
    [SerializeField]
    protected int collectiblePositionX;
    [SerializeField]
    protected int collectiblePositionY;
    [SerializeField]
    protected Sprite collectibleSprite;
    [SerializeField]
    protected CollectibleType collectibleType;
    [SerializeField]
    protected CollectibleSpriteIDs collectibleSpriteID;
    [SerializeField]
    protected AudioClip PickupAudioClip;
    [SerializeField]
    protected string collectibleName;
    [SerializeField]
    protected string collectibleDescription;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            GameStateManager.instance.audioManager.PlaySoundEffect(PickupAudioClip);
            collectEvent.Invoke();
            CCollider.enabled = false;
        }
    }

    public CollectibleType GetCollectibleType()
    {
        return collectibleType;
    }

    public void SetPosition(int xPos, int yPos)
    {
        collectiblePositionX = xPos;
        collectiblePositionY = yPos;
    }

    public void GetPosition(out int _xPos, out int _yPos)
    {
        _xPos = collectiblePositionX;
        _yPos = collectiblePositionY;
    }
}

public enum CollectibleType
{
    StatUpgrade,
    ArrowType,
    PermanentUpgrade,
    Spirit
}
