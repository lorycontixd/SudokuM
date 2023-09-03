using Newtonsoft.Json;
using System;
using UnityEngine;

[Serializable]
public class TimeraceResult
{
    public TimeraceResultUser Winner;
    public TimeraceResultUser Loser;

    public TimeraceResult() { }

    public TimeraceResult(string text)
    {
        TimeraceResult tr = JsonConvert.DeserializeObject<TimeraceResult>(text);
        Debug.Log($"TimeraceRes parse: {tr.Winner.ID}, {tr.Loser.ID}");
        this.Winner = tr.Winner;
        this.Loser = tr.Loser;
    }
}
