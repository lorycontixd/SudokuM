using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Achievement = GoogleData.Achievement;

public class AchievementsManager : MonoBehaviour
{
    #region Singleton
    private static AchievementsManager _instance;
    public static AchievementsManager Instance { get { return _instance; } }

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
    private List<Achievement> achievements;


    private IEnumerator Start()
    {
        achievements = new List<GoogleData.Achievement>();
        yield return new WaitUntil(() => DataManager.Instance.IsReady);
        Debug.Log($"[AchievementManage] IsReady");
        achievements = DataManager.Instance.GoogleData.Achievements;
        Debug.Log($"[AchievementManager] Ach: {achievements}, {achievements.Count}");
        IsReady = true;
    }

    public Achievement FindAchievementDataByName(string name)
    {
        return achievements.FirstOrDefault(a => a.Name == name);
    }
    public Achievement FindAchiementDataByPlayID(string playID)
    {
        return achievements.FirstOrDefault(a => a.PlayId == playID);
    }

    public void GiveFirstTimeAchievement()
    {
        if (GooglePlayManager.Instance == null)
        {
            return;
        }
        if (GooglePlayManager.Instance.IsActive && GooglePlayManager.Instance.IsAuthenticated)
        {
            if (DataManager.Instance != null)
            {
                if (DataManager.Instance.GoogleData != null)
                {
                    Achievement a = FindAchievementDataByName("First time for everything");
                    PlayGamesPlatform.Instance.UnlockAchievement(a.PlayId, (bool success) => {
                        Debug.Log($"[AchievementManager] FirstTimeAchievement => success: {success}");
                    });
                }
            }
        }
    }
}
