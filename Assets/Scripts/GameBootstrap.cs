using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private BoardView boardView;
    [SerializeField] private PlayerInputController playerInputController;
    [SerializeField] private Camera worldCamera;
    private AIController _aiController;
    private bool _aiTurnInProgress;

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
        InitializeAI();
    }

    public void InitializeAI()
    {
        MoveGenerator moveGenerator = new MoveGenerator();
        MoveEvaluationWeights weights =
            new MoveEvaluationWeights();

        IShotgunThreatProvider threatProvider =
            new NoShotgunThreatProvider();

        MoveEvaluator moveEvaluator = new MoveEvaluator(
            threatProvider,
            moveGenerator,
            weights);

        _aiController = new AIController(
            moveGenerator,
            moveEvaluator,
            weights);

        _aiController.MoveExecuted += boardView.ApplyMove;
        playerInputController.PlayerMoveCompleted += MakeAITurn;
    }

     private void MakeAITurn()
    {
        if (_aiController == null || _aiTurnInProgress)
        {
            return;
        }

        _aiTurnInProgress = true;
        playerInputController.SetInputEnabled(false);

        bool blackKingCaptured =
            _aiController.MakeTurn(gameManager.Board);

        if (blackKingCaptured)
        {
            gameManager.GameOver();
            return;
        }

        playerInputController.SetInputEnabled(true);
        _aiTurnInProgress = false;
    }

     private void OnDestroy()
    {
        if (_aiController != null)
        {
            _aiController.MoveExecuted -= boardView.ApplyMove;
        }

        if (playerInputController != null)
        {
            playerInputController.PlayerMoveCompleted -= MakeAITurn;
        }
    }
}
