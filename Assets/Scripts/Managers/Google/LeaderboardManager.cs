using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Leaderboard = GoogleData.Leaderboard;

public class LeaderboardManager : MonoBehaviour
{
    #region Singleton
    private static LeaderboardManager _instance;
    public static LeaderboardManager Instance { get { return _instance; } }

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

    // Privates
    public bool IsReady { get; private set; } = false;
    private List<Leaderboard> leaderboards = new List<Leaderboard>();
    private List<ILeaderboard> playLeaderboards = new List<ILeaderboard>();

    // Events
    public Action<bool, int, int> onGetPlayerScore; // (success, playerscore, maxscore)


    private IEnumerator Start()
    {
        leaderboards = new List<Leaderboard>();
        yield return new WaitUntil(() => DataManager.Instance.IsReady);
        Debug.Log($"[AchievementManage] IsReady");
        leaderboards = DataManager.Instance.GoogleData.Leaderboards;
        Debug.Log($"[AchievementManager] Ach: {leaderboards}, {leaderboards.Count}");
        IsReady = true;

        CheckInitialLeaderboards();
        //StartCoroutine(ShowLeaderboardDelay(leaderboards[0].PlayId));
    }

    private void CheckInitialLeaderboards()
    {
        if (GooglePlayManager.Instance.IsAuthenticated)
        {
            if (leaderboards.Count > 0)
            {
                playLeaderboards = new List<ILeaderboard>();
                foreach (Leaderboard leaderboard in leaderboards)
                {
                    ILeaderboard l = PlayGamesPlatform.Instance.CreateLeaderboard();
                    l.id = leaderboard.PlayId;

                    PlayGamesPlatform.Instance.LoadScores(
                        leaderboard.PlayId,
                        LeaderboardStart.PlayerCentered,
                        29,
                        LeaderboardCollection.Public,
                        LeaderboardTimeSpan.AllTime,
                        (data) =>
                        {
                            if (data.Status == ResponseStatus.Success)
                            {
                                if (data.PlayerScore == null)
                                {
                                    PlayGamesPlatform.Instance.ReportScore(0, leaderboard.PlayId, (bool success) =>
                                    {
                                        Debug.Log($"[LeadMgr->Start] User score for leaderbaord {leaderboard.Name} was null and was added successfully");
                                    });
                                }
                                else
                                {
                                    Debug.Log($"[LeadMgr->Start] User score for leaderbaord {leaderboard.Name} found => {data.PlayerScore.value}");
                                }
                            }
                            else
                            {
                                Debug.LogWarning($"[LeadMgr->Start] Failed to load scores for leaderboard {leaderboard.Name}/{leaderboard.PlayId}");
                            }
                        
                        });
                    playLeaderboards.Add( l );
                }
            }
        }
    }

    public void GetScores()
    {
        bool isvalid = false;
        
        //int score = (int)playLeaderboards.FirstOrDefault(l => l.id == GPGSIds.leaderboard_time_race).localUserScore.value;
        //int maxScore = (int)playLeaderboards.FirstOrDefault(l => l.id == GPGSIds.leaderboard_time_race).scores.Max(m => m.value);
        ILeaderboard lb = PlayGamesPlatform.Instance.CreateLeaderboard();
        lb.id = GPGSIds.leaderboard_time_race;
        lb.LoadScores(ok =>
        {
            if (ok)
            {
                Debug.Log($"[Leadmg->getscores] load scores ok");
                int score = (int)lb.localUserScore.value;
                int maxScore = (int)lb.scores.Max(m => m.value);
                onGetPlayerScore?.Invoke(true, score, maxScore);
            }
            else
            {
                int score = int.MinValue;
                int maxScore = int.MinValue;
                Debug.Log("[LeadMgr->Getscores] Error retrieving leaderboardi");
                onGetPlayerScore?.Invoke(false, score, maxScore);
            }
        });
    }


    public void RegisterTimeraceScore(int newScore)
    {
        if (GooglePlayManager.Instance.IsActive && GooglePlayManager.Instance.IsAuthenticated)
        {
            PlayGamesPlatform.Instance.ReportScore(newScore, GPGSIds.leaderboard_time_race, (bool success) => { });
        }
    }
}
