using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum QueryType
{
    CHECKLOGIN,
    REGISTER,
    GETUSER,
    ADDTIMERACESCORE,
    GETTIMERACELEADERBOARD,
    ADDLOGININSTANCE,
    CREATEGAMEINSTANCE
}



public abstract class QueryData
{
    public abstract QueryType QueryType { get;  }
    public string extraInfo;
    public string errorMessage;

    public bool IsSuccess { get { return errorMessage == string.Empty; } }
}

/// <summary>
/// Login Data 
/// </summary>
public class LoginQueryData : QueryData
{
    public int userid;
    public override QueryType QueryType { get => QueryType.CHECKLOGIN; }

    public LoginQueryData(int userid, string extrainfo = "")
    {
        this.userid = userid;
        this.extraInfo = extrainfo;
        this.errorMessage = string.Empty;
    }
    public LoginQueryData(string errorMessage)
    {
        this.userid = -1;
        this.extraInfo = "error";
        this.errorMessage = errorMessage;
    }
}

public class RegisterQueryData : QueryData
{
    public User user;
    public override QueryType QueryType => QueryType.REGISTER;

    public RegisterQueryData(User user, string extrainfo = "")
    {
        this.user = user;
        this.extraInfo = extrainfo;
        this.errorMessage = string.Empty;
    }
    public RegisterQueryData(string errorMessage)
    {
        this.user = null;
        this.errorMessage = errorMessage;
    }
}

public class GetUserQuery : QueryData
{
    public User user;
    public override QueryType QueryType => QueryType.GETUSER; 

    public GetUserQuery(User user, string extrainfo = "")
    {
        this.extraInfo = extrainfo;
        this.user = user;
    }
    public GetUserQuery(string errorMessage)
    {
        user = null;
        this.extraInfo = "";
        this.errorMessage = errorMessage;
    }
}

public class AddTimeraceScoreQuery : QueryData
{
    #region TimereaceScoreResult
    [Serializable]
    public class TimeraceScoreResult
    {
        public Dictionary<string, int> winner;
        public Dictionary<string, int> loser;
    }
    #endregion

    public TimeraceScoreResult result;
    public override QueryType QueryType => QueryType.ADDTIMERACESCORE;

    public AddTimeraceScoreQuery(string json, string extrainfo = "")
    {
        result = JsonConvert.DeserializeObject<TimeraceScoreResult>(json);

    }
    public AddTimeraceScoreQuery(string errorMessage)
    {
        this.errorMessage = errorMessage;
    }
}

public class GetTimeraceLeaderboardQuery : QueryData
{
    public TimeraceLeaderboard leaderboard;
    public override QueryType QueryType => QueryType.GETTIMERACELEADERBOARD;

    public GetTimeraceLeaderboardQuery(TimeraceLeaderboard l, string extrainfo = "")
    {
        this.extraInfo = extrainfo;
        leaderboard = l;
    }
    public GetTimeraceLeaderboardQuery(string errormsg)
    {
        this.extraInfo = "";
        this.leaderboard = null;
        this.errorMessage = errormsg;
    }
}

public class AddLoginInstanceQuery : QueryData
{
    public LoginData loginData;
    public override QueryType QueryType => QueryType.ADDLOGININSTANCE;

    public AddLoginInstanceQuery(int id, int userid, DateTime login, string dm, string dn, string dt, string logindata, string extrainfo = "")
    {
        loginData = new LoginData(id, userid, login, dm, dn, dt, logindata);
        this.extraInfo = extrainfo;
    }
    public AddLoginInstanceQuery(LoginData loginData, string extrainfo = "")
    {
        this.loginData = loginData;
        this.extraInfo = extrainfo;
    }

    public AddLoginInstanceQuery(string errormsg)
    {
        this.extraInfo = "";
        this.loginData = null;
        this.errorMessage = errormsg;
    }
}

public class CreateGameInstanceQuery : QueryData
{
    public GameInstance gameInstance;
    public override QueryType QueryType => QueryType.CREATEGAMEINSTANCE;

    public CreateGameInstanceQuery(GameInstance gameInstance, string extrainfo = "")
    {
        this.gameInstance = gameInstance;
        this.extraInfo = extrainfo;
    }
    public CreateGameInstanceQuery(string errormsg)
    {
        this.gameInstance = null;
        this.errorMessage = errormsg;
        this.extraInfo = "";
    }
}