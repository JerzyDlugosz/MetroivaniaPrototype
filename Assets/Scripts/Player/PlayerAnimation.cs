using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField]
    private Player player;
    [SerializeField]
    private List<Sprite> playerSprites;

    [SerializeField]
    private float moveAnimationSpeed;
    [SerializeField]
    private float jumpAnimationSpeed;

    private float playerXVelocity;

    void Update()
    {
        if(player.isStopped)
        {
            return;
        }
        UpdatePlayerAnimaitonVelocity();
        UpdatePlayerSprite();

    }

    void UpdatePlayerAnimaitonVelocity()
    {
        playerXVelocity += player.playerData.animationMotion.x * moveAnimationSpeed;

        if(playerXVelocity > playerSprites.Count - 1)
        {
            playerXVelocity -= playerSprites.Count;
        }
        if(playerXVelocity < 0)
        {
            playerXVelocity += playerSprites.Count;
        }

    }

    void UpdatePlayerSprite()
    {
        int spriteVelocity = (int)playerXVelocity;
        if (spriteVelocity > playerSprites.Count - 1)
        {
            spriteVelocity = playerSprites.Count - 1;
        }

        player.playerSpriteRenderer.sprite = playerSprites[spriteVelocity];
    }

    public void OnPlayerJump()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(player.playerSpriteRenderer.transform.DOScaleY(0.8f, jumpAnimationSpeed));
        sequence.Append(player.playerSpriteRenderer.transform.DOScaleY(1f, jumpAnimationSpeed));
    }
}
