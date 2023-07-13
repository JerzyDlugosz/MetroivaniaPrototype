using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SavingAndLoading : MonoBehaviour
{
    public int currentSaveFile = 0;
    public bool LoadGameFile(Save save)
    {
        string saveFilePath = Path.Combine(Application.persistentDataPath, $"gamesave{save.saveNumber}.json");
        if(File.Exists(saveFilePath))
        {
            string jsonText = File.ReadAllText(saveFilePath);
            JsonUtility.FromJsonOverwrite(jsonText, save);

            Debug.Log("Game Loaded");
            return true;
        }
        else
        {
            Debug.Log("No game saved!");
            return false;
        }
    }
    public bool SaveGameFile(Save save)
    {

        string saveFilePath = Path.Combine(Application.persistentDataPath, $"gamesave{save.saveNumber}.json");
        if (!File.Exists(saveFilePath))
        {
            string json = JsonUtility.ToJson(save);
            File.WriteAllText(saveFilePath, json);

            Debug.Log($"Game Saved in {saveFilePath}");

            Debug.Log("Save Game Created");
            return true;
        }
        else
        {
            string json = JsonUtility.ToJson(save);
            File.WriteAllText(saveFilePath, json);

            Debug.Log("Save file is arleady in use!");
            return false;
        }
    }

    public Save GetSaveFile(int saveNumber)
    {
        Save save = new Save();
        save.saveNumber = saveNumber;
        return save;
    }

    public bool CheckIfSaveFileExists(Save save)
    {
        string saveFilePath = Path.Combine(Application.persistentDataPath, $"gamesave{save.saveNumber}.json");
        if (File.Exists(saveFilePath))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool RemoveSaveFile(int saveNumber)
    {
        string saveFilePath = Path.Combine(Application.persistentDataPath, $"gamesave{saveNumber}.json");
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log($"Removed {saveFilePath}");
            return true;
        }
        else
        {
            return false;
        }
    }
}
