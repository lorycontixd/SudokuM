using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class GamePunEventSender
{
    public const byte SendQuitEventCode = 0;
    public const byte SendBoardEventCode = 1;
    public const byte SendMoveEventCode = 2;
    public const byte SendDigitSelectEventCode = 3;
    public const byte SendFinishEventCode = 4;

    public static void SendBoard(int[,] board, int[,] solution, int rating)
    {
        Debug.Log($"Board: {board}, is null: {board == null}");
        Debug.Log($"Sol: {solution}, is null: {solution == null}");
        Debug.Log($"[EventSender] Starting send board ==> board_size: ({board.GetLength(0)},{board.GetLength(1)}), sol_size: ({solution.GetLength(0)},{solution.GetLength(1)}), rating: {rating}");
        (int[], int) boardRes = MatrixUtilities.DecomposeMatrix(board);
        int[] flatBoard = boardRes.Item1;
        int N = boardRes.Item2;
        int[] flatSolution = MatrixUtilities.DecomposeMatrix(solution).Item1;
        SendBoard(flatBoard, flatSolution, N, rating);
    }
    public static void SendBoard(int[] board, int[] solution, int colNumber, int rating)
    {
        object[] content = new object[] { board, solution, colNumber, rating }; 
        Debug.Log($"[EventSender] Sending board to all ==> ColNumber: {colNumber}");
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(SendBoardEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public static void SendMove(int userid, string username, int row, int col, int digit, bool isCorrect, bool isDeleteMove, int completedCells, ReceiverGroup receivers = ReceiverGroup.All)
    {
        Debug.Log($"[EventSender] Sending move ==> {row}, {col}, {digit}, {isCorrect}, {isDeleteMove}, {completedCells}");
        object[] content = new object[] {userid, username, row, col, digit, isCorrect, isDeleteMove, completedCells }; 
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = receivers };
        PhotonNetwork.RaiseEvent(SendMoveEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public static void SendQuit(int userid, string username)
    {
        object[] content = new object[] { userid, username };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(SendQuitEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public static void SendDigitSelection(int userid, string username, int row, int col)
    {
        object[] content = new object[] { userid, username, row, col };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(SendDigitSelectEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public static void SendFinish(int userid, int winnerid)
    {
        object[] content = new object[] { userid, winnerid };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(SendFinishEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }
}
