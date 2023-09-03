using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardPanelItem : MonoBehaviour
{
    public TimeraceLeaderboard.TimeraceLeaderboardRecord record { get; private set; }
    [Header("UI")]
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI scoreText;
    

    public void SetRecord(TimeraceLeaderboard.TimeraceLeaderboardRecord record)
    {
        this.record = record;
    }

    public void UpdateUI()
    {
        if (record != null)
        {
            //this.usernameText.text = record.user.Username;
            this.rankText.text = record.rank.ToString();
            this.levelText.text = "LV 1";
            //this.scoreText.text = record.user.TimeraceScore.ToString();
        }
    }
}
