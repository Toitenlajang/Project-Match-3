using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MovesManager : MonoBehaviour
{
    public TextMeshProUGUI movesText;
    public int movesRemaining;
    public int startingMoves = 69;

    private Board board;
    // Start is called before the first frame update
    void Start()
    {
        board  = FindObjectOfType<Board>();
        movesRemaining = startingMoves;
        UpdateMovesText();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovesText();
    }
    public void UseMove()
    {
        movesRemaining--;
        UpdateMovesText();

        if (movesRemaining <= 0)
        {
            // Game over !!!
            movesRemaining = 0;
            Debug.Log("Game over!");

            // Disable further moves
            if(board != null)
            {
                board.currentState = GameState.wait;
            }
        }
    }

    private void UpdateMovesText()
    {
        if(movesText != null)
        {
            movesText.text = movesRemaining.ToString();
        }
    }
    public bool HasMovesRemaining()
    {
        return movesRemaining > 0;
    }
}
