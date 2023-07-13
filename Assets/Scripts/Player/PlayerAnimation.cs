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
    private SpriteRenderer playerSprite;
    [SerializeField]
    private float moveAnimationSpeed;
    [SerializeField]
    private float jumpAnimationSpeed;

    private float playerXVelocity;

    void Update()
    {
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
        //Debug.Log(playerXVelocity);
        playerSprite.sprite = playerSprites[(int)playerXVelocity];
    }

    public void OnPlayerJump()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(playerSprite.transform.DOScaleY(0.9f, jumpAnimationSpeed));
        sequence.Append(playerSprite.transform.DOScaleY(1f, jumpAnimationSpeed));
    }
}
