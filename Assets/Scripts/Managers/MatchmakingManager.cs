using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchmakingManager : MonoBehaviour
{
    #region Singleton
    private static MatchmakingManager _instance;
    public static MatchmakingManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    public static Dictionary<GameMode, int> GameModePlayers = new Dictionary<GameMode, int>()
    {
        {GameMode.SINGLEPLAYER, 1},
        {GameMode.COOP, 2 },
        {GameMode.TIMERACE, 2 }
    };


    /// <summary>
    /// Find player based on skill level
    /// </summary>
    public void FindRandomPlayer()
    {

    }
}
