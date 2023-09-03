using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class User
{
    #region User classes
    public class Currency
    {
        public int Level;
        public int Xp;

        public Currency()
        {

        }
        public Currency(int level, int xp)
        {
            Level = level;
            Xp = xp;
        }
        public Currency(string text)
        {
            string[] values = text.Split('\t');
            this.Level = int.Parse(values[0]);
            this.Xp = int.Parse(values[1]);
        }
    }

    [Serializable]
    public class Score
    {
        public int ScoreCoop;
        public int ScoreTimerace;

        public Score()
        {

        }
        public Score(string text)
        {
            string[] values = text.Split('\t');
            this.ScoreCoop = int.Parse(values[0]);
            this.ScoreTimerace = int.Parse(values[1]);
        }

        public Score(int CoopScore, int TimeraceScore)
        {
            this.ScoreCoop= CoopScore;
            this.ScoreTimerace= TimeraceScore;
        }
    }
    #endregion

    public int ID;
    public int RoleID;
    public string Username;
    public string GoogleID;
    public string FirstName;
    public string LastName;
    public string Email;
    public string Password;
    public DateTime CreationDate;
    public DateTime LastUpdated;
    public DateTime LastLogin;
    public bool IsOnline;

    public Currency Currencies;
    public Score Scores;

    public const int UserBaseLength = 12;
    public const int UserFullLength = 12;

    public User()
    {

    }

    public User(int iD, int roleID, string username, string googleID, string firstName, string lastName, string email, string password, DateTime creationDate, DateTime lastUpdated, DateTime lastLogin, bool isOnline)
    {
        ID = iD;
        RoleID = roleID;
        Username = username;
        GoogleID = googleID;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Password = password;
        CreationDate = creationDate;
        LastUpdated = lastUpdated;
        LastLogin = lastLogin;
        IsOnline = isOnline;
        Currencies = null;
        Scores = null;
    }

    public User(int iD, int roleID, string username, string googleID, string firstName, string lastName, string email, string password, DateTime creationDate, DateTime lastUpdated, DateTime lastLogin, bool isOnline, Currency userCurrency, Score userScore) : this(iD, roleID, username, googleID, firstName, lastName, email, password, creationDate, lastUpdated, lastLogin, isOnline)
    {
        Currencies = userCurrency;
        Scores = userScore;
    }

    public User(int iD, int roleID, string username, string googleID, string firstName, string lastName, string email, string password, DateTime creationDate, DateTime lastUpdated, DateTime lastLogin, bool isOnline, int level, int xp, int score_tr, int score_coop) : this(iD, roleID, username, googleID, firstName, lastName, email, password, creationDate, lastUpdated, lastLogin, isOnline)
    {
        Currencies = new Currency(level, xp);
        Scores = new Score(score_coop, score_tr) ;
    }

    public User(string txt, string mode = "j")
    {
        if (mode == "t")
        {
            string[] values = txt.Split('\t');
            int count = values.Length;

            ID = int.Parse(values[0]);
            RoleID = int.Parse(values[1]);
            Username = values[2];
            GoogleID = values[3];
            FirstName = values[4];
            LastName = values[5];
            Email = values[6];
            Password = values[7];
            CreationDate = DateTime.Parse(values[8]);
            LastUpdated = DateTime.Parse(values[9]);
            LastLogin = DateTime.Parse(values[10]);
            IsOnline = Convert.ToBoolean(int.Parse(values[11]));
            if (count == UserFullLength)
            {
                Currencies = new Currency(int.Parse(values[12]), int.Parse(values[13]));
                Scores = new Score(int.Parse(values[14]), int.Parse(values[15]));
            }
        }else if (mode == "j")
        {
            User u = FromJson(txt);
            ID = u.ID;
            RoleID = u.RoleID;
            Username = u.Username;
            GoogleID = u.GoogleID;
            FirstName = u.FirstName;
            LastName = u.LastName;
            Email = u.Email;
            Password = u.Password;
            CreationDate = u.CreationDate;
            LastUpdated = u.LastUpdated;
            LastLogin = u.LastLogin;
            IsOnline = u.IsOnline;
            Currencies = u.Currencies;
            Scores = u.Scores;
        }
        
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
    public static User FromJson(string json)
    {
        User user = JsonConvert.DeserializeObject<User>(json);
        return user;
    }
}
