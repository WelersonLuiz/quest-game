using System.Collections.Generic;
using Objects;
using Proyecto26;
using UnityEngine;

public class QuestionsDatabaseHandler : MonoBehaviour
{
    
    private const string FirebaseURL = ConfigFirebase.FirebaseURL;

    public delegate void GetAllQuestionsCallback(List<Question> questionList);
    public static void GetAllQuestions(GetAllQuestionsCallback callback)
    {
        var questionList = new List<Question>();
        RestClient.Get($"{FirebaseURL}questionList/.json").Then(response =>
        {
            var json = response.Text;
            Debug.Log("Response - " + json);
            var jsonObject = new JSONObject(json);

            if (jsonObject.IsNull) callback(questionList);

            foreach (var theme in jsonObject.keys)
            {
                var themeObj = jsonObject[theme];
                var list = QuestionsFromThemeObject(themeObj, theme);
                questionList.AddRange(list);
            }

            callback(questionList);
        }).Catch(ex =>
        {
            Debug.LogError("Exception - " + ex.Message);
            callback(questionList);
        });
    }

    private static IEnumerable<Question> QuestionsFromThemeObject(JSONObject themeObj, string theme)
    {
        var list = new List<Question>();
        
        foreach (var key in themeObj.keys)
        {
            var obj = themeObj[key];
                
            var difficulty = obj.GetField("difficulty").str;
            var question = obj.GetField("question").str;
            var rightAnswer = obj.GetField("rightAnswer").str;
                
            list.Add(new Question(key, difficulty, question, rightAnswer, theme));
        }

        return list;
    }
    
    
    public delegate void CreateQuestionCallback(Question question);
    public static void CreateQuestion(Question question, CreateQuestionCallback callback)
    {
        RestClient.Post($"{FirebaseURL}questionList/{question.theme}/.json", question).Then(response =>
        {
            var json = new JSONObject(response.Text);
            question.id = json.GetField("name").str;
            callback(question); 
        }).Catch(ex =>
        {
            Debug.LogError("Exception - " + ex.Message);
            callback(question);
        });
    }

    
    public delegate void UpdateQuestionCallback(Question question);
    public static void UpdateQuestion(Question question, UpdateQuestionCallback callback)
    {
        RestClient.Put($"{FirebaseURL}questionList/{question.theme}/{question.id}.json", question).Then(response =>
        {
            callback(question); 
        }).Catch(ex =>
        {
            Debug.LogError("Exception - " + ex.Message);
            callback(question);
        });
    }
    
    public delegate void DeleteQuestionCallback();
    public static void DeleteQuestion(Question question, DeleteQuestionCallback callback)
    {
        RestClient.Delete($"{FirebaseURL}questionList/{question.theme}/{question.id}.json").Then(response =>
        {
            callback();
        }).Catch(ex =>
        {
            Debug.LogError("Exception - " + ex.Message);
            callback();
        });
    }

    public delegate void UpdateAllQuestionsCallback();
    public static void UpdateAllQuestionsTheme(string newTheme, string questionsList, UpdateAllQuestionsCallback callback)
    {
        
        RestClient.Put($"{FirebaseURL}questionList/{newTheme}/.json", questionsList).Then(response =>
        {
            callback(); 
        }).Catch(ex =>
        {
            Debug.LogError("Exception - " + ex.Message);
            callback();
        });
    }
    
    
    public delegate void GetAllQuestionsByThemeCallback(string listQuestions);
    public static void GetAllQuestionsByTheme(string theme, GetAllQuestionsByThemeCallback callback)
    {
        var json = "";
        RestClient.Get($"{FirebaseURL}questionList/{theme}.json").Then(response =>
        {
            json = response.Text;
            callback(json);
        }).Catch(ex =>
        {
            Debug.LogError("Exception - " + ex.Message);
            callback(json);
        });
        
    }
    
    public delegate void DeleteAllQuestionsCallback();
    public static void DeleteAllQuestionsFromTheme(string theme, DeleteAllQuestionsCallback callback)
    {
        RestClient.Delete($"{FirebaseURL}questionList/{theme}.json").Then(response =>
        {
            callback();
        }).Catch(ex =>
        {
            Debug.LogError("Exception - " + ex.Message);
            callback();
        });
    }
}