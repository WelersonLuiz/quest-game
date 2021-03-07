using TMPro;
using UnityEngine;
using UnityEngine.UI;
using User = Objects.User;

public class LoginMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject adminMenu;
    public GameObject loading;

    private GameObject _errorMessage;
    private TMP_InputField _userName;
    private TMP_InputField _password;
    private Button _loginButton;
    private Button _signUpButton;

    private GameObject _signUpPopUp;
    private GameObject _popUpContent;
    
    private void Start()
    {
        _userName = gameObject.transform.Find("UserNameInput").GetComponent<TMP_InputField>();
        _password = gameObject.transform.Find("PasswordInput").GetComponent<TMP_InputField>();
        _errorMessage = gameObject.transform.Find("ErrorMessage").gameObject;
        _errorMessage.SetActive(false);
        
        _loginButton = gameObject.transform.Find("LoginButton").GetComponent<Button>();
        _loginButton.onClick.AddListener(VerifyUser);

        _signUpPopUp = gameObject.transform.Find("SignUpPopUp").gameObject;
        _popUpContent = _signUpPopUp.transform.Find("PopUpContent").gameObject;
        
        var signUpButton = gameObject.transform.Find("SignUpButton").GetComponent<Button>();
        signUpButton.onClick.AddListener(() =>
        {
            _signUpPopUp.SetActive(true);
        });
        
        var cancelButton = _popUpContent.transform.Find("CancelButton").GetComponent<Button>();
        cancelButton.onClick.AddListener(() =>
        {
            _signUpPopUp.SetActive(false);
        });

        var saveButton = _popUpContent.transform.Find("SaveButton").GetComponent<Button>();
        saveButton.onClick.AddListener(SignUp);

    }

    private void VerifyUser()
    {
        Debug.Log("LoginMenu.VerifyUser - Verifying Login Info...");
        loading.SetActive(true);
        _errorMessage.SetActive(false);
        
        UsersDatabaseHandler.GetUser(_userName.text, user =>
        {
            
            if (user == null || !user.password.Equals(_password.text))
            {
                FindObjectOfType<ToastController>().CallBadToast("Invalid user");
                Debug.Log("LoginMenu.VerifyUser - User Invalid");
                _errorMessage.SetActive(true);
            }
            else
            {
                Debug.Log("LoginMenu.VerifyUser - User Valid");
                LoginUser(user);
                _userName.text = "";
                _password.text = "";
            }
            
            loading.SetActive(false);
        });

    }

    private void LoginUser(User user)
    {
        gameObject.SetActive(false);
        
        if (user.type.Equals("admin"))
        {
            FindObjectOfType<ToastController>().CallGoodToast("Valid super user");
            adminMenu.SetActive(true);
        }
        else
        {
            FindObjectOfType<ToastController>().CallGoodToast("Valid user");
            mainMenu.SetActive(true);
        }
        
        loading.SetActive(false);
    }

    private void SignUp()
    {
        var userName = _popUpContent.transform.Find("UserNameInputField").GetComponent<TMP_InputField>().text;
        var password = _popUpContent.transform.Find("PasswordInputField").GetComponent<TMP_InputField>().text;
        var confirmPassword = _popUpContent.transform.Find("ConfirmPasswordInputField").GetComponent<TMP_InputField>().text;
        
        loading.SetActive(true);
        
        if (!password.Equals(confirmPassword)) 
        {
            loading.SetActive(false);
            Debug.Log("As senhas devem ser iguais");
            return;
        }
        
        UsersDatabaseHandler.GetUser(userName, callback =>
        {
            if (callback != null) 
            {
                loading.SetActive(false);
                Debug.Log("Usuário já existe");
                return;
            }
            
            var user = new User(userName, password, "player", null);
            UsersDatabaseHandler.CreateUser(user, userCreated =>
            {
                if (userCreated)
                {
                    Debug.Log("Usuário criado");
                    _signUpPopUp.SetActive(false);   
                }
                else
                {
                    Debug.Log("Falha ao criar usuário");
                }
                
                loading.SetActive(false);
            });
        });
    }
    
}