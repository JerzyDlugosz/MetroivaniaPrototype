using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionOption : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown Dropdown;
    [SerializeField]
    private List<Vector2Int> resolutions = new List<Vector2Int>();

    private void Start()
    {
        Vector2Int currentResolution = new Vector2Int(Screen.width, Screen.height);
        //Debug.Log($"resolution: {currentResolution}/ indexOf: {resolutions.IndexOf(currentResolution)}");
        Dropdown.value = resolutions.IndexOf(currentResolution);
        Dropdown.onValueChanged.AddListener(delegate {
            OnResolutionChange(Dropdown);
        });
    }

    public void OnResolutionChange(TMP_Dropdown dropdown)
    {
        Screen.SetResolution(resolutions[dropdown.value].x, resolutions[dropdown.value].y, false);

       
    }
}
