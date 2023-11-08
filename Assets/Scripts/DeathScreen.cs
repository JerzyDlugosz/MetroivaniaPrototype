using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    [SerializeField]
    private Button LoadSaveButton;
    private void Start()
    {
        LoadSaveButton.Select();
    }
}
