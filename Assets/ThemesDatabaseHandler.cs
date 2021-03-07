using System.Collections.Generic;
using Objects;
using Proyecto26;
using UnityEngine;

public class ThemesDatabaseHandler : MonoBehaviour
{
    
    private const string FirebaseURL = ConfigFirebase.FirebaseURL;

    public delegate void GetAllThemesCallback(Dictionary<string, Theme> themeList);
    public static void GetAllThemes(GetAllThemesCallback callback)
    {
        var themeList = new Dictionary<string, Theme>();
        RestClient.Get($"{FirebaseURL}themes/.json").Then(response =>
        {
            var jsonResponse = new JSONObject(response.Text);

            foreach (var key in jsonResponse.keys)
            {
                var theme = jsonResponse[key];
                var value = JsonUtility.FromJson<Theme>(theme.ToString());
                themeList.Add(key, value);
            }

            callback(themeList);
        }).Catch(ex =>
        {
            Debug.LogError("Exception - " + ex.Message);
            callback(themeList);
        });
    }

    
    public delegate void CreateThemeCallback(string id, Theme theme);
    public static void CreateTheme(Theme theme, CreateThemeCallback callback)
    {
        RestClient.Post($"{FirebaseURL}themes/.json", theme).Then(response =>
        {
            var json = new JSONObject(response.Text);
            var key = json.GetField("name").str;
            callback(key, theme);
        }).Catch(ex =>
        {
            Debug.LogError("Exception - " + ex.Message);
            callback(null, null);
        });
    }

    
    public delegate void UpdateThemeCallback();
    public static void UpdateTheme(string themeId, Theme theme, UpdateThemeCallback callback)
    {
        RestClient.Put($"{FirebaseURL}themes/{themeId}.json", theme).Then(response =>
        {
            callback();
        }).Catch(ex =>
        {
            Debug.LogError("Exception - " + ex.Message);
            callback();
        });
    }
    
    
    public delegate void DeleteThemeCallback(bool success);
    public static void DeleteTheme(string themeId, DeleteThemeCallback callback)
    {
        RestClient.Delete($"{FirebaseURL}themes/{themeId}.json").Then(response =>
        {
            callback(true);
        }).Catch(ex =>
        {
            Debug.LogError("Exception - " + ex.Message);
            callback(false);
        });
    }
    
}
