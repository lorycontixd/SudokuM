using Michsky.MUIP;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class PlayerStatsPanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private TextMeshProUGUI sliderTitleText;
    [SerializeField] private ProgressBar bar;
    [SerializeField] private TextMeshProUGUI movesText;

    [SerializeField] private TextMeshProUGUI correctMovesText;
    [SerializeField] private TextMeshProUGUI wrongMovesText;
    [SerializeField] private TextMeshProUGUI avgMoveTimeText;

    private int userid;
    private string username;

    private void Start()
    {
    }

    public void SetIdentity(Player player)
    {
        userid = player.ActorNumber;
        username = player.NickName;
        UpdatePlayerIdentity();
    }
    public void UpdatePlayerIdentity()
    {
        usernameText.text = username;
    }

    public void UpdateStats()
    {
        if (StatsManagerCoop.Instance == null)
        {
            Debug.LogWarning($"No StatsManager found!");
            return;
        }
        if (GameManager.Instance == null)
        {
            Debug.LogWarning($"No GameManager found!");
        }
        if (GameManager.Instance.GameMode == GameMode.COOP)
        {
            sliderTitleText.text = "Player contribution";
            float perc0 = (float)StatsManagerCoop.Instance.totalMoves[userid] / StatsManagerCoop.Instance.overallTotalMoves;
            float perc = StatsManagerCoop.Instance.overallTotalMoves != 0 ? (perc0 * 100f) : 0f;
            bar.ChangeValue(perc);
            bar.UpdateUI();
            movesText.text = $"{StatsManagerCoop.Instance.correctMoves[userid]} / {StatsManagerCoop.Instance.overallTotalMoves}";

            correctMovesText.text = $"Correct moves: {StatsManagerCoop.Instance.correctMoves[userid]} / {StatsManagerCoop.Instance.totalMoves[userid]}";
            wrongMovesText.text = $"Wrong moves: {StatsManagerCoop.Instance.wrongMoves[userid]} / {StatsManagerCoop.Instance.totalMoves[userid]}";
            avgMoveTimeText.text = $"Avg. time per move: {StatsManagerCoop.Instance.averageTimePerMove[userid].ToString("0.00")}";
        }
        else if (GameManager.Instance.GameMode == GameMode.TIMERACE)
        {
            sliderTitleText.text = "Board completed";
            float perc0 = (float)StatsManagerCoop.Instance.puzzleCompleted[userid] / StatsManagerCoop.TOTAL_CELLS;
            float perc = perc0 * 100f;
            //float perc = StatsManagerCoop.Instance.overallTotalMoves != 0 ? (perc0 * 100f) : 0f;
            bar.ChangeValue(perc);
            bar.UpdateUI();
            movesText.text = $"Cells completed: {StatsManagerCoop.Instance.puzzleCompleted[userid]} / {StatsManagerCoop.TOTAL_CELLS}";

            correctMovesText.text = $"Correct moves: {StatsManagerCoop.Instance.correctMoves[userid]} / {StatsManagerCoop.Instance.totalMoves[userid]}";
            wrongMovesText.text = $"Wrong moves: {StatsManagerCoop.Instance.wrongMoves[userid]} / {StatsManagerCoop.Instance.totalMoves[userid]}";
            avgMoveTimeText.text = $"Avg. time per move: {StatsManagerCoop.Instance.averageTimePerMove[userid].ToString("0.00")}";
        }
        
    }

}
