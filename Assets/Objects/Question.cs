using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Objects
{
    [Serializable]
    public class Question
    {

        [NonSerialized]
        public string id;
        public string question;
        public string rightAnswer;
        public string theme;
        public string difficulty;

        public Question()
        {
        }
        
        public Question(string id, string difficulty, string question, string rightAnswer, string theme)
        {
            this.id = id;
            this.difficulty = difficulty;
            this.question = question;
            this.rightAnswer = rightAnswer;
            this.theme = theme;
        }

        public static List<Question> ListFromJson(JSONObject jsonObject)
        {
            var list = new List<Question>();
            if (jsonObject.IsNull) return list;

            foreach (var key in jsonObject.keys)
            {
                var obj = jsonObject[key];
                
                var id = key;
                var difficulty = UtfFormat(obj.GetField("difficulty").str);
                var question = UtfFormat(obj.GetField("question").str);
                var rightAnswer = UtfFormat(obj.GetField("rightAnswer").str);
                var theme = UtfFormat(obj.GetField("theme").str);
                
                list.Add(new Question(id, difficulty, question, rightAnswer, theme));
            }
            
            return list;
        }

        private static string UtfFormat(string original)
        {
            var bytes = Encoding.Default.GetBytes(original);
            return Encoding.UTF8.GetString(bytes);
        }
        
        // Method not being used
        public static Dictionary<string, Question> FirebaseJsonToDictionary(JSONObject jsonObject)
        {
            Debug.Log("STARTED - FirebaseJsonToDictionary");
            var dictionary = new Dictionary<string, Question>();
            
            if (jsonObject.IsNull) return dictionary;

            foreach (var key in jsonObject.keys)
            {
                var obj = jsonObject[key];
                
                var id = key;
                var difficulty = UtfFormat(obj.GetField("difficulty").str);
                var question = UtfFormat(obj.GetField("question").str);
                var rightAnswer = UtfFormat(obj.GetField("rightAnswer").str);
                var theme = UtfFormat(obj.GetField("theme").str);
                
                dictionary.Add(key, new Question( id, difficulty, question, rightAnswer, theme));
            }

            return dictionary;
        }
        
    }
    
}