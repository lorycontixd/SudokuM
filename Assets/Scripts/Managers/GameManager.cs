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

    public GameMode GameMode { get; private set; }
    public bool IsRankedGame { get; private set; }
    public bool IsPrivateGame { get; private set; }
    public string RoomCode { get; private set; }

    [SerializeField] private SudokuCanvas sudokuCanvas;
    private int[,] currentPuzzle = null;
    private int[,] currentSolution = null;
    private int currentRating = -1;

    [Header("Settings")]
    public Color localPlayerCellColour = Color.green;
    public Color otherPlayerCellColour = Color.blue;

    public static int NullCellValue = 0;



    private void Start()
    {

        DatabaseManager.Instance.onQuery.AddListener(OnDbQuery);
        if (PhotonNetwork.IsConnectedAndReady)
        {
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
        PhotonNetwork.LeaveRoom();
    }


    #region Pun Callbacks
    
    public override void OnLeftRoom()
    {
        Debug.Log($"I left the room! ==> Am i in lobby? {PhotonNetwork.InLobby}, Am i connected? {PhotonNetwork.IsConnected}");
    }
    
    #endregion
}
