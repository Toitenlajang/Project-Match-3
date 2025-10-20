using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private Board board;
    public TextMeshProUGUI scoreText;
    public int score;
    public Image scoreBar;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        board = FindObjectOfType<Board>();
        scoreText.text = score.ToString();
    }
    public void IncreaseScore(int amountToIncrease)
    {
        score += amountToIncrease;
        if(board != null && scoreBar != null)
        {
            scoreBar.fillAmount = (float)score / (float)board.scoreGoals;
        }
    }
}
