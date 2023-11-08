using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameStateManager : MonoBehaviour
{
    public static GameStateManager instance;
    public AudioManager audioManager;
    public SavingAndLoading savingAndLoading;

    public int LoadingSceneNumber;

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
            SceneManager.LoadScene(1);
        }
    }

    public void LoadScene(int sceneNumber)
    {
        StartCoroutine(LoadAsyncScene(sceneNumber));
    }

    public void LoadGameSceneWithLoadingScreen()
    {
        StartCoroutine(LoadAsyncGameScene());
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

    IEnumerator LoadAsyncGameScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(LoadingSceneNumber);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
