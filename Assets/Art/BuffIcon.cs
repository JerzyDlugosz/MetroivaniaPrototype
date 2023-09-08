using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffIcon : MonoBehaviour
{
    private int buffType;
    [SerializeField]
    private Image buffImage;
    [SerializeField]
    private Image durationImage;

    private Color startColor = Color.green;
    private Color middleColor = new Color(1f, 1f, 0f);
    private Color endColor = Color.red;

    private float value;


    private void Start()
    {
        durationImage.fillAmount = 1;
    }

    public void SetupBuff(int _buffType, Sprite buffSprite)
    {
        buffType = _buffType;
        buffImage.sprite = buffSprite;
    }

    public void SetupBuff(int _buffType)
    {
        buffType = _buffType;
    }

    // Update is called once per frame
    void Update()
    {
        durationImage.fillAmount = value;
        SetColor();
    }

    void SetColor()
    {
        durationImage.color = LerpColor(1 - durationImage.fillAmount);
    }

    Color LerpColor(float time)
    {
        if(time < 0.5f)
        {
            return Color.Lerp(startColor, middleColor, time * 2);
        }
        else
        {
            return Color.Lerp(middleColor, endColor, (time - 0.5f) * 2);
        }
    }
}
