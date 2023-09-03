using Managers;
using Michsky.MUIP;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RegisterMenu : BaseMenu
{
    public int UsernameMinLength = 4;

    [Header("Debug")]
    public bool DebugAllowAllEmails = true;
    public string DebugUsername;
    public string DebugPassword;
    public string DebugEmail;

    [Header("Components")]
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_InputField passwordConfirmInput;
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField firstnameInput;
    [SerializeField] private TMP_InputField lastnameInput;
    [SerializeField] private ButtonManager registerButton;
    [SerializeField] private ButtonManager backButton;

    public override MenuType Type => MenuType.REGISTER;


    private void Start()
    {
        DatabaseManager.Instance.onQuery.AddListener(OnDatabaseQuery);

    }

    private void OnDatabaseQuery(DatabaseQueryResultType resultType, QueryData data)
    {
        if (data.QueryType == QueryType.REGISTER)
        {
            if (resultType == DatabaseQueryResultType.SUCCESS)
            {
                RegisterQueryData regData = (RegisterQueryData)data;
                SessionManager.Instance.SetHostUser(regData.user);
                NetworkManager.Instance.ConnectToEU();
            }
            else
            {
                ToggleUI(true);
            }
        }else if (data.QueryType == QueryType.ADDLOGININSTANCE)
        {
            Debug.Log($"[RegMen] AddLoginINst. ==> {data.QueryType}, {data.extraInfo}, {resultType}");
            if (data.extraInfo == "register_request")
            {
                if (resultType == DatabaseQueryResultType.SUCCESS)
                {
                    AddLoginInstanceQuery addLoginInstanceQuery = (AddLoginInstanceQuery)data;
                    LoginData loginData = addLoginInstanceQuery.loginData;
                    SessionManager.Instance.SetLoginData(loginData);
                    controller.SwitchMenu(MenuType.MATCHMAKING);
                }
            }
        }
    }

    public override void Close()
    {
    }

    public override void Open()
    {
        ResetUI();
        ToggleUI(true);
        if (DebugManager.Instance.DebugAutofill)
        {
            if (DebugUsername != string.Empty)
            {
                usernameInput.text = DebugUsername;
            }
            if (DebugPassword != string.Empty)
            {
                passwordInput.text = DebugPassword;
                passwordConfirmInput.text = DebugPassword;
            }
            if (DebugEmail != string.Empty)
            {
                emailInput.text = DebugEmail;
            }
        }
    }

    public void UpdateUI()
    {
    }
    public void ResetUI()
    {
        usernameInput.text = string.Empty;
        passwordInput.text = string.Empty;
        passwordConfirmInput.text = string.Empty;
        emailInput.text = string.Empty;
    }


    #region Buttons
    public void ButtonRegister()
    {
        ToggleUI(false);
        if (ValidateInputs())
        {
            DatabaseManager.Instance.RegisterUser(
                usernameInput.text,
                firstnameInput.text,
                lastnameInput.text,
                emailInput.text,
                passwordInput.text
            );
        }
    }
    public void ButtonBack()
    {
        controller.SwitchMenu(MenuType.LOGIN);
    }
    #endregion

    #region Pun Callbacks
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable());
        Hashtable table = PhotonNetwork.LocalPlayer.CustomProperties;
        if (!table.ContainsKey("uid"))
        {
            table.Add("uid", SessionManager.Instance.ActiveUser.ID);
        }
        PhotonNetwork.LocalPlayer.SetCustomProperties(table);

        // Register login
        Debug.Log($"Adding login instance from register menu");
        DatabaseManager.Instance.AddLoginInstance(
            SessionManager.Instance.ActiveUser,
            SystemInfo.deviceModel,
            SystemInfo.deviceName,
            SystemInfo.deviceType.ToString(),
            "register_request"
        );
    }
    #endregion

    private bool ValidateInputs()
    {
        bool usernameValid = usernameInput.text != string.Empty && usernameInput.text.Length > UsernameMinLength; ;
        bool passwordValid = passwordInput.text != string.Empty && passwordInput.text.Length > 6;
        bool passwordsMatch = passwordInput.text == passwordConfirmInput.text;
        bool validEmail = emailInput.text != string.Empty && (DebugAllowAllEmails || emailInput.text.Contains('@'));

        if (!usernameValid)
        {
            Managers.NotificationManager.Instance.Error("Invalid username", $"Username must have at least {UsernameMinLength} characters");
            ToggleUI(true);
            return false;
        }
        if (!passwordValid)
        {
            Managers.NotificationManager.Instance.Error("Invalid password", $"Password must have at least {6} characters ");
            ToggleUI(true);
            return false;
        }
        if (!passwordsMatch)
        {
            Managers.NotificationManager.Instance.Error("Invalid passwords", "Passwords do not match!");
            ToggleUI(true);
            return false;
        }
        if (!validEmail)
        {
            Managers.NotificationManager.Instance.Error("Invalid email", "Invalid email");
            ToggleUI(true);
            return false;
        }
        return true;
    }

    private void ToggleUI(bool active)
    {
        registerButton.Interactable(active);
        backButton.Interactable(active);
    }
}
