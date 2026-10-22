using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Board Board;
    public int PiecesCount { get; private set; }
    public int WaveCount { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        StartGame();
    }
    public void StartGame()
    {
        Board = new Board();
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
        }
        
    }
    public void GameOver()
    {
        
    }

    public void AddPiece()
    {
        PiecesCount++;
    }
}