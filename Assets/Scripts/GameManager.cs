using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Board Board;
    public int PiecesCount { get; private set; }
    public int WaveCount { get; private set; }

    public void Initialize()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Another GameManager instance is already initialized.");
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        StartGame();
    }

    public void StartGame()
    {
        Board = new Board();
        Board.InitializeBoard();
    }

    public void OnPieceDefeated()
    {
        PiecesCount--;
        if (PiecesCount <= 0)
        {
            CompleteWave();
        }
    }

    public void CompleteWave()
    {
        if (WaveCount < 6)
        {
            WaveCount++;
            AddPiece();
        }
    }
    public void GameOver()
    {
        Time.timeScale = 0;
    }

    public void AddPiece()
    {
        //Board.TryGetSpawnCell();
        PiecesCount++;
    }
}
