using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public Cell[,] Board = new Cell[8, 8];
    public int NumberOfPieces = 6;

    // Update is called once per frame
    private void StartGame()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                Board[x, y] = new Cell(x, y);
            }
        }
    }
}
