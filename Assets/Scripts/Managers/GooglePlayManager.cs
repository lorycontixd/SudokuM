using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using System.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GooglePlayManager : MonoBehaviour
{
    #region Singleton
    private static GooglePlayManager _instance;
    public static GooglePlayManager Instance { get { return _instance; } }

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

    public bool IsActive = true;
    public bool AuthenticateOnStart = false;
    public bool IsAuthenticated { get => PlayGamesPlatform.Instance.IsAuthenticated(); }

    public static Dictionary<string, string> GoogleAchievementKeys;
    public static Dictionary<string, string> GoogleLeaderboardKeys;

    public Action<bool> onAuthenticationCompleted; // Params = (success)

    private bool reconnectAttempted = false;


    private void Start()
    {
        if (AuthenticateOnStart)
        {
            Debug.Log($"[GP] Activating debug log");
            PlayGamesPlatform.DebugLogEnabled = false;
            Debug.Log($"[GP] Initializing!!");
            Authenticate();
        }
    }

    internal void Authenticate()
    {
        if (!IsActive)
        {
            return;
        }
        if (!DataManager.Instance.ReadGoogleData)
        {
            Debug.LogError("Cannot try to authenticate to google without internet. Google data not downloaded");
            return;
        }
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
        Social.localUser.Authenticate((bool success) => { Debug.Log($"Social auth success: {success}"); });
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            Debug.Log($"=============>>> PlayGames: Sign in successful");
            onAuthenticationCompleted?.Invoke(true);
        }
        else
        {
            Debug.Log($"=============>>> PlayGames: Sign in FAIL ==> {status} ==> {PlayGamesPlatform.Instance.IsAuthenticated()}");
            onAuthenticationCompleted?.Invoke(false);
        }

    }

    private void Login()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }
    private IEnumerator LoginDelay(float delay = 2f)
    {
        yield return new WaitForSeconds(delay);
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }


}
