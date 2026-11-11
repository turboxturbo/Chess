using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoToRegButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private string targetScene = "RegScene";

    private void Start()
    {
        button.onClick.AddListener(() => SceneManager.LoadScene(targetScene));
    }
}
