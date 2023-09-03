using System;

[Serializable]
public class TimeraceResultUser
{
    public int ID;
    public int ScoreOld;
    public int ScoreChange;
    public int ScoreNew;

    public TimeraceResultUser() { }

    public TimeraceResultUser(int iD, int scoreOld, int scoreChange, int scoreNew)
    {
        ID = iD;
        ScoreOld = scoreOld;
        ScoreChange = scoreChange;
        ScoreNew = scoreNew;
    }
}
