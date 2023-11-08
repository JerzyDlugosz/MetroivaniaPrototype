using DG.Tweening;
using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class FancyAnimatedText : MonoBehaviour
{

    [SerializeField]
    List<TextMeshProUGUI> letters = new List<TextMeshProUGUI>();

    public float animationDuration;
    public float animationDistance;

    private List<Transform> letterParents = new List<Transform>();

    [SerializeField]
    private Transform lettersWith0Offset;
    [SerializeField]
    private Transform lettersWith25Offset;
    [SerializeField]
    private Transform lettersWith50Offset;
    [SerializeField]
    private Transform lettersWith75Offset;
    [SerializeField]
    private Transform lettersWith100Offset;

    public void SpreadText(TextMeshProUGUI text)
    {
        letterParents.Add(lettersWith0Offset);
        letterParents.Add(lettersWith25Offset);
        letterParents.Add(lettersWith50Offset);
        letterParents.Add(lettersWith75Offset);
        letterParents.Add(lettersWith100Offset);


        TMP_TextInfo textInfo = text.textInfo;
        Debug.Log(text.text.Length);
        Debug.Log(textInfo.characterCount);

        int timeOffset = 0;
        bool bounce = false;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (timeOffset >= 4)
                bounce = true;

            if (timeOffset <= 0)
                bounce = false;

            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            Vector3 bottomRight = charInfo.bottomLeft;

            GameObject obj = new GameObject(textInfo.characterInfo[i].character.ToString() + "_Char");
            TextMeshProUGUI meshPro = obj.AddComponent<TextMeshProUGUI>();
            meshPro.text = textInfo.characterInfo[i].character.ToString();
            meshPro.fontSize = text.fontSize;
            obj.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
            obj.transform.SetParent(this.transform, false);
            obj.transform.localPosition = new Vector3(bottomRight.x, bottomRight.y, 0f);
            
            letters.Add(meshPro);

            obj.transform.SetParent(letterParents[timeOffset]);

            if (bounce)
            {
                timeOffset -= 1;
            }
            else
            {
                timeOffset += 1;
            }
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

        foreach (var item in letterParents)
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
        

        for (int i = 0; i < letterParents.Count; i++)
        {
            letterParents[i].transform.DOKill();
        }

        for (int i = 0; i < letters.Count; i++)
        {
            Destroy(letters[i].gameObject);
        }

        letters.Clear();
    }
}
