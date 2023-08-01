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
        if (PhotonNetwork.IsConnectedAndReady)
        {
            GameMode = (GameMode)PhotonNetwork.CurrentRoom.CustomProperties["mode"];
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log($"GameManager->Master->creating new game");
                NewGame();
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
        PhotonNetwork.LoadLevel("Lobby");
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PhotonNetwork.LeaveRoom();
    }
    #endregion
}
