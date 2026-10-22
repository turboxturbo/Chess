using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Board Board;
    public int PiecesCount { get; private set; }
    void Start()
    {
        StartGame();
    }

    // Update is called once per frame
    private void StartGame()
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
        
    }
    public void GameOver()
    {
        Debug.Log("Game Over");
    }

    public void AddPiece()
    {
        PiecesCount++;
    }
}
