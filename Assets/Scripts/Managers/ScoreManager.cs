using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScoreManager
{

    public static int CalculateTimeraceWin(int currentScore, int maxScore)
    {
        if (maxScore <= 0) { maxScore = 10; }
        float x = (float)currentScore;
        int flatValue = 15;
        float a = 0.1f * maxScore;
        float b = 5;
        float bonusValue = a * Mathf.Exp(-(b * x) / maxScore);
        return flatValue + Mathf.RoundToInt(bonusValue);
    }

    public static int CalculateTimeraceLoss(int currentScore, int maxScore)
    {
        if (maxScore <= 0) { maxScore = 10; }
        return 0;
    }
}