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

    public void OnCompositeEnemyDeath()
    {
        GameManagerScript.instance.player.progressTracker.AddBoss(bossData);
    }

    public void TakeDamage(float damage)
    {
        compositeEnemyHealth -= damage;

        float scale = (compositeEnemyMaxHealth - compositeEnemyHealth) / (compositeEnemyMaxHealth / 4);

        for (int i = 0; i < dragonParts.Count; i++)
        {
            dragonParts[i].spriteRenderer.material.SetFloat(DamageScaleID, 1 + scale);
        }

        if (compositeEnemyHealth <= 0) 
        {
            OnCompositeEnemyDeath();
            Debug.Log("Count: " + dragonParts.Count);
            for (int i = 0; i < dragonParts.Count; i++)
            {
                Debug.Log("Part: " + dragonParts[i].name);
                dragonParts[i].onNPCDeath.Invoke();
            }
        }
    }
}
