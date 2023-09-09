using Managers;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviourPunCallbacks
{
    #region Singleton
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

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

    // Game Props
    public GameMode GameMode { get; private set; }
    public bool IsRankedGame { get; private set; }
    public bool IsPrivateGame { get; private set; }
    public string RoomCode { get; private set; }

    // Game Vars
    public float GameTime { get; private set; }
    public bool IsPlaying { get; set; }

    [SerializeField] private SudokuCanvas sudokuCanvas;
    private int[,] currentPuzzle = null;
    private int[,] currentSolution = null;
    private int currentRating = -1;
    private bool pendingFinishIsWin = false;

    [Header("Settings")]
    public Color localPlayerCellColourCoop = Color.green;
    public Color otherPlayerCellColourCoop = Color.blue;

    public static int NullCellValue = 0;

    public Action<bool, bool, int, int> onGameFinish; // params = (iswin, registergame)



    private void Start()
    {
        DatabaseManager.Instance.onQuery.AddListener(OnDbQuery);
        if (PhotonNetwork.IsConnectedAndReady)
        {
            NetworkManager.Instance.SwitchScene(NetworkManager.SceneType.GAME);
            NetworkManager.Instance.IsConnectedToGame = true;
            GameMode = (GameMode)PhotonNetwork.CurrentRoom.CustomProperties["mode"];
            IsRankedGame = (bool)PhotonNetwork.CurrentRoom.CustomProperties["r"];
            IsPrivateGame = (bool)PhotonNetwork.CurrentRoom.CustomProperties["p"];
            RoomCode = (string)PhotonNetwork.CurrentRoom.CustomProperties["code"];
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log($"GameManager->Master->creating new game");
                NewGame();
            }
        }
    }

    private void OnDbQuery(DatabaseQueryResultType resultType, QueryData data)
    {
        if (data.QueryType == QueryType.CREATEGAMEINSTANCE)
        {
            if (resultType == DatabaseQueryResultType.SUCCESS)
            {
                CreateGameInstanceQuery gidata = (CreateGameInstanceQuery)data;
                GameInstance gameInstance = gidata.gameInstance;
                Debug.Log($"Create Game INstance successful ==> GI: {gameInstance.User1}, {gameInstance.User2}, ir: {gameInstance.IsRankedGame}, ip: {gameInstance.PhotonRoomIsPrivate}");
                SessionManager.Instance.SetGameInstance(gameInstance);
                GamePunEventSender.SendGameInstance(gameInstance);
            }
        }
        else if (data.QueryType == QueryType.ADDTIMERACESCORE)
        {
            if (resultType == DatabaseQueryResultType.SUCCESS)
            {
                Debug.Log($"[SC] OnDbQuery -> AddTimeraceScore successful");
                AddTimeraceScoreQuery q = (AddTimeraceScoreQuery)data;
                bool isWin = (int)q.result.winner["id"] == SessionManager.Instance.ActiveUser.ID;
                int scoreChange = -1;
                if (isWin)
                {
                    scoreChange = (int)q.result.winner["scoreChange"];
                }
                else
                {
                    scoreChange = (int)q.result.loser["scoreChange"];
                }
                onGameFinish?.Invoke(pendingFinishIsWin, true, SessionManager.Instance.ActiveUser.Scores.ScoreTimerace, scoreChange);
                //UpdateFinalScoreText(isWin, SessionManager.Instance.ActiveUser.Scores.ScoreTimerace, scoreChange);
            }
        }
    }

    public void NewGame()
    {
        SudokuGenerator sg = new SudokuGenerator();
        currentPuzzle = sg.generate();
        Debug.Log($"[GameManager] new puzzle generated!");
        SudokuSolver solver = new SudokuSolver(currentPuzzle);
        int code = solver.solve();
        Debug.Log($"[GameManager] new puzzle solved!");
        currentSolution = solver.getSolvedBoard();
        //currentRating = SudokuStore.calculatePuzzleRating(currentPuzzle);
        currentRating = -1;
        if (HistoryManager.Instance != null)
        {
            HistoryManager.Instance.Clear();
        }
        if (StatsManagerCoop.Instance != null)
        {
            StatsManagerCoop.Instance.ResetStats();
        }

        Debug.Log($"Sending board across!");
        GamePunEventSender.SendBoard(currentPuzzle, currentSolution, currentRating);

        if (DatabaseManager.Instance != null)
        {
            Debug.LogWarning($"Creating game instance!");
            DatabaseManager.Instance.CreateGameInstance(
                SessionManager.Instance.ActiveUser,
                SessionManager.Instance.OtherUser,
                GameMode,
                IsRankedGame,
                IsPrivateGame,
                RoomCode,
                PhotonNetwork.ServerTimestamp,
                PhotonNetwork.ServerAddress,
                "0.0.0",
                //PhotonNetwork.GameVersion,
                PhotonNetwork.AppVersion,
                PhotonNetwork.CloudRegion,
                PhotonNetwork.IsMasterClient
            );
        }
    }

    public void RetryGame()
    {
        GamePunEventSender.SendBoard(currentPuzzle, currentSolution, currentRating);
        if (HistoryManager.Instance != null)
        {
            HistoryManager.Instance.Clear();
        }
        if (StatsManagerCoop.Instance != null)
        {
            StatsManagerCoop.Instance.ResetStats();
        }
    }
    

    public void Quit()
    {
        GamePunEventSender.SendQuit(PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.NickName);
        NetworkManager.Instance.ImLeaving = true;
        PhotonNetwork.LeaveRoom();
    }


    public void Finish(bool isWin)
    {
        Debug.Log($"Finish!!");
        IsPlaying = false;

        if (GameManager.Instance.IsRankedGame && PhotonNetwork.CurrentRoom.PlayerCount > 1 && SessionManager.Instance.OtherUser != null)
        {
            pendingFinishIsWin = isWin;
            DatabaseManager.Instance.RegisterTimerace(
                SessionManager.Instance.GameInstance,
                (isWin) ? SessionManager.Instance.ActiveUser.ID : SessionManager.Instance.OtherUser.ID,
                (int)GameTime,
                PhotonNetwork.IsMasterClient
            );
        }
        else
        {
            onGameFinish?.Invoke(isWin, false, -1, -1);
        }

    }

    /// <summary>
    /// Send leave request to other players. Will leave the game once other players have received the request.
    /// Once received the feedback from other players, will call GameManager.LeaveGame from  GamePunEventReceiver.
    /// </summary>
    public void SendLeaveRequest()
    {
        Debug.Log($"[GM] Sending leave request");
        NetworkManager.Instance.ImLeaving = true;
        GamePunEventSender.SendLeave();
    }

    /// <summary>
    /// Leaves the game immediately.
    /// Typically called after sending a leave event and receiving player feedback. If called directly leaves immediately.
    /// </summary>
    public void LeaveGame()
    {
        PhotonNetwork.Disconnect();
    }

    public void SetGametime(float time)
    {
        GameTime = time;
    }


    #region Pun Callbacks
    #endregion
}
