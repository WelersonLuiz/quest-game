using System;
using System.Collections.Generic;

namespace Objects
{
    [Serializable]
    public class User
    {

        public string name;
        public string password;
        public string type;
        public Dictionary<string, string> scores;

        public User()
        {
        }
        
        public User(string name, string password, string type, Dictionary<string, string> scores)
        {
            this.name = name;
            this.password = password;
            this.type = type;
            this.scores = scores;
        }
        
        public static User FromJson(JSONObject json)
        {
            if (json.IsNull) return null;

            var name = json.keys[0];
            var obj = json[name];
            var password = obj.GetField("password").str;
            var type = obj.GetField("type").str;
            var jsonScore = obj.GetField("scores");

            var score = new Dictionary<string, string>();

            if (jsonScore != null)
            {
                foreach (var theme in jsonScore.keys)
                {
                    score.Add(theme, jsonScore[theme].str);
                }   
            }
            
            return new User(name, password, type, score);
        }
    }
}