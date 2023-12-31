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

            Debug.Log("Saving over previous save");
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

    /// <summary>
    /// Get player progress from save file. Returns in order: MapCompletedPercent, ItemsCollectedPercent, BossesKilledPercent, TotalPercent.
    /// </summary>
    /// <param name="save"></param>
    /// <returns></returns>
    public float[] LoadSaveFileProgress(Save save)
    {

        float maxMap = 116; //124 - 8 for last boss and secret boss. Maybe -1 just to make it more glich proof
        float maxItems = 29; //24 + 4 spirits
        float maxBosses = 4; //6 - 1 last boss - 1 secret boss

        float mapCompletedPercent;
        float itemsCollectedPercent;
        float bossesKilledPercent;
        float totalPercent;

        int unlockedMapCount = 0;
        foreach (var item in save.unlockedMap)
        {
            if (item)
                unlockedMapCount++;
        }

        mapCompletedPercent = Mathf.Min((unlockedMapCount / maxMap) * 100f, 100);
        itemsCollectedPercent = Mathf.Min((save.collectibles.Count / maxItems) * 100f, 100);
        bossesKilledPercent = Mathf.Min((save.bossesSlayed.Count / maxBosses) * 100f, 100);

        totalPercent = (mapCompletedPercent + itemsCollectedPercent + bossesKilledPercent) / 3;

        return new float[] {mapCompletedPercent, itemsCollectedPercent, bossesKilledPercent, totalPercent};
    }

    public bool loadTrueEndingState(Save save)
    {
        return save.trueEndingReached;
    }
}
