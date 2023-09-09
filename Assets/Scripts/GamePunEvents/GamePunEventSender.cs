using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
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
    public const byte SendHalfBoardEventCode = 4;
    public const byte SendFinishEventCode = 5;
    public const byte SendLossEventCode = 6;
    public const byte SendPauseEventCode = 7;
    public const byte SendUnpauseEventCode = 8;
    public const byte SendEmojiEventCode = 9;
    public const byte SendGameInstanceEventCode = 10;
    public const byte SendReconnectedEventCode = 11;
    public const byte SendReconnectionCheckEventCode = 12; // See function for documentation.
    public const byte SendReconnectionCheckReplyEventCode = 13;
    public const byte SendResumeGameAfterReconnectionEventCode = 14;
    public const byte SendLeaveEventCode = 15;
    public const byte SendLeaveRequestReceivedEventCode = 16;


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
        object[] content = new object[] { userid, username, row, col, digit, isCorrect, isDeleteMove, completedCells };
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

    public static void SendHalfBoard(int userid)
    {
        object[] content = new object[] { userid };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(SendHalfBoardEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public static void SendFinish(int userid, int winnerid)
    {
        object[] content = new object[] { userid, winnerid };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(SendFinishEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public static void SendLoss(int userid, int loserid)
    {
        object[] content = new object[] { userid, loserid };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(SendLossEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public static void SendPause(int userid)
    {
        object[] content = new object[] { userid };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(SendPauseEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public static void SendUnpause(int userid, int reasonCode)
    {
        object[] content = new object[] { userid, reasonCode };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(SendUnpauseEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public static void SendEmoji(int userid, int emojiId)
    {
        object[] content = new object[] { userid, emojiId };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(SendEmojiEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public static void SendGameInstance(
        int id, int user1, int user2, int gamemode, string gamecode, bool isRanked, bool isPrivate,
        string roomCode, int punServerTimestamp, string punServerAddress,
        string punGameVersion, string punAppVersion,
        string punCloudRegion, string starttime
    )
    {
        object[] content = new object[] { id, user1, user2, gamemode, gamecode, isRanked, isPrivate, roomCode, punServerTimestamp, punServerAddress, punGameVersion, punAppVersion, punCloudRegion, starttime };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(SendGameInstanceEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public static void SendGameInstance(GameInstance instance)
    {
        object[] content = new object[] {
            instance.ID,
            instance.User1,
            instance.User2,
            (int)instance.GameMode,
            instance.GameCode,
            instance.IsRankedGame,
            instance.PhotonRoomIsPrivate,
            instance.PhotonRoomPrivateCode,
            instance.PhotonServerTimestamp,
            instance.PhotonServerAddress,
            instance.PhotonGameVersionHost,
            instance.PhotonAppVersionHost,
            instance.PhotonCloudRegionHost,
            instance.StartTime.ToString()
        };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(SendGameInstanceEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public static void SendReconnect(Player player, float gameTime)
    {
        object[] content = new object[] { player.ActorNumber, gameTime };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(SendReconnectedEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    /// <summary>
    /// This function asks the recently disconnected player if everything is fine. (He should be reconnected)
    /// Everything is fine means:
    /// 1) PhotonNetwork.IsConnectedAndReady
    /// 2) PhotonNetwork.InRoom and state = joined
    /// 
    /// If everything is fine, the disconnected (reconnected) player replies
    /// </summary>
    /// <param name="disconnectedPlayer"></param>
    public static void SendReconnectionCheck(Player sendingPlayer, Player disconnectedPlayer)
    {
        object[] content = new object[] { sendingPlayer.ActorNumber, disconnectedPlayer.ActorNumber };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(SendReconnectionCheckEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }


    public static void SendReconnectionCheckReply(int sender, int reconStatusCode)
    {
        object[] content = new object[] { sender, reconStatusCode };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        PhotonNetwork.RaiseEvent(SendReconnectionCheckReplyEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public static void SendResumeGameAfterReconnection(float gameTime)
    {
        object[] content = new object[] { gameTime };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(SendResumeGameAfterReconnectionEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public static void SendLeave()
    {
        object[] content = new object[] { PhotonNetwork.LocalPlayer.ActorNumber };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        Debug.Log($"[GPE Sender] Sending leave to all");
        PhotonNetwork.RaiseEvent(SendLeaveEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public static void SendLeaveRequestReceived(int myActorN, int targetActorN)
    {
        object[] content = new object[] { PhotonNetwork.LocalPlayer.ActorNumber, targetActorN };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        Debug.Log($"[GPE Sender] Sending leave request accept to {targetActorN}");
        PhotonNetwork.RaiseEvent(SendLeaveRequestReceivedEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }
}
