using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Board Board;
    void Start()
    {
        StartGame();
    }

    // Update is called once per frame
    private void StartGame()
    {
        Board = new Board();

    }
}
