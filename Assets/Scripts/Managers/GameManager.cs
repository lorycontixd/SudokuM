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

    [SerializeField] private SudokuCanvas sudokuCanvas;
    private int[,] currentPuzzle = null;
    private int[,] currentSolution = null;
    private int currentRating = -1;

    [Header("Settings")]
    public Color localPlayerCellColour = Color.green;
    public Color otherPlayerCellColour = Color.blue;



    private void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                NewGame();
            }
        }
    }


    public void NewGame()
    {
        SudokuGenerator sg = new SudokuGenerator();
        currentPuzzle = sg.generate();
        SudokuSolver solver = new SudokuSolver(currentPuzzle);
        int code = solver.solve();
        currentSolution = solver.getSolvedBoard();
        currentRating = SudokuStore.calculatePuzzleRating(currentPuzzle);
        if (HistoryManager.Instance != null)
        {
            HistoryManager.Instance.Clear();
        }
        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.ResetStats();
        }
        GamePunEventSender.SendBoard(currentPuzzle, currentSolution, currentRating);
    }

    public void RetryGame()
    {
        GamePunEventSender.SendBoard(currentPuzzle, currentSolution, currentRating);
        if (HistoryManager.Instance != null)
        {
            HistoryManager.Instance.Clear();
        }
        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.ResetStats();
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