using UnityEngine;
using UnityEngine.InputSystem;

public class AuthSceneController : MonoBehaviour
{
    [SerializeField] private DialogueTyper dialogueTyper;
    [SerializeField, TextArea(3, 10)] private string openingText = "* hey. you.\n* do you think you can win?";

    private void Start()
    {
        dialogueTyper.Play(openingText);
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
            dialogueTyper.Skip();
    }
}
