using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FancyAnimatedText))]
public class FancyAnimatedTextInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        FancyAnimatedText FancyAnimatedTextScript = (FancyAnimatedText)target;

    }
}
