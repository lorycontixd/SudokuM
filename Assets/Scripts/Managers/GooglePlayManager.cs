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

    private bool reconnectAttempted = false;
    public static PlayGamesPlatform platform;

    private void Start()
    {
        //Debug.Log($"[GP] Initializing!!");
        //PlayGamesPlatform.Activate();
        //PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }
    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            Debug.Log($"=============>>> PlayGames: Sign in successful");
        }
        else
        {
            Debug.Log($"=============>>> PlayGames: Sign in FAIL ==> {status} ==> {PlayGamesPlatform.Instance.IsAuthenticated()}");
            // Disable your integration with Play Games Services or show a login button
            // to ask users to sign-in. Clicking it should call
            if (!reconnectAttempted)
            {
                Debug.Log($"=====> Authenticating manually!");
                PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);
                reconnectAttempted = true;
            }
            else
            {
                Debug.Log($"=====> Failed again :(");
                return;
            }
            //PlayGamesPlatform.Instance.RequestServerSideAccess(true, (string s) => { Debug.Log($"====> Req. serverside access: {s}"); });
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
