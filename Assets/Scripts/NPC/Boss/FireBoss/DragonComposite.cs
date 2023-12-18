using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonComposite : MonoBehaviour
{
    public List<DragonBoss> dragonParts = new List<DragonBoss>();

    public BossData bossData;

    public float compositeEnemyHealth;

    public float compositeEnemyMaxHealth;

    protected static readonly int DamageScaleID = Shader.PropertyToID("_DamageScale");

    public List<Sprite> handSprites;

    public List<SpriteRenderer> bossSpriteRenderers;

    public void OnCompositeEnemyDeath()
    {
        GameManagerScript.instance.player.progressTracker.AddBoss(bossData);
    }

    public float TakeDamage(float damage)
    {
        compositeEnemyHealth -= damage;

        float scale = (compositeEnemyMaxHealth - compositeEnemyHealth) / compositeEnemyMaxHealth;

        for (int i = 0; i < dragonParts.Count; i++)
        {
            dragonParts[i].spriteRenderer.material.SetFloat(DamageScaleID, 1 + scale);
        }

        if (compositeEnemyHealth <= 0) 
        {
            OnCompositeEnemyDeath();
            for (int i = 0; i < dragonParts.Count; i++)
            {
                dragonParts[i].onNPCDeath.Invoke();
            }
            for (int i = 0; i < bossSpriteRenderers.Capacity; i++)
            {
                if (bossSpriteRenderers[i] != null)
                    Destroy(bossSpriteRenderers[i].gameObject);
            }
        }

        return scale;
    }

    public void FadeIn()
    {
        foreach (var item in bossSpriteRenderers)
        {
            item.DOFade(1, 2f);
        }
        GameManagerScript.instance.cameraHolder.DOShakePosition(2f, 1.5f);
    }

    public void SetAlpha(float alpha)
    {
        foreach (var item in bossSpriteRenderers)
        {
            item.color = new Color(item.color.r, item.color.g, item.color.b, alpha);
        }
    }
}
