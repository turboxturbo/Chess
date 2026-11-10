using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameResultView : MonoBehaviour
{
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private TextMeshProUGUI loseText;
    [SerializeField] private float fastBlinkDuration = 4f;
    [SerializeField] private float fastBlinkInterval = 0.05f;
    [SerializeField] private float slowBlinkInterval = 0.35f;

    private void Start()
    {
        winPanel.SetActive(false);
        losePanel.SetActive(false);

        GameManager.Instance.GameWon += ShowWin;
        GameManager.Instance.GameOverEvent += ShowLose;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameWon -= ShowWin;
            GameManager.Instance.GameOverEvent -= ShowLose;
        }
    }

    public void ShowWin()
    {
        winPanel.SetActive(true);
        StartCoroutine(BlinkText(winText));
    }

    public void ShowLose()
    {
        losePanel.SetActive(true);
        StartCoroutine(BlinkText(loseText));
    }

    private IEnumerator BlinkText(TextMeshProUGUI text)
    {
        float elapsed = 0f;
        while (elapsed < fastBlinkDuration)
        {
            text.enabled = !text.enabled;
            yield return new WaitForSecondsRealtime(fastBlinkInterval);
            elapsed += fastBlinkInterval;
        }

        while (true)
        {
            text.enabled = !text.enabled;
            yield return new WaitForSecondsRealtime(slowBlinkInterval);
        }
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
