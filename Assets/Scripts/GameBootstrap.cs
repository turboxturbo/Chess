using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private BoardView boardView;
    [SerializeField] private PlayerInputController playerInputController;
    [SerializeField] private Camera worldCamera;


    private void Awake()
    {
        InitializeGame();
    }

    public void InitializeGame()
    {
        if (gameManager == null ||
            boardManager == null ||
            boardView == null ||
            playerInputController == null ||
            worldCamera == null)
        {
            Debug.LogError("GameBootstrap references are not assigned.");
            return;
        }

        gameManager.Initialize();
        boardView.Initialize(gameManager.Board);

        playerInputController.Initialize(
            gameManager.Board,
            boardManager,
            boardView,
            worldCamera);
    }
}
