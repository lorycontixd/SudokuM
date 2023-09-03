using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SocialPlatforms.Impl;

namespace Managers
{
    public class DatabaseManager : MonoBehaviour
    {
        #region Singleton
        private static DatabaseManager _instance;
        public static DatabaseManager Instance { get { return _instance; } }

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

        public string AppAddress;
        public string DatabaseName;
        public string DatabaseServer;
        public string ConnectionString;

        public UnityEvent<DatabaseQueryResultType, QueryData> onQuery;



        private string GetUserPath(string file)
        {
            if (!file.EndsWith(".php"))
            {
                file = file + ".php";
            }
            return $"{AppAddress}/PHP/user/{file}";
        }
        private string GetScorePath(string file)
        {
            if (!file.EndsWith(".php"))
            {
                file = file + ".php";
            }
            return $"{AppAddress}/PHP/scores/{file}";
        }
        private string GetGamePath(string file)
        {
            if (!file.EndsWith(".php"))
            {
                file = file + ".php";
            }
            return $"{AppAddress}/PHP/game/{file}";
        }


        #region User Functions
        public void ValidateLogin(string username, string password, string extrainfo = "")
        {
            StartCoroutine(ValidateLoginCo(username, password, extrainfo));
        }
        private IEnumerator ValidateLoginCo(string username, string password, string extrainfo = "")
        {
            WWWForm form = new WWWForm();
            form.AddField("username", username);
            form.AddField("password", password);
            string file = GetUserPath("validatelogin");
            Debug.Log($"[DatabaseManager] Validating with file {file}");
            UnityWebRequest www = UnityWebRequest.Post(file, form);
            yield return www.SendWebRequest();
            Debug.Log("Login text: " + www.downloadHandler.text, this);
            //int randomNumber = Random.Range(0, 9999);
            //onQuery?.Invoke(ResultType.SUCCESS, new MyLoginData(new User(0, $"Player{randomNumber}", "Temporary", "Account", "email", "hashedpass")));

            if (www.result != UnityWebRequest.Result.Success)
            {
                onQuery?.Invoke(DatabaseQueryResultType.FAIL_CONNECTION, new LoginQueryData("Unexpected error while connecting to server. Contact administration."));
            }
            else
            {
                string text = www.downloadHandler.text;
                Debug.Log("Checklogin text: " + text);
                if (!text.Contains("Error"))
                {
                    int userid = int.Parse(text);
                    onQuery?.Invoke(DatabaseQueryResultType.SUCCESS, new LoginQueryData(userid, extrainfo));
                }
                else
                {
                    onQuery?.Invoke(DatabaseQueryResultType.FAIL_QUERY, new LoginQueryData(text));
                }
            }
        }

        public void RegisterUser(string username, string firstname, string lastname, string email, string password, string extrainfo = "")
        {
            StartCoroutine(RegisterUserCo(username, firstname, lastname, email, password, extrainfo));
        }
        private IEnumerator RegisterUserCo(string username, string firstname, string lastname, string email, string password, string extrainfo = "")
        {
            WWWForm form = new WWWForm();
            form.AddField("username", username);
            form.AddField("firstname", firstname);
            form.AddField("lastname", lastname);
            form.AddField("email", email);
            form.AddField("password", password);

            Debug.Log("Sending REGISTER request with form");
            UnityWebRequest www = UnityWebRequest.Post(GetUserPath("registeruser"), form);
            yield return www.SendWebRequest();
            Debug.Log($"Register text: {www.downloadHandler.text}");

            if (www.result != UnityWebRequest.Result.Success)
            {
                onQuery?.Invoke(DatabaseQueryResultType.FAIL_CONNECTION, new RegisterQueryData(www.error));
            }
            else
            {
                string text = www.downloadHandler.text;
                if (text.Contains("Error"))
                {
                    Debug.Log("TEXT: " + text);
                    if (text == "Error0")
                    {
                        onQuery?.Invoke(DatabaseQueryResultType.FAIL_CONNECTION, new RegisterQueryData("User already exists"));
                    }
                    else
                    {
                        onQuery?.Invoke(DatabaseQueryResultType.FAIL_QUERY, new RegisterQueryData(text));
                    }
                }
                else
                {
                    User user = new User(text);
                    onQuery?.Invoke(DatabaseQueryResultType.SUCCESS, new RegisterQueryData(user, extrainfo));
                }
            }
        }
        
        public void GetUserBase(int userid, string extrainfo = "")
        {
            StartCoroutine(GetUserBaseByIdCo(userid, extrainfo));
        }

        public void GetUserFull(int userid, string extrainfo = "")
        {
            StartCoroutine(GetUserFullByIdCo(userid, extrainfo));
        }

        private IEnumerator GetUserBaseByIdCo(int userid, string extrainfo = "")
        {
            WWWForm form = new WWWForm();
            form.AddField("userid", userid);

            UnityWebRequest www = UnityWebRequest.Post(GetUserPath("getuserbyid"), form);
            yield return www.SendWebRequest();
            Debug.Log($"GetUserID text: {www.downloadHandler.text}");

            if (www.result != UnityWebRequest.Result.Success)
            {
                onQuery?.Invoke(DatabaseQueryResultType.FAIL_CONNECTION, new GetUserQuery(www.error));
            }
            else
            {
                string text = www.downloadHandler.text;
                if (text.Contains("Error"))
                {
                    Debug.Log("GetUserID text: " + text);
                    if (text == "Error0")
                    {
                        onQuery?.Invoke(DatabaseQueryResultType.FAIL_CONNECTION, new GetUserQuery("User already exists"));
                    }
                    else
                    {
                        onQuery?.Invoke(DatabaseQueryResultType.FAIL_QUERY, new GetUserQuery(text));
                    }
                }
                else
                {
                    User user = new User(text);
                    onQuery?.Invoke(DatabaseQueryResultType.SUCCESS, new GetUserQuery(user));
                }
            }

        }
        private IEnumerator GetUserFullByIdCo(int userid, string extrainfo = "")
        {
            WWWForm form = new WWWForm();
            form.AddField("userid", userid);

            UnityWebRequest www = UnityWebRequest.Post(GetUserPath("getuserfullbyid"), form);
            yield return www.SendWebRequest();
            Debug.Log($"GetUserID text: {www.downloadHandler.text}");

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log($"getuserid error: {www.result}");
                onQuery?.Invoke(DatabaseQueryResultType.FAIL_CONNECTION, new GetUserQuery(www.error));
            }
            else
            {
                string text = www.downloadHandler.text;
                if (text.Contains("Error"))
                {
                    Debug.Log("GetUserID text: " + text);
                    if (text == "Error0")
                    {
                        onQuery?.Invoke(DatabaseQueryResultType.FAIL_CONNECTION, new GetUserQuery("User already exists"));
                    }
                    else
                    {
                        onQuery?.Invoke(DatabaseQueryResultType.FAIL_QUERY, new GetUserQuery(text));
                    }
                }
                else
                {
                    User user = new User(text);
                    onQuery?.Invoke(DatabaseQueryResultType.SUCCESS, new GetUserQuery(user, extrainfo));
                }
            }

        }


        public void AddLoginInstance(User user, string devicemodel = "", string devicename = "", string devicetype = "", string extrainfo = "")
        {
            StartCoroutine(AddLoginInstanceCo(user,devicemodel,devicename,devicetype, extrainfo));
        }
        private IEnumerator AddLoginInstanceCo(User user, string devicemodel = "", string devicename = "", string devicetype = "", string extrainfo = "")
        {
            string logindata = "";
            
            WWWForm form = new WWWForm();
            form.AddField("userid", user.ID);
            form.AddField("devicemodel", devicemodel);
            form.AddField("devicename", devicename);
            form.AddField("devicetype", devicetype);
            form.AddField("logindata", logindata);

            string file = GetUserPath("addlogininstance");
            UnityWebRequest www = UnityWebRequest.Post(file, form);
            yield return www.SendWebRequest();
            Debug.Log("addlogininstance text: " + www.downloadHandler.text, this);

            if (www.result != UnityWebRequest.Result.Success)
            {
                onQuery?.Invoke(DatabaseQueryResultType.FAIL_CONNECTION, new AddLoginInstanceQuery("Unexpected error while connecting to server. Contact administration."));
            }
            else
            {
                string text = www.downloadHandler.text;
                Debug.Log("addlogininstance text: " + text);
                if (!text.Contains("Error"))
                {
                    //var ldata = DatabaseParsers.ParseAddLogin(text, extrainfo);
                    var ldata = new LoginData(text);
                    var logininstance = new AddLoginInstanceQuery(ldata, extrainfo);
                    onQuery?.Invoke(DatabaseQueryResultType.SUCCESS, logininstance);
                }
                else
                {
                    onQuery?.Invoke(DatabaseQueryResultType.FAIL_QUERY, new AddLoginInstanceQuery(text));
                }
            }
        }
        
        public void AddLogoutInstance(LoginData loginData)
        {
            StartCoroutine(AddLogoutInstanceCo(loginData));
        }
        private IEnumerator AddLogoutInstanceCo(LoginData loginData)
        {
            WWWForm form = new WWWForm();
            form.AddField("loginid", loginData.ID);

            string file = GetUserPath("addlogoutinstance");
            UnityWebRequest www = UnityWebRequest.Post(file, form);
            yield return www.SendWebRequest();
            Debug.Log("addlogoutinstance text: " + www.downloadHandler.text, this);

            if (www.result != UnityWebRequest.Result.Success)
            {
                onQuery?.Invoke(DatabaseQueryResultType.FAIL_CONNECTION, null);
            }
            else
            {
                string text = www.downloadHandler.text;
                Debug.Log("addlogoutinstance text: " + text);
                if (!text.Contains("Error"))
                {
                    onQuery?.Invoke(DatabaseQueryResultType.SUCCESS, null);
                }
                else
                {
                    onQuery?.Invoke(DatabaseQueryResultType.FAIL_QUERY, null);
                }
            }
        }
        #endregion

        #region Score Functions
        public void RegisterTimerace(GameInstance gameInstance, int winnerid, int gamedurationsecs, bool insert, string extrainfo = "")
        {
            StartCoroutine(RegisterTimeraceCo(gameInstance, winnerid, gamedurationsecs, insert, extrainfo));
        }
        private IEnumerator RegisterTimeraceCo(GameInstance gameInstance, int winnerid, int gamedurationsecs, bool insert, string extrainfo = "")
        {
            WWWForm form = new WWWForm();
            int loserid = (winnerid == gameInstance.User1) ? gameInstance.User1 : gameInstance.User2;
            form.AddField("winnerid", winnerid);
            form.AddField("loserid", loserid);
            form.AddField("gamedurationseconds", gamedurationsecs);
            form.AddField("gameid", gameInstance.ID);
            form.AddField("gamecode", gameInstance.GameCode);
            form.AddField("insert", insert ? 1 : 0);

            string file = GetScorePath("addtimeracegame");
            UnityWebRequest www = UnityWebRequest.Post(file, form);
            yield return www.SendWebRequest();
            Debug.Log("addtimerace text: " + www.downloadHandler.text, this);

            if (www.result != UnityWebRequest.Result.Success)
            {
                onQuery?.Invoke(DatabaseQueryResultType.FAIL_CONNECTION, new AddTimeraceScoreQuery("Unexpected error while connecting to server. Contact administration."));
            }
            else
            {
                string text = www.downloadHandler.text;
                Debug.Log("addtimerace text: " + text);
                if (!text.Contains("Error"))
                {
                    TimeraceResult res = new TimeraceResult(text);
                    var timeraceScoreData = new AddTimeraceScoreQuery(text, extrainfo);
                    onQuery?.Invoke(DatabaseQueryResultType.SUCCESS, timeraceScoreData);
                }
                else
                {
                    onQuery?.Invoke(DatabaseQueryResultType.FAIL_QUERY, new AddTimeraceScoreQuery(text));
                }
            }
        }

        public void GetTimeraceLeaderboard()
        {
            StartCoroutine(GetTimeraceLeaderboardCo());
        }
        private IEnumerator GetTimeraceLeaderboardCo()
        {
            WWWForm form = new WWWForm();

            string file = GetScorePath("gettimeraceleaderboard");
            UnityWebRequest www = UnityWebRequest.Post(file, form);
            yield return www.SendWebRequest();
            Debug.Log("gettimeraceL text: " + www.downloadHandler.text, this);

            if (www.result != UnityWebRequest.Result.Success)
            {
                onQuery?.Invoke(DatabaseQueryResultType.FAIL_CONNECTION, new GetTimeraceLeaderboardQuery("Unexpected error while connecting to server. Contact administration."));
            }
            else
            {
                string text = www.downloadHandler.text;
                Debug.Log("addtimerace text: " + text);
                if (!text.Contains("Error"))
                {
                    TimeraceLeaderboard l = new TimeraceLeaderboard(text);
                    onQuery?.Invoke(DatabaseQueryResultType.SUCCESS, new GetTimeraceLeaderboardQuery(l));
                }
                else
                {
                    onQuery?.Invoke(DatabaseQueryResultType.FAIL_QUERY, new GetTimeraceLeaderboardQuery(text));
                }
            }
        }
        #endregion

        #region Game
        public void CreateGameInstance(
            User user1,
            User user2,
            GameMode gameMode,
            bool isRanked,
            bool isPrivate,
            string privateCode,
            int punServerTimestamp,
            string punServerAddress,
            string punGameVersion,
            string punAppVersion,
            string punCloudRegion,
            bool insert
        )
        {
            StartCoroutine(CreateGameInstanceCo(user1,user2,gameMode,isRanked,isPrivate,privateCode,punServerTimestamp,punServerAddress,punGameVersion,punAppVersion,punCloudRegion, insert));
        }

        private IEnumerator CreateGameInstanceCo(
            User host,
            User other,
            GameMode gameMode,
            bool isRanked,
            bool isPrivate,
            string privateCode,
            int punServerTimestamp,
            string punServerAddress,
            string punGameVersion,
            string punAppVersion,
            string punCloudRegion,
            bool insert
        )
        {
            WWWForm form = new WWWForm();
            int otherID = (other != null) ? other.ID : -1;
            form.AddField("hostid", host.ID);
            form.AddField("otherid", otherID);
            form.AddField("gamemodeid", (int)gameMode);
            form.AddField("isranked", isRanked ? 1 : 0);
            form.AddField("isprivate", isPrivate ? 1 : 0);
            form.AddField("privatecode", privateCode);
            form.AddField("photon_server_timestamp", punServerTimestamp);
            form.AddField("photon_server_address", punServerAddress);
            form.AddField("photon_game_version", punGameVersion);
            form.AddField("photon_app_version", punAppVersion);
            form.AddField("photon_cloud_region", punCloudRegion);
            form.AddField("insert", insert ? 1 : 0);

            string file = GetGamePath("creategameinstance");
            UnityWebRequest www = UnityWebRequest.Post(file, form);
            yield return www.SendWebRequest();
            Debug.Log("creategameinstance text: " + www.downloadHandler.text, this);

            if (www.result != UnityWebRequest.Result.Success)
            {
                onQuery?.Invoke(DatabaseQueryResultType.FAIL_CONNECTION, new CreateGameInstanceQuery("Unexpected error while connecting to server. Contact administration."));
            }
            else
            {
                string text = www.downloadHandler.text;
                Debug.Log("creategameinstance text: " + text);
                if (!text.Contains("Error"))
                {
                    GameInstance game = new GameInstance(text);
                    onQuery?.Invoke(DatabaseQueryResultType.SUCCESS, new CreateGameInstanceQuery(game));
                }
                else
                {
                    onQuery?.Invoke(DatabaseQueryResultType.FAIL_QUERY, new CreateGameInstanceQuery(text));
                }
            }
        }
        #endregion

    }

}

