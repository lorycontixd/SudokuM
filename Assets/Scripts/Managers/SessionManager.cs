using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    #region Singleton
    private static SessionManager _instance;
    public static SessionManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    #endregion

    public DateTime LocalStartTime { get; private set; }
    public User ActiveUser { get; private set; } = null;
    public User OtherUser { get; private set; } = null;
    public LoginData LoginData { get; private set; } = null;
    public GameInstance GameInstance { get; private set; } = null;

    private void Start()
    {
        LocalStartTime = DateTime.Now;
        DatabaseManager.Instance.onQuery.AddListener(OnDbQuery);
    }

    private void OnDbQuery(DatabaseQueryResultType resultType, QueryData data)
    {
        /*if (data.queryType == QueryType.ADDTIMERACESCORE)
        {
            // Called from 
            if (resultType == DatabaseQueryResultType.SUCCESS)
            {
                AddTimeraceScoreQuery trdata = (AddTimeraceScoreQuery)data;
                SetHostUser(trdata.user);
            }
        }*/
    }

    public void SetHostUser(User user)
    {
        ActiveUser = user;
    }
    public void SetOtherUser(User user)
    {
        OtherUser = user;
    }
    public void SetLoginData(LoginData data)
    {
        LoginData = data;
    }
    public void SetGameInstance(GameInstance game)
    {
        this.GameInstance = game;
    }

    public void OnApplicationQuit()
    {
        if (DatabaseManager.Instance != null)
        {
            if (LoginData != null)
                DatabaseManager.Instance.AddLogoutInstance(LoginData);
        }
    }
}
