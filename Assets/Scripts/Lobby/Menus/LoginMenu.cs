using ExitGames.Client.Photon;
using Managers;
using Michsky.MUIP;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LoginMenu : BaseMenu
{
    [Range(10f, 200f)] public float ConnectingImageRotationSpeed = 100;
    public int GoogleLoginMaxAttempts = 3;
    public string AutofillUsernameEditor;
    public string AutofillPasswordEditor;
    public string AutofillUsernameApp;
    public string AutofillPasswordApp;


    [Header("Components")]
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private ButtonManager loginButton;
    [SerializeField] private ButtonManager registerButton;
    [SerializeField] private ButtonManager backButton;
    [SerializeField] private ButtonManager settingsButton;
    [SerializeField] private Button googleLoginButton;
    [SerializeField] private SwitchManager rememberMeSwitch;
    [SerializeField] private Button passwordVisibilityButton;
    //[SerializeField] private TextMeshProUGUI connectingText;
    [SerializeField] private Image connectingImage;


    public override MenuType Type => MenuType.LOGIN;
    private int googleLoginAttempts = 0;
    private bool isConnectedPing;
    private bool isConnecting = false;
    private float connectingDuration = 0f;
    private int loggingInUserID = -1;
    private bool IsPasswordVisible = false;

    private void Start()
    {
        googleLoginAttempts = 0;
        GooglePlayManager.Instance.onAuthenticationCompleted += OnGooglePlayAuth;
        DatabaseManager.Instance.onQuery.AddListener(OnDatabaseQuery);

        if (DebugManager.Instance.DebugGame && DebugManager.Instance.DebugAutofill)
        {
            if (AutofillUsernameEditor != string.Empty)
            {
                if (Application.isEditor)
                    usernameInput.text = AutofillUsernameEditor;
                else
                    usernameInput.text = AutofillUsernameApp;
            }
            if (AutofillPasswordEditor != string.Empty)
            {
                if (Application.isEditor)
                    passwordInput.text = AutofillPasswordEditor;
                else
                    passwordInput.text = AutofillPasswordApp;
            }
        }
        //StartCoroutine(StartPing());
        isConnectedPing = true;

        
    }
    private void Update()
    {
        if (isConnecting)
        {
            connectingDuration += Time.deltaTime;
            if (connectingDuration >= NetworkManager.Instance.TimeoutLogin)
            {
                Managers.NotificationManager.Instance.Error("Connection timeout", "Unable to connect to the server. Please try again.");
                connectingDuration = 0f;
                SetConnectingState(false);
                connectingImage.rectTransform.rotation = Quaternion.identity;
                SessionManager.Instance.SetHostUser(null);
                isConnecting = false;
            }
            if (connectingImage != null)
            {
                connectingImage.rectTransform.Rotate(new Vector3(0f, 0f, ConnectingImageRotationSpeed * Time.deltaTime)) ;
            }
        }
    }

    private void OnDatabaseQuery(DatabaseQueryResultType resultType, QueryData data)
    {
        if (data.QueryType == QueryType.CHECKLOGIN)
        {
            if (resultType == DatabaseQueryResultType.SUCCESS)
            {
                LoginQueryData loginData = (LoginQueryData)data;
                loggingInUserID = loginData.userid;
                DatabaseManager.Instance.GetUserFull(loggingInUserID, "login_request");
                Debug.Log($"[LoginMenu->OnQuery] Validated login for user {loggingInUserID}");
            }
            else
            {
                Debug.Log($"[LoginMenu->OnQuery] Bad login request");
                Managers.NotificationManager.Instance.Error("Login Failed", "Could not connect to the server. Try again.");
                SetConnectingState(false);
            }
        }
        else if (data.QueryType == QueryType.GETUSER)
        {
            if (data.extraInfo == "login_request")
            {
                if (resultType == DatabaseQueryResultType.SUCCESS)
                {
                    GetUserQuery udata = (GetUserQuery)data;
                    User user = udata.user;
                    Debug.Log($"[LoginMenu->OnQuery] Successfully fetched current user -> Connecting to photon");
                    SessionManager.Instance.SetHostUser(user);
                    NetworkManager.Instance.ConnectToEU();
                }
                else
                {

                }
            }
        }
        else if (data.QueryType == QueryType.ADDLOGININSTANCE)
        {
            if (data.extraInfo == "login_request")
            {
                if (resultType == DatabaseQueryResultType.SUCCESS)
                {
                    AddLoginInstanceQuery addLoginInstanceQuery = (AddLoginInstanceQuery)data;
                    LoginData loginData = addLoginInstanceQuery.loginData;
                    SessionManager.Instance.SetLoginData(loginData);
                    controller.SwitchMenu(MenuType.MATCHMAKING);
                }
                else
                {
                    Managers.NotificationManager.Instance.Error("Login Failed", "Could not connect to the server. Try again.");
                    SetConnectingState(false);
                    return;
                }
            }
            
        }
    }

    private void OnGooglePlayAuth(bool success)
    {
        if (success)
        {
        }
        else
        {
            googleLoginAttempts++;
            ToggleUI(true);
        }
    }

    public override void Close()
    {
    }

    public override void Open()
    {
        connectingDuration = 0f;
        passwordInput.text = string.Empty;
        SetConnectingState(false);

        if (ES3.KeyExists("username"))
        {
            string u = ES3.Load<string>("username");
            usernameInput.text = u;
            rememberMeSwitch.SetOn();
            rememberMeSwitch.UpdateUI();
        }
        else
        {
            usernameInput.text = string.Empty;
        }

        IsPasswordVisible = false;
        passwordInput.contentType = TMP_InputField.ContentType.Password;

    }

    public void UpdateUI()
    {
    }

    private IEnumerator StartPing()
    {
        while (true)
        {
            var ping = new Ping("8.8.8.8");

            yield return new WaitForSeconds(1f);
            while (!ping.isDone)
            {
                isConnectedPing = false;
                yield return null;
            }
            isConnectedPing = true;
            break;
        }
    }

    #region Buttons
    public void ButtonLogin()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Managers.NotificationManager.Instance.Error("No internet connection", "Make sure you have an internet connection to play Multiplayer");
            return;
        }
        if (!isConnectedPing)
        {
            Managers.NotificationManager.Instance.Error("Network error", "You must first connect to the internet");
        }
        if (usernameInput.text == string.Empty ||  passwordInput.text == string.Empty)
        {
            Managers.NotificationManager.Instance.Error("Invalid inputs", "You must enter a valid username and password");
            return;
        }
        ToggleUI(false);
        if (SessionManager.Instance.ActiveUser == null || !PhotonNetwork.IsConnected)
        {
            if (ValidateInputs())
            {
                DatabaseManager.Instance.ValidateLogin(usernameInput.text, passwordInput.text);
                SetConnectingState(true);
                isConnecting = true;
            }
        }
        else
        {
            controller.SwitchMenu(MenuType.MATCHMAKING);
        }
    }
    public void ButtonRegister()
    {
        if (controller != null)
        {
            controller.SwitchMenu(MenuType.REGISTER);
        }
    }
    public void ButtonGoogleLogin()
    {
        if (true) // !Application.isEditor
        {
            if (GooglePlayManager.Instance != null)
            {
                if (!GooglePlayManager.Instance.IsAuthenticated)
                {
                    if (googleLoginAttempts < GoogleLoginMaxAttempts)
                    {
                        GooglePlayManager.Instance.Authenticate();
                        ToggleUI(false);
                    }
                }
            }
        }
        else
        {
        }
        
    }
    public void ButtonTogglePasswordVisibility()
    {
        Debug.Log($"pass visibility");
        TogglePasswordVisibility();
    }
    public void ButtonBack()
    {
        controller.SwitchMenu(MenuType.MAIN);
    }
    #endregion

    #region Pun Callbacks
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        if (SessionManager.Instance.ActiveUser != null)
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                PhotonNetwork.NickName = SessionManager.Instance.ActiveUser.Username;
                SetConnectingState(false);

                // Set properties
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable());
                Hashtable table = PhotonNetwork.LocalPlayer.CustomProperties;
                if (!table.ContainsKey("uid"))
                {
                    table.Add("uid", SessionManager.Instance.ActiveUser.ID);
                }
                PhotonNetwork.LocalPlayer.SetCustomProperties(table);

                Debug.Log($"Set my custom props uid ==> {PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("uid")}");

                // Save username if remember me
                if (rememberMeSwitch.isOn)
                {
                    ES3.Save("username", usernameInput.text);
                }
                else
                {
                    if (ES3.KeyExists("username"))
                    {
                        ES3.DeleteKey("username");
                    }
                }

                // Register login
                DatabaseManager.Instance.AddLoginInstance(
                    SessionManager.Instance.ActiveUser,
                    SystemInfo.deviceModel,
                    SystemInfo.deviceName,
                    SystemInfo.deviceType.ToString(),
                    "login_request"
                );
                
                // Switch menu in database query received
            }
        }
    }
    #endregion


    private bool ValidateInputs()
    {
        return usernameInput.text != string.Empty && passwordInput.text != string.Empty;
    }
    private void ToggleUI(bool activate)
    {
        usernameInput.interactable = activate;
        passwordInput.interactable = activate;
        loginButton.Interactable(activate);
        registerButton.Interactable(activate);
        settingsButton.Interactable(activate);
        googleLoginButton.interactable = activate;
        backButton.Interactable(activate);
        passwordVisibilityButton.interactable = activate;
    }

    public void SetConnectingState(bool connecting)
    {
        isConnecting = connecting;
        connectingImage.gameObject.SetActive(isConnecting);
        ToggleUI(!connecting);
    }

    private void TogglePasswordVisibility()
    {
        if (IsPasswordVisible)
        {
            IsPasswordVisible = false;
            passwordInput.contentType = TMP_InputField.ContentType.Password;
            passwordInput.ForceLabelUpdate();
        }
        else
        {
            IsPasswordVisible = true;
            passwordInput.contentType = TMP_InputField.ContentType.Standard;
            passwordInput.ForceLabelUpdate();
        }
    }
}
