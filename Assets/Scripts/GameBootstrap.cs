using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private BoardView boardView;
    [SerializeField] private PlayerInputController playerInputController;
    [SerializeField] private Camera worldCamera;
    [SerializeField, Min(1)] private int userId = 1;

    private AIController _aiController;
    private bool _aiTurnInProgress;
    private bool _skipAiTurn;
    private int _gameId;
    private bool _gameFinished;
    private bool _gameEndSent;

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

        gameManager.PieceSpawnRequested += boardView.SpawnPieceView;
        gameManager.WaveCleared += OnWaveCleared;
        gameManager.GameWon += OnGameWon;
        gameManager.GameOverEvent += OnGameOver;

        gameManager.Initialize();
        boardView.Initialize(gameManager.Board);
        gameManager.StartFirstWave();

        playerInputController.Initialize(
            gameManager.Board,
            boardManager,
            boardView,
            worldCamera);
        InitializeAI();
        CreateGameRecord();
        StartCoroutine(WaitForDropsThenEnableInput());
    }

    public void InitializeAI()
    {
        MoveGenerator moveGenerator = new MoveGenerator();
        MoveEvaluationWeights weights = new MoveEvaluationWeights();
        IShotgunThreatProvider threatProvider = new NoShotgunThreatProvider();

        MoveEvaluator moveEvaluator = new MoveEvaluator(
            threatProvider,
            moveGenerator,
            weights);

        _aiController = new AIController(
            moveGenerator,
            moveEvaluator,
            weights);

        _aiController.MoveExecuted += boardView.ApplyMove;
        _aiController.PawnPromoted += boardView.ApplyPromotion;
        playerInputController.PlayerMoveCompleted += StartAITurn;
    }

    private void StartAITurn()
    {
        if (_aiController == null || _aiTurnInProgress) return;

        if (_skipAiTurn)
        {
            _skipAiTurn = false;
            return;
        }

        StartCoroutine(AITurnCoroutine());
    }

    private System.Collections.IEnumerator AITurnCoroutine()
    {
        _aiTurnInProgress = true;
        playerInputController.SetInputEnabled(false);

        bool blackKingCaptured = _aiController.MakeTurn(gameManager.Board);

        while (boardView.IsAnyPieceMoving())
            yield return null;

        if (blackKingCaptured)
        {
            gameManager.GameOver();
            yield break;
        }

        playerInputController.SetInputEnabled(true);
        _aiTurnInProgress = false;
    }

    private void OnWaveCleared()
    {
        _skipAiTurn = true;
        boardView.ResetSpawnIndex();
        boardView.ClearWhitePieces();
        gameManager.Board.ClearWhitePieces();
        gameManager.Board.ResetBlackKing(4, 7);
        boardView.ResetBlackKingView(4, 7);
        playerInputController.Ammo.Reset();
        playerInputController.SetInputEnabled(false);
        StartCoroutine(WaitForDropsThenEnableInput());
    }

    private System.Collections.IEnumerator WaitForDropsThenEnableInput()
    {
        playerInputController.SetInputEnabled(false);
        yield return null; // ждём кадр чтобы SpawnWave успел выполниться
        while (boardView.IsAnyPieceMoving())
            yield return null;
        playerInputController.SetInputEnabled(true);
    }

    private void OnGameWon()
    {
        Debug.Log("You survived all 10 waves!");
        FinishGameRecord();
        playerInputController.SetInputEnabled(false);
        Time.timeScale = 0;
    }

    private void OnGameOver()
    {
        FinishGameRecord();
    }

    private void CreateGameRecord()
    {
        ApiRequest.StartGame(OnGameCreated, new GameStartRequest
        {
            IdUser = userId,
            WavesCount = gameManager.WaveCount,
            Status = true
        });
    }

    private void OnGameCreated(GameStartAnswer answer)
    {
        if (answer == null || !answer.status || answer.data == null)
        {
            Debug.LogError("The API did not return a game ID.");
            return;
        }

        _gameId = answer.data.idGame;

        if (_gameFinished)
        {
            SendEndGameRequest();
        }
    }

    private void FinishGameRecord()
    {
        _gameFinished = true;
        SendEndGameRequest();
    }

    private void SendEndGameRequest()
    {
        if (_gameId <= 0 || _gameEndSent)
        {
            return;
        }

        _gameEndSent = true;
        ApiRequest.EndGame(null, _gameId);
    }

    private void OnDestroy()
    {
        if (_aiController != null)
        {
            _aiController.MoveExecuted -= boardView.ApplyMove;
            _aiController.PawnPromoted -= boardView.ApplyPromotion;
        }

        if (playerInputController != null)
            playerInputController.PlayerMoveCompleted -= StartAITurn;

        if (gameManager != null)
        {
            gameManager.PieceSpawnRequested -= boardView.SpawnPieceView;
            gameManager.WaveCleared -= OnWaveCleared;
            gameManager.GameWon -= OnGameWon;
            gameManager.GameOverEvent -= OnGameOver;
        }
    }
}
