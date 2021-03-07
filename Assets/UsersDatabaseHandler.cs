using Objects;
using Proyecto26;
using UnityEngine;

public class UsersDatabaseHandler : MonoBehaviour
{
    
    private const string FirebaseURL = ConfigFirebase.FirebaseURL;
    
    public delegate void GetUserCallback(User user);
    public static void GetUser(string userName, GetUserCallback callback)
    {
        RestClient.Get($"{FirebaseURL}users/{userName}.json").Then(response =>
        {
            var jsonResponse = "{\""+userName+"\":"+response.Text+"}";
            Debug.Log("RESPONSE - " + response.Text);
            Debug.Log("RESPONSE - " + jsonResponse);
            var json = new JSONObject(jsonResponse);
            callback(User.FromJson(json));
        }).Catch(ex =>
        {
            Debug.LogError("Exception - " + ex.Message);
            callback(null);
        });
    }

    public delegate void CreateUserCallback(bool userCreated);
    public static void CreateUser(User data, CreateUserCallback callback)
    {
        RestClient.Put($"{FirebaseURL}users/{data.name}/.json", data).Then(response =>
        {
            callback(true);
        }).Catch(ex =>
        {
            Debug.LogError("Exception - " + ex.Message);
            callback(false);
        });
    }
    
}
