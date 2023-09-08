using DG.Tweening;
using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEditor.Progress;

public class FancyAnimatedText : MonoBehaviour
{

    [SerializeField]
    List<TextMeshProUGUI> letters = new List<TextMeshProUGUI>();

    public float animationDuration;
    public float animationDistance;

    public void SpreadText(TextMeshProUGUI text)
    {
        TMP_TextInfo textInfo = text.textInfo;
        Debug.Log(text.text.Length);
        Debug.Log(textInfo.characterCount);

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            Vector3 bottomRight = charInfo.bottomLeft;

            GameObject obj = new GameObject(textInfo.characterInfo[i].character.ToString() + "_Char");
            TextMeshProUGUI meshPro = obj.AddComponent<TextMeshProUGUI>();
            meshPro.text = textInfo.characterInfo[i].character.ToString();
            meshPro.fontSize = text.fontSize;
            obj.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            obj.transform.SetParent(this.transform, false);
            obj.transform.localPosition = new Vector3(bottomRight.x, 0f, 0f);
            
            letters.Add(meshPro);
        }
    }

    public void AnimateText()
    {
        if (letters.Count == 0)
        {
            Debug.LogWarning("No text to animate");
            return;
        }


        float timeOffset = 0;
        bool bounce = false;

        foreach (var item in letters)
        {
            if (timeOffset >= 1f)
                bounce = true;

            if (timeOffset <= 0)
                bounce = false;

            item.transform.DOMove(item.transform.position + new Vector3(0, animationDistance, 0f), animationDuration).SetLoops(-1, LoopType.Yoyo).Goto(timeOffset, true);

            if (bounce)
            {
                timeOffset -= 0.25f;
            }
            else
            {
                timeOffset += 0.25f;
            }
        }
    }

    public void ClearText()
    {
        

        for (int i = 0; i < letters.Count; i++)
        {
            letters[i].transform.DOKill();
            Destroy(letters[i].gameObject);
        }
        letters.Clear();
    }
}
