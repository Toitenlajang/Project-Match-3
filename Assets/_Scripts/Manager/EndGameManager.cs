using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameResult
{
    Playing,
    Win,
    Lose

}
public class EndGameManager : MonoBehaviour
{
    public Board board;
    public MovesManager movesManager;
    public ScoreManager scoreManager;

    [Header("End Game UI")]
    public GameObject winPanel;
    public GameObject losePanel;

    [Header("Game State")]
    public GameResult currentGameResult = GameResult.Playing;
    private bool gameEnded = false;

    [Header("Win Rewards")]
    public int baseReward = 100;
    public int bonusPoints = 1;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        movesManager = FindObjectOfType<MovesManager>();
        scoreManager = FindObjectOfType<ScoreManager>();

        // Initially hide win/lose panels
        if(winPanel != null)
        {
            winPanel.SetActive(false);
        }
        if(losePanel != null)
        {
            losePanel.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Only check once when game hasn't ended yet
        if (!gameEnded)
        {
            CheckEndGameConditions();
        }
    }
    private void CheckEndGameConditions()
    {
        // Check for lose condition: no moves left and score below target
        if (movesManager.movesRemaining == 0 && board.currentState == GameState.move)
        {

            if (scoreManager.score < board.scoreGoals)
            {
                // Player lost
                EndGame(GameResult.Lose);
            }
            else
            {
                // Player won (reached goal with exactly 0 moves)
                EndGame(GameResult.Win);
            }
        }
        // Check for win condition: reached score goal
        else if (scoreManager.score >= board.scoreGoals && !gameEnded)
        {
            // Player won before running out of moves
            EndGame(GameResult.Win);
        }
    }
    
    private void EndGame(GameResult result)
    {
        gameEnded = true;
        currentGameResult = result;

        //Stop the game
        board.currentState = GameState.wait;

        // Short delay before showing end screen
        StartCoroutine(ShowEndScreen(result));
    }

    private IEnumerator ShowEndScreen(GameResult result)
    {
        yield return new WaitForSeconds(1f);

        if(result == GameResult.Win)
        {
            HandleWin();
        }
        else if(result == GameResult.Lose)
        {
            HandleLose();
        }
    }
    private void HandleWin()
    {
        Debug.Log("You win !");

        // calculate reward

        //show win panel
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        // Here you can add:
        // - Save rewards to player data
        // - Unlock next level
        // - Display stars based on score/moves
        // - Animate reward collection
    }
    private void HandleLose()
    {
        Debug.Log("You Lose!");

        // Show lose panel
        if (losePanel != null)
        {
            losePanel.SetActive(true);
        }

        // Here you can add:
        // - Show how close they were to winning
        // - Offer to continue with extra moves (ads/purchase)
        // - Display retry button
    }
    // Calculate win reward

    // Helper method to get star rating
    public int GetStarRating()
    {
        if (scoreManager.score >= board.scoreGoals * 1.5f)
        {
            return 3; // 3 stars
        }
        else if (scoreManager.score >= board.scoreGoals * 1.2f)
        {
            return 2; // 2 stars
        }
        else if (scoreManager.score >= board.scoreGoals)
        {
            return 1; // 1 star
        }
        return 0; // No stars (shouldn't happen in win condition)
    }

    // Public methods for UI buttons
    public void ContinueButton()
    {
        // Reload current scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}
