using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private Button loginButton;
    [SerializeField] private string gameSceneName = "SampleScene";

    [Header("Fade")]
    [SerializeField] private Image fadeOverlay;
    [SerializeField] private float fadeDuration = 1.5f;

    [Header("Transition Sound")]
    [SerializeField] private AudioSource transitionAudioSource;
    [SerializeField] private AudioClip transitionClip;

    private void Start()
    {
        loginButton.onClick.AddListener(OnLoginButtonClicked);

        if (fadeOverlay != null)
            fadeOverlay.color = new Color(1f, 1f, 1f, 0f);
    }

    private void OnLoginButtonClicked()
    {
        string username = usernameField.text.Trim();
        string password = passwordField.text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return;

        StartCoroutine(LoginCoroutine(username, password));
    }

    private IEnumerator LoginCoroutine(string username, string password)
    {
        LoginAnswer answer = null;

        yield return ApiRequest.LoginCoroutine(
            result => answer = result,
            new LoginRequest { Username = username, Password = password });

        if (answer != null && answer.Status)
            yield return FadeToWhiteAndLoad();
        else
            Debug.LogWarning("Login failed.");
    }

    private IEnumerator FadeToWhiteAndLoad()
    {
        loginButton.interactable = false;

        if (transitionClip != null && transitionAudioSource != null)
            transitionAudioSource.PlayOneShot(transitionClip);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);
            fadeOverlay.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        SceneManager.LoadScene(gameSceneName);
    }
}
