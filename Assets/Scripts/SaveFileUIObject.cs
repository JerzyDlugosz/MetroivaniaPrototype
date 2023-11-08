using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SaveFileUIObject : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public TextMeshProUGUI zoneName;
    public TextMeshProUGUI playTime;
    public TextMeshProUGUI mapCompletedNumber;
    public TextMeshProUGUI itemsCollectedNumber;
    public TextMeshProUGUI bossesKilledNumber;

    public GameObject savePanel;
    public GameObject noSavePanel;
    public Image background;

    public GameObject fileHighlight;

    public StartMenu startMenu;

    public void OnSelect(BaseEventData eventData)
    {
        fileHighlight.SetActive(true);
        fileHighlight.GetComponent<MinimapAnimation>().RestartCoroutine();
    }

    public void OnDeselect(BaseEventData data)
    {
        fileHighlight.SetActive(false);
    }
}
