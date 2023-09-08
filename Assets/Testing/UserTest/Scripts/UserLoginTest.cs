using JetBrains.Annotations;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserLoginTest : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private TextMeshProUGUI debugText;


    private void Start()
    {
        DatabaseManager.Instance.onQuery.AddListener(OnDbQuery);
    }

    private void OnDbQuery(DatabaseQueryResultType type, QueryData data)
    {
        switch (data.QueryType)
        {
            case (QueryType.CHECKLOGIN):
                LoginQueryData loginData = (LoginQueryData)data;
                debugText.text = $"Logindata ===> type: {type}, user: {loginData.userid}";
                break;
        }
    }

    public void ButtonLogin()
    {
        DatabaseManager.Instance.ValidateLogin(usernameInput.text, passwordInput.text);
    }
    public void WriteDebug(string text)
    {
        debugText.text = text;
    }
}
