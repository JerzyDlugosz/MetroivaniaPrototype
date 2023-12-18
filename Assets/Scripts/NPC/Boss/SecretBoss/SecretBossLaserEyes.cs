using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class OnSecretBossEyeDestroy : UnityEvent { }
public class SecretBossLaserEyes : MonoBehaviour
{
    public OnSecretBossEyeDestroy OnSecretBossEyeDestroyEvent;
    [SerializeField]
    private float maxSpeed = 2f;
    [SerializeField]
    private SpriteRenderer laserSpriteRenderer;
    [SerializeField]
    private BoxCollider2D laserCollider;
    [SerializeField]
    private List<Sprite> laserSprites = new List<Sprite>();

    float damage = 1f;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="movePosition"></param>
    /// <param name="rotation">true - left, false - right</param>
    public void EyeAttack(Vector3 movePosition, bool rotation)
    {
        if(rotation)
        {
            transform.DOLocalRotate(new Vector3(0, 0, 0), MathF.Min(Vector3.Distance(movePosition, transform.localPosition) / maxSpeed, 4f)).SetEase(Ease.Linear);
        }
        else
        {
            transform.DOLocalRotate(new Vector3(0, 0, 180), MathF.Min(Vector3.Distance(movePosition, transform.localPosition) / maxSpeed, 4f)).SetEase(Ease.Linear);
        }
        transform.DOLocalMove(movePosition, MathF.Min(Vector3.Distance(movePosition, transform.localPosition) / maxSpeed, 4f)).SetEase(Ease.Linear).OnComplete(() =>
        {
            SetLaser(0f, 2);
            transform.DOShakePosition(1f, 0.2f).OnComplete(() =>
            {
                SetLaser(0.75f, 3);
                //Wait for 2s
                transform.DOShakePosition(2f, 0f).OnComplete(() =>
                {
                    SetLaser(0, 0);

                    if (rotation)
                    {
                        transform.DOLocalRotate(new Vector3(0, 0, 140), MathF.Min(Vector3.Distance(movePosition, transform.localPosition) / maxSpeed, 4f)).SetEase(Ease.Linear);
                        transform.DOLocalMove(transform.localPosition + new Vector3(-10, 10, 0), MathF.Min(Vector3.Distance(Vector3.zero, transform.localPosition) / maxSpeed, 4f)).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            Destroy(gameObject);
                        });
                    }
                    else
                    {
                        transform.DOLocalRotate(new Vector3(0, 0, 40), MathF.Min(Vector3.Distance(movePosition, transform.localPosition) / maxSpeed, 4f)).SetEase(Ease.Linear);
                        transform.DOLocalMove(transform.localPosition + new Vector3(10, 10, 0), MathF.Min(Vector3.Distance(Vector3.zero, transform.localPosition) / maxSpeed, 4f)).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            Destroy(gameObject);
                        });
                    }
                });
            });
        });
    }

    private void SetLaser(float colliderSize, int laserSprite)
    {
        if (colliderSize == 0)
        {
            laserCollider.enabled = false;
        }
        else
        {
            laserCollider.enabled = true;
            laserCollider.size = new Vector2(colliderSize, 1f);
        }
        laserSpriteRenderer.sprite = laserSprites[laserSprite];
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponentInParent<Player>().damageTakenEvent.Invoke(damage);
        }
    }

    private void OnDestroy()
    {
        OnSecretBossEyeDestroyEvent.Invoke();
    }
}
