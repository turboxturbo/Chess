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

    [Header("Sans Trick")]
    [SerializeField] private DialogueTyper dialogueTyper;
    [SerializeField, TextArea(2, 5)] private string trickText = "* heh.\n* you really thought it'd be that easy?";
    [SerializeField, TextArea(2, 5)] private string secondText = "* ...ok fine. go ahead.";
    [SerializeField] private Vector2 trickButtonPosition = new Vector2(-584.4999f, 297.5f);

    private bool _tricked;

    private void Start()
    {
        loginButton.onClick.AddListener(OnLoginButtonClicked);

        if (fadeOverlay != null)
            fadeOverlay.color = new Color(1f, 1f, 1f, 0f);
    }

    private void OnLoginButtonClicked()
    {
        if (_tricked)
        {
            StartCoroutine(FadeToWhiteAndLoad());
            return;
        }

        string username = usernameField.text.Trim();
        string password = passwordField.text.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            return;

        StartCoroutine(LoginCoroutine(username, password));
    }

    private IEnumerator LoginCoroutine(string username, string password)
    {
        loginButton.interactable = false;

        LoginAnswer answer = null;

        yield return ApiRequest.LoginCoroutine(
            result => answer = result,
            new LoginRequest { Username = username, Password = password });

        if (answer != null && answer.status)
        {
            TeleportButton();
            if (dialogueTyper != null)
            {
                dialogueTyper.Play(trickText);
                dialogueTyper.TypewriterFinished += OnTrickTextFinished;
            }
            _tricked = true;
        }
        else
        {
            usernameField.text = "";
            passwordField.text = "";
            usernameField.placeholder.GetComponent<TMP_Text>().text = "Неверный логин или пароль";
            passwordField.placeholder.GetComponent<TMP_Text>().text = "Неверный логин или пароль";
        }

        loginButton.interactable = true;
    }

    private void OnTrickTextFinished()
    {
        dialogueTyper.TypewriterFinished -= OnTrickTextFinished;
        dialogueTyper?.Play(secondText);
    }

    private void TeleportButton()
    {
        loginButton.GetComponent<RectTransform>().anchoredPosition = trickButtonPosition;
    }

    private IEnumerator FadeToWhiteAndLoad()
    {
        loginButton.interactable = false;

        if (transitionClip != null && transitionAudioSource != null)
        {
            transitionAudioSource.pitch = transitionClip.length / fadeDuration;
            transitionAudioSource.clip = transitionClip;
            transitionAudioSource.Play();
        }

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
