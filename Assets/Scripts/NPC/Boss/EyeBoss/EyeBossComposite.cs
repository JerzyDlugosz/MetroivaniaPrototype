using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class OnCompositeEyeBossDeath : UnityEvent { }

public class EyeBossComposite : MonoBehaviour
{
    public OnCompositeEyeBossDeath compositeEyeBossDeathEvent;
    public List<EyeBoss> EyeBossParts = new List<EyeBoss>();

    public BossData bossData;

    [SerializeField]
    private int EyeBossPartsCount;
    public float remainingPartsModifier = 1f;

    protected static readonly int DamageScaleID = Shader.PropertyToID("_DamageScale");

    public List<SpriteRenderer> bossSpriteRenderers;

    private void Start()
    {
        foreach (var item in EyeBossParts)
        {
            item.onNPCDeath.AddListener(OnEnemyPartDeath);
        }

        compositeEyeBossDeathEvent.AddListener(() => GameManagerScript.instance.player.progressTracker.AddBoss(bossData));
        EyeBossPartsCount = EyeBossParts.Count;
    }

    public void OnCompositeEnemyDeath()
    {
        compositeEyeBossDeathEvent.Invoke();
    }

    public void OnEnemyPartDeath()
    {
        EyeBossPartsCount--;
        remainingPartsModifier += 0.5f;
        if (EyeBossPartsCount <= 0)
            OnCompositeEnemyDeath();
            
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
