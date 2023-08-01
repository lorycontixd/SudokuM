using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestScoreScene : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField currentScoreInput;
    [SerializeField] private TMP_InputField maxScoreInput;
    [SerializeField] private Button timeraceWinButton;

    [SerializeField] private TextMeshProUGUI finalScoreText;


    private void Start()
    {
        finalScoreText.text = "";
    }

    public void OnTimeraceButton()
    {
        int currentScore = 0;
        Int32.TryParse(currentScoreInput.text, out currentScore);

        int maxScore = 0;
        Int32.TryParse(maxScoreInput.text, out maxScore);

        int finalScore = ScoreManager.CalculateTimeraceWin(currentScore, maxScore);
        Debug.Log($"Calculated new score gain : {finalScore}");
        finalScoreText.text = finalScore.ToString();
    }
}
