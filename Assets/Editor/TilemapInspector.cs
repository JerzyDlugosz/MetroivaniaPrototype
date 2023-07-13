using System.Diagnostics;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(CustomTilemap))]
public class TilemapInspector : Editor
{
    public VisualTreeAsset m_InspectorXML;
    public VisualElement outsideColliders;

    public override VisualElement CreateInspectorGUI()
    {
        // Create a new VisualElement to be the root of our inspector UI
        VisualElement myInspector = new VisualElement();

        // Load from default reference
        m_InspectorXML.CloneTree(myInspector);

        VisualElement inspectorFoldout = myInspector.Q("Default");
        VisualElement borderButton = myInspector.Q("TilemapBorders");

        VisualElement xSize = myInspector.Q("xSize");
        VisualElement ySize = myInspector.Q("ySize");
        outsideColliders = myInspector.Q("outsideColliders");

        borderButton.RegisterCallback<ClickEvent>(OnButtonClick);
        

        InspectorElement.FillDefaultInspector(inspectorFoldout, serializedObject, this);

        // Return the finished inspector UI
        return myInspector;
    }


    public void OnButtonClick(ClickEvent evt)
    {
        //outsideColliders = myInspector.Q("outsideColliders");
        //foreach (Transform child in outsideColliders)
        //{

        //}
        //GameObject Gobject = new GameObject();
        //Instantiate(Gobject);
    }
}
