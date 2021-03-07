using System.Collections.Generic;
using Objects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionsPage : MonoBehaviour
{
    
    public GameObject loading;
    
    private GameObject _content;
    private GameObject _entry;
    private Button _updateButton;
    private Button _deleteButton;
    private GameObject _updatePopUp;

    public bool needReload;
    private Dictionary<int, Question> _keys;

    private void Start()
    {
        var viewport = gameObject.transform.Find("Scroll View").Find("Viewport").gameObject;
        _content = viewport.transform.Find("Content").gameObject;
        _entry = viewport.transform.Find("Entry").gameObject;
        
        SetupCreateButton();
        SetupQuestionsTabButton();
        LoadAllQuestions();
    }
    private void SetupQuestionsTabButton()
    {
        var questionsTabButton = gameObject.transform.parent.parent.Find("TabList").Find("QuestionsTab").GetComponent<Button>();
        questionsTabButton.onClick.AddListener(() =>
        {
            if (needReload)
            {
                LoadAllQuestions();
                needReload = false;
            }
        });
    }
    private void SetupCreateButton()
    {
        var createPopUp = gameObject.transform.Find("CreatePopUp").gameObject;
        var contentPopUp = createPopUp.transform.Find("PopUpContent").gameObject;
        
        // SetUp Add Button
        var createButton = gameObject.transform.Find("AddButton").GetComponent<Button>();
        createButton.onClick.RemoveAllListeners();
        createButton.onClick.AddListener(() =>
        {
            var questionInputField = contentPopUp.transform.Find("QuestionInputField").GetComponent<TMP_InputField>();
            questionInputField.text = "";
            var answerInputField = contentPopUp.transform.Find("AnswerInputField").GetComponent<TMP_InputField>();
            answerInputField.text = "";
            var themeDropdown = contentPopUp.transform.Find("ThemeDropdown").GetComponent<Dropdown>();
            themeDropdown.value = 0;
            var difficultyDropdown = contentPopUp.transform.Find("DifficultyDropdown").GetComponent<Dropdown>();
            difficultyDropdown.value = 0;

            ThemesDatabaseHandler.GetAllThemes(callback =>
            {
                var listOptions = new List<string>();
                foreach (var pair in callback)
                {
                    listOptions.Add(pair.Value.name);
                }
                themeDropdown.ClearOptions();
                themeDropdown.AddOptions(listOptions);
            });
            
            // SetUp Save Button
            var saveCreate = contentPopUp.transform.Find("SaveButton").GetComponent<Button>();
            saveCreate.onClick.RemoveAllListeners();
            saveCreate.onClick.AddListener(() =>
            {
                var difficulty = difficultyDropdown.options[difficultyDropdown.value].text;
                var question = questionInputField.text;
                var answer = answerInputField.text;
                var theme = themeDropdown.options[themeDropdown.value].text;
                
                var obj = new Question("", difficulty, question, answer, theme);
                CreateQuestion(obj);
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
        var contentPopUp = _updatePopUp.transform.Find("PopUpContent").gameObject;

        var questionInputField = contentPopUp.transform.Find("QuestionInputField").GetComponent<TMP_InputField>();
        var answerInputField = contentPopUp.transform.Find("AnswerInputField").GetComponent<TMP_InputField>();
        var themeDropdown = contentPopUp.transform.Find("ThemeDropdown").GetComponent<Dropdown>();
        var difficultyDropdown = contentPopUp.transform.Find("DifficultyDropdown").GetComponent<Dropdown>();
        
        // SetUp Update Button
        var updateButton = entry.transform.Find("EditButton").GetComponent<Button>();
        updateButton.onClick.RemoveAllListeners();
        updateButton.onClick.AddListener(() =>
        {
            // Retrieve values from the entry and create the original object
            var question = entry.transform.Find("Question").GetComponent<TMP_Text>().text;
            var answer = entry.transform.Find("Answer").GetComponent<TMP_Text>().text;
            var theme = entry.transform.Find("Theme").GetComponent<TMP_Text>().text;
            var difficulty = entry.transform.Find("Difficulty").GetComponent<TMP_Text>().text;

            // Get all themes possible for the Dropdown
            ThemesDatabaseHandler.GetAllThemes(callback =>
            {
                var listOptions = new List<string>();
                foreach (var pair in callback)
                {
                    listOptions.Add(pair.Value.name);
                }
                themeDropdown.ClearOptions();
                themeDropdown.AddOptions(listOptions);

                // Set original values on the input fields
                questionInputField.text = question;
                answerInputField.text = answer;
                themeDropdown.SetValueWithoutNotify(FindIndexByNameValue(themeDropdown, theme));
                difficultyDropdown.SetValueWithoutNotify(FindIndexByNameValue(difficultyDropdown, difficulty));
            });
            
            // SetUp Save Button
            var saveUpdate = contentPopUp.transform.Find("SaveButton").GetComponent<Button>();
            saveUpdate.onClick.RemoveAllListeners();
            saveUpdate.onClick.AddListener(() =>
            {
                question = questionInputField.text;
                answer = answerInputField.text;
                theme = themeDropdown.options[themeDropdown.value].text;
                difficulty = difficultyDropdown.options[difficultyDropdown.value].text;
            
                var newQuestion = new Question("", difficulty, question, answer, theme);
                
                UpdateQuestion(entry, newQuestion);
                _updatePopUp.SetActive(false);
            });
            
            // SetUp Cancel Button
            var cancelDelete = contentPopUp.transform.Find("CancelButton").GetComponent<Button>();
            cancelDelete.onClick.RemoveAllListeners();
            cancelDelete.onClick.AddListener(() =>
            {
                _updatePopUp.SetActive(false);
            });

            _updatePopUp.SetActive(true);
        });
        
    }

    private static int FindIndexByNameValue(Dropdown dropdown, string value)
    {
        return dropdown.options.FindIndex((i) => i.text.Equals(value));
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
                DeleteQuestion(entry);
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
    
    private void LoadAllQuestions()
    {
        Debug.Log("LoadAllQuestions");
        loading.SetActive(true);
        QuestionsDatabaseHandler.GetAllQuestions(list =>
        {
            foreach (Transform child in _content.transform) {
                Destroy(child.gameObject);
            }
            
            _keys = new Dictionary<int, Question>();
            foreach (var question in list)
            {
                var newEntry = QuestionToEntry(question);
                newEntry.SetActive(true);
            }
            loading.SetActive(false);
        });
    }
    
    private GameObject QuestionToEntry(Question question)
    {
        var newEntry = Instantiate(_entry, _content.transform);
        
        newEntry.transform.Find("Difficulty").GetComponent<TMP_Text>().text = question.difficulty;
        newEntry.transform.Find("Theme").GetComponent<TMP_Text>().text = question.theme;
        newEntry.transform.Find("Question").GetComponent<TMP_Text>().text = question.question;
        newEntry.transform.Find("Answer").GetComponent<TMP_Text>().text = question.rightAnswer;

        SetupUpdateButton(newEntry);
        SetupDeleteButton(newEntry);
        
        _keys.Add(newEntry.GetInstanceID(), question);
        
        return newEntry;
    }

    private void CreateQuestion(Question question)
    {
        QuestionsDatabaseHandler.CreateQuestion(question, callback =>
        {
            var newEntry = QuestionToEntry(question);
            newEntry.SetActive(true);
        });
    }

    private void UpdateQuestion(GameObject entry, Question newQuestion)
    {
        loading.SetActive(true);

        var original = _keys[entry.GetInstanceID()];
        if (!newQuestion.theme.Equals(original.theme))
        {
            QuestionsDatabaseHandler.DeleteQuestion(original, () => {});
        }

        newQuestion.id = original.id;
        QuestionsDatabaseHandler.UpdateQuestion(newQuestion, callback =>
        {
            entry.transform.Find("Question").GetComponent<TMP_Text>().text = newQuestion.question;
            entry.transform.Find("Answer").GetComponent<TMP_Text>().text = newQuestion.rightAnswer;
            entry.transform.Find("Theme").GetComponent<TMP_Text>().text = newQuestion.theme;
            entry.transform.Find("Difficulty").GetComponent<TMP_Text>().text = newQuestion.difficulty;

            _keys[entry.GetInstanceID()] = newQuestion;
            loading.SetActive(false);
        });
    }

    private void DeleteQuestion(GameObject entry)
    {
        var question = _keys[entry.GetInstanceID()];
        QuestionsDatabaseHandler.DeleteQuestion(question, () =>
        {
            entry.SetActive(false);
        });
    }
    
}
