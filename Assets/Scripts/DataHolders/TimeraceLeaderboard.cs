using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TimeraceLeaderboard
{
    #region Record Class
    [Serializable]
    public class TimeraceLeaderboardRecord
    {
        public int userid;
        public int score;
        public int rank;

        public TimeraceLeaderboardRecord(int userid, int score, int rank)
        {
            this.userid = userid;
            this.score = score;
            this.rank = rank;
        }

        public TimeraceLeaderboardRecord(string text)
        {
            string[] values = text.Split('\t');
            this.userid = int.Parse(values[0]);
            this.score = int.Parse(values[1]);
            this.rank = int.Parse(values[2]);
        }
    }
    #endregion

    public List<TimeraceLeaderboardRecord> records = new List<TimeraceLeaderboardRecord>();

    public TimeraceLeaderboard(string text)
    {
        string[] lines = text.Split(new[] { "<br>" }, StringSplitOptions.None);
        records = new List<TimeraceLeaderboardRecord>();
        foreach(string line in lines)
        {
            if (line != string.Empty)
            {
                TimeraceLeaderboardRecord record = new TimeraceLeaderboardRecord(line);
                records.Add(record);
            }
        }
    }
}
