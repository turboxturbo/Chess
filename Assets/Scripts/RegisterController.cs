using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RegisterController : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private Button registerButton;
    [SerializeField] private string loginScene = "Auth";

    private bool _isRegistering;

    private void Start()
    {
        registerButton.onClick.RemoveAllListeners();
        registerButton.onClick.AddListener(OnRegisterButtonClicked);
    }

    private void OnRegisterButtonClicked()
    {
        if (_isRegistering) return;

        string username = usernameField.text.Trim();
        string password = passwordField.text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowError(usernameField, "Enter a username");
            ShowError(passwordField, "Enter a password");
            return;
        }

        StartCoroutine(RegisterCoroutine(username, password));
    }

    private IEnumerator RegisterCoroutine(string username, string password)
    {
        _isRegistering = true;
        registerButton.interactable = false;

        RegAnswer answer = null;

        yield return ApiRequest.RegCoroutine(
            result => answer = result,
            new RegRequest { Username = username, Password = password });

        if (answer != null && answer.status)
        {
            SceneManager.LoadScene(loginScene);
        }
        else
        {
            string error = answer != null && !string.IsNullOrEmpty(answer.message)
                ? answer.message
                : "Registration failed";
            ShowError(usernameField, error);
            ShowError(passwordField, error);
        }

        registerButton.interactable = true;
        _isRegistering = false;
    }

    private void ShowError(TMP_InputField field, string message)
    {
        field.text = "";
        field.placeholder.GetComponent<TMP_Text>().text = " " + message.TrimStart();
    }
}
