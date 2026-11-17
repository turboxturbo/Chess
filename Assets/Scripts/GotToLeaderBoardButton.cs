using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GotToLeaderBoardButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private string targetScene = "BoardScene";

    private void Start()
    {
        button.onClick.AddListener(() => SceneManager.LoadScene(targetScene));
    }
}
