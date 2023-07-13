using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomButton : MonoBehaviour
{
    public void ChangeScene(int sceneNumber)
    {
        GameStateManager.instance.LoadScene(sceneNumber);
    }

    public void DeleteSaveFile()
    {
        GameStateManager.instance.GetComponent<SavingAndLoading>().RemoveSaveFile(GameStateManager.instance.GetComponent<SavingAndLoading>().currentSaveFile);
    }
}
