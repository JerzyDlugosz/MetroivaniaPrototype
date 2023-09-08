using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager instance;
    public AudioManager audioManager;
    public SavingAndLoading savingAndLoading;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        if(SceneManager.GetActiveScene().name == "PersistentScene")
        {
            LoadScene(1);
        }
    }

    public void LoadScene(int sceneNumber)
    {
        StartCoroutine(LoadAsyncScene(sceneNumber));
    }

    public void ExitApplication()
    {
        Application.Quit();
    }

    IEnumerator LoadAsyncScene(int sceneNumber)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNumber);
        while (!asyncLoad.isDone)
        {
            Debug.Log(asyncLoad.progress);
            yield return null;
        }
    }
}
