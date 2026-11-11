using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private string targetScene = "Auth";

    private void Start()
    {
        button.onClick.AddListener(() => SceneManager.LoadScene(targetScene));
    }
}
