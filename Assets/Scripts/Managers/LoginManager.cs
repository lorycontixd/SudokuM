using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    #region Singleton
    private static LoginManager _instance;
    public static LoginManager Instance { get { return _instance; } }

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


    private void Start()
    {
        GooglePlayManager.Instance.onAuthenticationCompleted += OnGooglePlayAuth;
    }

    private void OnGooglePlayAuth(bool success)
    {
    }

    public void GoogleLogin()
    {
        if (GooglePlayManager.Instance != null)
        {
            if (!GooglePlayManager.Instance.IsAuthenticated)
            {
                GooglePlayManager.Instance.Authenticate();
            }
        }
    }
}
