using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MenuButton : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(OpenMenu);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(OpenMenu);
    }

    private void OpenMenu()
    {
        SceneManager.LoadScene("LoginScene");
    }
}
