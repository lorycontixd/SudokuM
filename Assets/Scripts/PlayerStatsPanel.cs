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
        if (StatsManager.Instance == null)
        {
            Debug.LogWarning($"No Stats Manager found!");
            return;
        }
        float perc0 = (float)StatsManager.Instance.totalMoves[userid] / StatsManager.Instance.overallTotalMoves;
        float perc = StatsManager.Instance.overallTotalMoves != 0 ? (perc0 * 100f) : 0f;
        bar.ChangeValue(perc);
        Debug.Log($"Correct Moves: {StatsManager.Instance.correctMoves[userid]} / Total Moves: {StatsManager.Instance.overallTotalMoves},    Player stats slider: {perc}");
        bar.UpdateUI();
        movesText.text = $"{StatsManager.Instance.correctMoves[userid]} / {StatsManager.Instance.overallTotalMoves}";

        correctMovesText.text = $"Correct moves: {StatsManager.Instance.correctMoves[userid]} / {StatsManager.Instance.totalMoves[userid]}";
        wrongMovesText.text = $"Wrong moves: {StatsManager.Instance.wrongMoves[userid]} / {StatsManager.Instance.totalMoves[userid]}";
        avgMoveTimeText.text = $"Avg. time per move: {StatsManager.Instance.averageTimePerMove[userid].ToString("0.00")}";
    }

}
