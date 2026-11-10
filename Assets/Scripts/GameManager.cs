using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Board Board { get; private set; }
    public int WaveCount { get; private set; }

    public event Action<ChessPiece, int, int> PieceSpawnRequested;
    public event Action WaveCleared;
    public event Action GameWon;
    public event Action GameOverEvent;

    private const int MaxWaves = 10;

    public void Initialize()
    {
        if (Instance != null && Instance != this)
            Destroy(Instance.gameObject);

        Instance = this;
        StartGame();
    }

    public void StartGame()
    {
        Board = new Board();
        Board.InitializeBoard();
        WaveCount = 1;
    }

    public void StartFirstWave()
    {
        SpawnWave();
    }

    public void OnWhiteKingDefeated()
    {
        if (WaveCount >= MaxWaves)
        {
            GameWon?.Invoke();
            return;
        }

        WaveCount++;
        WaveCleared?.Invoke();
        SpawnWave();
    }

    // Фиксированные позиции базового состава
    private static readonly (int x, int y)[] BaseFormation =
    {
        (3, 0), // WhiteKing
        (2, 0), // Knight
        (4, 0), // Bishop
        (2, 1), // Pawn
        (3, 1), // Pawn
        (4, 1), // Pawn
    };

    private void SpawnWave()
    {
        // Базовый состав — на фиксированных позициях
        var basePieces = new ChessPiece[]
        {
            new WhiteKing(8),
            new Knight(PieceColor.White, 4),
            new Bishop(PieceColor.White, 4),
            new Pawn(PieceColor.White, 3),
            new Pawn(PieceColor.White, 3),
            new Pawn(PieceColor.White, 3),
        };

        for (int i = 0; i < basePieces.Length; i++)
        {
            var (x, y) = BaseFormation[i];
            Board.PlacePiece(basePieces[i], x, y);
            PieceSpawnRequested?.Invoke(basePieces[i], x, y);
        }

        // Дополнительные фигуры по волнам — в свободные клетки
        int extraPawns = Math.Min(WaveCount - 1, 5);
        for (int i = 0; i < extraPawns; i++)
            SpawnExtra(new Pawn(PieceColor.White, 3));

        if (WaveCount >= 7)  SpawnExtra(new Bishop(PieceColor.White, 4));
        if (WaveCount >= 8)  SpawnExtra(new Knight(PieceColor.White, 4));
        if (WaveCount >= 9)  SpawnExtra(new Rook(PieceColor.White, 4));
        if (WaveCount >= 10) SpawnExtra(new Queen(PieceColor.White, 5));
    }

    private void SpawnExtra(ChessPiece piece)
    {
        if (!Board.SpawnWhitePiece(piece, out int x, out int y))
        {
            Debug.LogWarning($"Wave {WaveCount}: no free cell for {piece.GetType().Name}");
            return;
        }
        PieceSpawnRequested?.Invoke(piece, x, y);
    }

    public bool IsGameOver { get; private set; }

    public void GameOver()
    {
        IsGameOver = true;
        GameOverEvent?.Invoke();
        Time.timeScale = 0;
    }
}
