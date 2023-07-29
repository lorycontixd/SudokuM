using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class StatsManager : MonoBehaviour
{
    #region Singleton
    private static StatsManager _instance;
    public static StatsManager Instance { get { return _instance; } }

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

    public Dictionary<int, int> correctMoves = new Dictionary<int, int>();
    public Dictionary<int, int> wrongMoves = new Dictionary<int, int>();
    public Dictionary<int, int> totalMoves = new Dictionary<int, int>();

    public Dictionary<int, float> averageTimePerMove = new Dictionary<int, float>();
    public int overallTotalMoves = 0;
    public bool IsSetup { get { return _isSetup; } }

    private Dictionary<int, List<float>> playerMoveTimes = new Dictionary<int, List<float>>();
    private float localPlayerTimestamp = 0;
    private float otherPlayerTimestamp = 0;
    private bool _isSetup = false;


    private void Start()
    {
        ResetStats();
        _isSetup = true;
    }
    private void Update()
    {
        localPlayerTimestamp += Time.deltaTime ;
        otherPlayerTimestamp += Time.deltaTime;
    }

    public void ResetStats()
    {
        correctMoves = new Dictionary<int, int>();
        wrongMoves = new Dictionary<int, int>();
        averageTimePerMove = new Dictionary<int, float>();
        playerMoveTimes = new Dictionary<int, List<float>>();
        totalMoves = new Dictionary<int, int>();
        overallTotalMoves = 0;

        foreach(KeyValuePair<int, Player> kvp in PhotonNetwork.CurrentRoom.Players)
        {
            int actorNumber = kvp.Value.ActorNumber;
            correctMoves.Add(actorNumber, 0);
            wrongMoves.Add(actorNumber, 0);
            totalMoves.Add(actorNumber, 0);
            playerMoveTimes.Add(actorNumber, new List<float>());
            averageTimePerMove.Add(actorNumber, 0);
        }
    }

    public void AddMove(Move move)
    {
        bool isCorrect = move.IsCorrect;
        int userid = move.UserID;
        bool isMe = PhotonNetwork.LocalPlayer.ActorNumber == userid;
        if (isCorrect)
        {
            correctMoves[userid]++;
        }
        else
        {
            wrongMoves[userid]++;
        }
        totalMoves[userid]++;
        overallTotalMoves++;

        // avg. move time
        if (isMe)
        {
            playerMoveTimes[userid].Add(localPlayerTimestamp);
            localPlayerTimestamp = 0;
        }
        else
        {
            playerMoveTimes[userid].Add(otherPlayerTimestamp);
            otherPlayerTimestamp = 0;
        }
        averageTimePerMove[userid] = playerMoveTimes[userid].Average();
    }
}
