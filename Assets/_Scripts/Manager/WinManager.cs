using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WinManager : MonoBehaviour
{
    private EndGameManager endGameManager;
    private ScoreManager scoreManager;

    public TextMeshProUGUI finalScoreText;
    public void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        endGameManager = FindObjectOfType<EndGameManager>();
        scoreManager = FindObjectOfType<ScoreManager>();

        //int finalScore = scoreManager.GetFinalScore();

        //finalScoreText.text = finalScore.ToString();

    }
}
