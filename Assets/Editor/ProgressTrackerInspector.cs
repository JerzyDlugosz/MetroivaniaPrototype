using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ProgressTracker))]
[CanEditMultipleObjects]
public class ProgressTrackerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        ProgressTracker progressTrackerScript = (ProgressTracker)target;
        if (GUILayout.Button("Unlock bow and arrow"))
        {
            GameManagerScript.instance.player.OnBowUnlocked(true);

            GameManagerScript.instance.player.arrowTypeCollectedEvent.Invoke();

            GameManagerScript.instance.player.arrowCapacityIncreaseEvent.Invoke();
        }
        EditorGUILayout.Space();
        if (GUILayout.Button("Give Player an Arrow"))
        {
            GameManagerScript.instance.player.arrowCapacityIncreaseEvent.Invoke();
        }
        EditorGUILayout.Space();
        if (GUILayout.Button("Unlock all arrowtypes"))
        {
            for (int i = 0; i < 5; i++)
            {
                GameManagerScript.instance.player.arrowTypeCollectedEvent.Invoke();
            }
        }
        EditorGUILayout.Space();
    }
}
