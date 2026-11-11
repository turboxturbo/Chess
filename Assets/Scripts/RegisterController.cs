using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterController : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private Button registerButton;

    private void Start()
    {
        registerButton.onClick.AddListener(OnRegisterButtonClicked);
    }

    private void OnRegisterButtonClicked()
    {
        string username = usernameField.text.Trim();
        string password = passwordField.text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return;

        StartCoroutine(RegisterCoroutine(username, password));
    }

    private IEnumerator RegisterCoroutine(string username, string password)
    {
        registerButton.interactable = false;

        RegAnswer answer = null;

        yield return ApiRequest.RegCoroutine(
            result => answer = result,
            new RegRequest { Username = username, Password = password });

        if (answer != null && answer.Status)
            Debug.Log("Registration successful.");
        else
            Debug.LogWarning("Registration failed.");

        registerButton.interactable = true;
    }
}
