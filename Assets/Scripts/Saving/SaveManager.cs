using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class SaveManager : MonoBehaviour
{
    [SerializeField] TMP_InputField saveName;
    [SerializeField] Button createButton;
    [SerializeField] Button loadButton;
    [Space(5)]
    [SerializeField] Button loadPanelBackButton;
    [SerializeField] GameObject loadButtonPrefab;
    [Space(5)]
    [SerializeField] Button deleteWarningNoButton;
    [SerializeField] GameObject deleteWarningPanel;
    [SerializeField] TextMeshProUGUI deleteWarningSaveName;
    [SerializeField] GameObject saveAlreadyExistsText;
    [Space(5)]
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] RectTransform scrollRectContent;
    [SerializeField] Scrollbar scrollBar;
    [SerializeField] float rectContentOffset = 250f;
    [SerializeField] FSM fsm;
    [Space(5)]
    [SerializeField] string sceneToLoad;

    private string[] saveFiles;
    private string deleteSaveName;
    private List<GameObject> instantiatedLoadButtons = new List<GameObject>();

    private void Start()
    {
        EventSystem<Transform>.Subscribe(EventType.LOAD_BUTTON_SELECT, ScrollReposition);
    }

    public void ScrollReposition(Transform _objPosition)
    {
        Canvas.ForceUpdateCanvases();

        Vector2 childLocalPosition = _objPosition.localPosition;

        var objPosition = (Vector2)scrollRect.transform.InverseTransformPoint(_objPosition.position);
        var scrollHeight = scrollRect.GetComponent<RectTransform>().rect.height;

        Vector2 result = new Vector2(0, -childLocalPosition.y);

        if (objPosition.y > scrollHeight / 2.25)
        {
            result.y += rectContentOffset;
            scrollRectContent.transform.localPosition = result;
        }

        if (objPosition.y < -scrollHeight / 2.25)
        {
            result.y -= rectContentOffset;
            scrollRectContent.transform.localPosition = result;
        }
    }

    public void OnCreateSave()
    {
        GetLoadFiles();

        if (saveName.text.Length > 0)
        {
            for (int i = 0; i < saveFiles.Length; i++)
            {
                string save = saveFiles[i].Replace(Application.persistentDataPath + "/saves/", "");
                save = save.Substring(0, save.IndexOf("."));

                if (saveName.text == save)
                {
                    saveAlreadyExistsText.SetActive(true);
                    return;
                }
                else
                {
                    saveAlreadyExistsText.SetActive(false);
                }
            }

            SerializationManager.Save(saveName.text, SaveData.current);
            createButton.interactable = false;
            loadButton.interactable = false;

            PersistentSaveName.saveName = saveName.text;
            fsm.StartGame(sceneToLoad);
        }
    }

    public void GetLoadFiles()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/saves/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves/");
        }

        saveFiles = Directory.GetFiles(Application.persistentDataPath + "/saves/");
    }

    public void ShowLoadScreen()
    {
        GetLoadFiles();

        foreach (Transform button in scrollRectContent)
        {
            Destroy(button.gameObject);
        }

        instantiatedLoadButtons.Clear();

        for (int i = 0; i < saveFiles.Length; i++)
        {
            GameObject buttonObject = Instantiate(loadButtonPrefab);
            buttonObject.transform.SetParent(scrollRectContent.transform, false);
            instantiatedLoadButtons.Add(buttonObject);

            var index = i;
            string save = saveFiles[index].Replace(Application.persistentDataPath + "/saves/", "");
            save = save.Substring(0, save.IndexOf("."));

            SaveFileItem saveFileItem = buttonObject.GetComponent<SaveFileItem>();

            saveFileItem.loadSaveButton.onClick.AddListener(() =>
            {
                PersistentSaveName.saveName = save;
                fsm.StartGame(sceneToLoad);

                foreach (var button in instantiatedLoadButtons)
                {
                    button.GetComponent<SaveFileItem>().loadSaveButton.interactable = false;
                }
            });

            saveFileItem.deleteButton.onClick.AddListener(() =>
            {
                deleteSaveName = save;
                deleteWarningPanel.SetActive(true);
                deleteWarningSaveName.text = save;
                loadPanelBackButton.interactable = false;
                deleteWarningNoButton.Select();
            });

            saveFileItem.saveFileText.text = save;
        }

        SetNavigation();

        if (instantiatedLoadButtons.Count > 0)
        {
            instantiatedLoadButtons[0].GetComponent<SaveFileItem>().loadSaveButton.Select();
        }

        scrollRect.content.localPosition = Vector2.zero;
    }

    private void SetNavigation()
    {
        for (int i = 0; i < instantiatedLoadButtons.Count; i++)
        {
            SaveFileItem saveFileItem = instantiatedLoadButtons[i].gameObject.GetComponent<SaveFileItem>();

            Navigation loadButtonNav = new Navigation();
            loadButtonNav.mode = Navigation.Mode.Explicit;

            Navigation deleteButtonNav = new Navigation();
            deleteButtonNav.mode = Navigation.Mode.Explicit;

            loadButtonNav.selectOnRight = saveFileItem.deleteButton;
            deleteButtonNav.selectOnLeft = saveFileItem.loadSaveButton;
            deleteButtonNav.selectOnRight = scrollBar;

            if (i != 0)
            {
                loadButtonNav.selectOnUp = instantiatedLoadButtons[i - 1].gameObject.GetComponent<SaveFileItem>().loadSaveButton;
                deleteButtonNav.selectOnUp = instantiatedLoadButtons[i - 1].gameObject.GetComponent<SaveFileItem>().deleteButton;
            }
            if (i != instantiatedLoadButtons.Count - 1)
            {
                loadButtonNav.selectOnDown = instantiatedLoadButtons[i + 1].gameObject.GetComponent<SaveFileItem>().loadSaveButton;
                deleteButtonNav.selectOnDown = instantiatedLoadButtons[i + 1].gameObject.GetComponent<SaveFileItem>().deleteButton;
            }
            if (i == instantiatedLoadButtons.Count - 1)
            {
                loadButtonNav.selectOnDown = loadPanelBackButton;
                deleteButtonNav.selectOnDown = loadPanelBackButton;
            }

            saveFileItem.loadSaveButton.navigation = loadButtonNav;
            saveFileItem.deleteButton.navigation = deleteButtonNav;
        }
    }

    public void SelectButton()
    {
        if (instantiatedLoadButtons.Count > 0)
        {
            instantiatedLoadButtons[0].GetComponent<SaveFileItem>().loadSaveButton.Select();
        }
    }

    public void DeleteSaveFile()
    {
        SerializationManager.Delete(deleteSaveName);
    }
}
