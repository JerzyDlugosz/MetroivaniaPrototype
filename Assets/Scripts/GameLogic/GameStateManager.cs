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
            OnGameInit();
            SceneManager.LoadScene(1);
        }
    }

    private void OnGameInit()
    {
        Cursor.visible = false;
    }

    public void LoadScene(int sceneNumber)
    {
        DOTween.KillAll();
        StartCoroutine(LoadAsyncScene(sceneNumber));
    }

    public void LoadGameSceneWithLoadingScreen()
    {
        DOTween.KillAll();
        StartCoroutine(LoadAsyncGameScene());
    }

    public void ExitApplication()
    {
        DOTween.KillAll();
        Application.Quit();
    }

    IEnumerator LoadAsyncScene(int sceneNumber)
    {


        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNumber);
        while (!asyncLoad.isDone)
        {
            //Debug.Log(asyncLoad.progress);
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
