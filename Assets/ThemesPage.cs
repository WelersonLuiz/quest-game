using System;
using System.Collections.Generic;
using Objects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemesPage : MonoBehaviour
{
    
    public GameObject loading;
    
    private GameObject _content;
    private GameObject _entry;
    private Button _updateButton;
    private Button _deleteButton;
    private GameObject _updatePopUp;

    private Dictionary<int, string> _keys;
    private QuestionsPage _questionsPage;
    
    private void Start()
    {
        _questionsPage = gameObject.transform.parent.Find("QuestionsPage").GetComponent<QuestionsPage>();
        _content = gameObject.transform.Find("Scroll View").Find("Viewport").Find("Content").gameObject;
        _entry = _content.transform.Find("Entry").gameObject;
        
        SetupCreateTheme();
        LoadAllThemes();
    }
    private void SetupCreateTheme()
    {
        var createPopUp = gameObject.transform.Find("CreatePopUp").gameObject;
        var contentPopUp = createPopUp.transform.Find("PopUpContent").gameObject;
        
        // SetUp Add Button
        var createButton = gameObject.transform.Find("AddButton").GetComponent<Button>();
        createButton.onClick.RemoveAllListeners();
        createButton.onClick.AddListener(() =>
        {
            var themeInputField = contentPopUp.transform.Find("ThemeNameInputField").GetComponent<TMP_InputField>();
            themeInputField.text = "";
            
            // SetUp Save Button
            var saveCreate = contentPopUp.transform.Find("SaveButton").GetComponent<Button>();
            saveCreate.onClick.RemoveAllListeners();
            saveCreate.onClick.AddListener(() =>
            {
                var themeName = themeInputField.text;
                CreateTheme(themeName);
                createPopUp.SetActive(false);
            });
            
            createPopUp.SetActive(true);
        });
        
        // SetUp Cancel Button
        var cancelCreate = contentPopUp.transform.Find("CancelButton").GetComponent<Button>();
        cancelCreate.onClick.RemoveAllListeners();
        cancelCreate.onClick.AddListener(() =>
        {
            createPopUp.SetActive(false);
        });

    }
    private void SetupUpdateButton(GameObject entry)
    {
        _updatePopUp = gameObject.transform.Find("UpdatePopUp").gameObject;
        var popUpContent = _updatePopUp.transform.Find("PopUpContent").gameObject;

        // SetUp Update Button
        var updateButton = entry.transform.Find("EditButton").GetComponent<Button>();
        updateButton.onClick.RemoveAllListeners();
        updateButton.onClick.AddListener(() =>
        {
            // SetUp Update PopUp
            var saveUpdate = popUpContent.transform.Find("SaveButton").GetComponent<Button>();
            var themeInputField = popUpContent.transform.Find("ThemeNameInputField").GetComponent<TMP_InputField>();
            themeInputField.text = entry.transform.Find("Name").GetComponent<TMP_Text>().text;
            
            // SetUp Save Button
            saveUpdate.onClick.RemoveAllListeners();
            saveUpdate.onClick.AddListener(() =>
            {
                var themeName = themeInputField.text;
                UpdateTheme(entry, themeName);
                _updatePopUp.SetActive(false);
            });
            
            // SetUp Cancel Button
            var cancelDelete = popUpContent.transform.Find("CancelButton").GetComponent<Button>();
            cancelDelete.onClick.RemoveAllListeners();
            cancelDelete.onClick.AddListener(() =>
            {
                _updatePopUp.SetActive(false);
            });

            _updatePopUp.SetActive(true);
        });
        
    }
    private void SetupDeleteButton(GameObject entry)
    {
        var deletePopUp = gameObject.transform.Find("DeletePopUp").gameObject;
        var popUpContent = deletePopUp.transform.Find("PopUpContent").gameObject;
        
        // SetUp Delete Button
        var deleteButton = entry.transform.Find("DeleteButton").GetComponentInChildren<Button>();
        deleteButton.onClick.RemoveAllListeners();
        deleteButton.onClick.AddListener(() =>
        {
            // SetUp Accept Button
            var acceptDelete = popUpContent.transform.Find("AcceptButton").GetComponent<Button>();
            
            acceptDelete.onClick.RemoveAllListeners();
            acceptDelete.onClick.AddListener(() =>
            {
                DeleteTheme(entry);
                deletePopUp.SetActive(false);
            });

            // SetUp Decline Button
            var declineDelete = popUpContent.transform.Find("DeclineButton").GetComponent<Button>();
            declineDelete.onClick.RemoveAllListeners();
            declineDelete.onClick.AddListener(() =>
            {
                deletePopUp.SetActive(false);
            });
            
            deletePopUp.SetActive(true);
        });
        
    }
    
    private void LoadAllThemes()
    {
        Debug.Log("LoadAllThemes");
        loading.SetActive(true);
        ThemesDatabaseHandler.GetAllThemes(themeList =>
        {
            _keys = new Dictionary<int, string>();
            foreach (var pair in themeList)
            {
                var entry = ThemeToEntry(pair.Key, pair.Value);
                entry.SetActive(true);
            }
            loading.SetActive(false);
        });
    }

    private GameObject ThemeToEntry(string id, Theme theme)
    {
        var entry = Instantiate(_entry, _content.transform);

        entry.transform.Find("Name").GetComponent<TMP_Text>().text = theme.name;
        SetupUpdateButton(entry);
        SetupDeleteButton(entry);
        _keys.Add(entry.GetInstanceID(), id);
        
        return entry;
    }

    private void CreateTheme(string themeName)
    {
        Debug.Log("CreateTheme");
        var obj = new Theme {name = themeName};
        ThemesDatabaseHandler.CreateTheme(obj, (id, theme) =>
        {
            var entry = ThemeToEntry(id, theme);
            entry.SetActive(true);
        });
    }

    private void UpdateTheme(GameObject entry, string themeName)
    {
        Debug.Log("UpdateTheme");
        var id = _keys[entry.GetInstanceID()];
        var theme = new Theme {name = themeName};
        ThemesDatabaseHandler.UpdateTheme(id, theme, () =>
        {
            var oldTheme = entry.transform.Find("Name").GetComponent<TMP_Text>().text;
            QuestionsDatabaseHandler.GetAllQuestionsByTheme(oldTheme, listQuestions =>
            {
                QuestionsDatabaseHandler.UpdateAllQuestionsTheme(theme.name, listQuestions, () => {});
                QuestionsDatabaseHandler.DeleteAllQuestionsFromTheme(oldTheme, () => {});
            });

            entry.transform.Find("Name").GetComponent<TMP_Text>().text = theme.name;
            _questionsPage.needReload = true;
        });

    }
    
    private void DeleteTheme(GameObject entry)
    {
        Debug.Log("DeleteTheme");
        
        Debug.Log("_keys: " + string.Join(Environment.NewLine, _keys));
        var themeId = _keys[entry.GetInstanceID()];
        ThemesDatabaseHandler.DeleteTheme(themeId, success =>
        {
            var themeName = entry.transform.Find("Name").GetComponent<TMP_Text>().text;
            var theme = new Theme {name = themeName};
            QuestionsDatabaseHandler.DeleteAllQuestionsFromTheme(theme.name, () =>
            {
                _keys.Remove(entry.GetInstanceID());
                Destroy(entry);
                _questionsPage.needReload = true;
            });
        });
    }
    
}
