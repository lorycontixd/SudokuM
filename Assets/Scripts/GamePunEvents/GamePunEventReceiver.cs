using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class GamePunEventReceiver : MonoBehaviour, IOnEventCallback
{
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == GamePunEventSender.SendBoardEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            int N = (int)data[2];
            int[,] board = MatrixUtilities.ComposeMatrix((int[])data[0], N);
            int[,] solution = MatrixUtilities.ComposeMatrix((int[])data[1], N);
            int rating = (int)data[3];
            Debug.Log($"[EventReceiver] Received Board ==> N: {N}, Rating: {rating} ==> Initializing...");
            SudokuCanvas.Instance.InitializeBoard(board, solution, rating);
        }
        else if (eventCode == GamePunEventSender.SendMoveEventCode)
        {

            object[] data = (object[])photonEvent.CustomData;
            int userid = (int)data[0];
            string username = (string)data[1];
            int row = (int)data[2];
            int col = (int)data[3];
            int digit = (int)data[4];
            bool isCorrect = (bool)data[5];
            bool isDeleteMove = (bool)data[6];
            int completedCells = (int)data[7];
            Debug.Log($"[EventReceiver] Recived move ==> {row}, {col}, {digit}, {isCorrect}, {isDeleteMove}, {completedCells}");
            SudokuCanvas.Instance.UpdateCellMultiplayer(userid, username, row, col, digit, isCorrect, isDeleteMove, completedCells);
        }
        else if (eventCode == GamePunEventSender.SendHalfBoardEventCode)
        {
            Debug.Log($"RECEIVED HALF BOARD CODE");
            object[] data = (object[])photonEvent.CustomData;
            int userid = (int)data[0];
            if (AudioManager.Instance != null)
            {
                bool success = AudioManager.Instance.PlayHalfBoardNotification();
                if (success)
                {
                    SudokuCanvas.Instance.ShowEnemyHalfBoard();
                }
            }
        }
        else if (eventCode == GamePunEventSender.SendFinishEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            int userid = (int)data[0];
            int winnerid = (int)data[1];
            SudokuCanvas.Instance.FinishAndClose(winnerid == PhotonNetwork.LocalPlayer.ActorNumber);
        }
        else if (eventCode == GamePunEventSender.SendLossEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            int userid = (int)data[0];
            int loserid = (int)data[1];
            SudokuCanvas.Instance.FinishAndClose(loserid != PhotonNetwork.LocalPlayer.ActorNumber);
        }
        else if (eventCode == GamePunEventSender.SendPauseEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            int userid = (int)data[0];
            bool isMe = userid == PhotonNetwork.LocalPlayer.ActorNumber;
            if (isMe)
            {
                Debug.LogError($"[UnpauseEventReceived] Impossible that i get my own messages!");
                return;
            }
            if (PauseManager.Instance != null)
            {
                PauseManager.Instance.OnPauseReceived(userid);
            }
        }
        else if (eventCode == GamePunEventSender.SendUnpauseEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            int userid = (int)data[0];
            int reasonCode = (int)data[1];
            PauseManager.UnpauseReason reason = (PauseManager.UnpauseReason)reasonCode;
            bool isMe = userid == PhotonNetwork.LocalPlayer.ActorNumber;
            if (isMe)
            {
                Debug.LogError($"[UnpauseEventReceived] Impossible that i get my own messages!");
                return;
            }
            if (PauseManager.Instance != null)
            {
                PauseManager.Instance.OnUnpauseReceived(userid, reason);
            }
        }
        else if (eventCode == GamePunEventSender.SendEmojiEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            int userid = (int)data[0];
            int emojiId = (int)data[1];
            EmojiManager.Instance.ReceiveEmoji(emojiId);
        }
        else if (eventCode == GamePunEventSender.SendGameInstanceEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            int id = (int)data[0];
            int user1 = (int)data[1];
            int user2 = (int)data[2];
            int gamemode = (int)data[3];
            string gamecode = (string)data[4];
            bool isranked = (bool)data[5];
            bool isprivate = (bool)data[6];
            string roomcode = (string)data[7];
            int serverTimestamp = (int)data[8];
            string serverAddress = (string)data[9];
            string gameVersion = (string)data[10];
            string appVersion = (string)data[11];
            string cloudRegion = (string)data[12];
            string serverDate = (string)data[13];

            GameInstance gameInstance = new GameInstance(
                id, user1, user2, gamemode, gamecode, isranked, isprivate, roomcode, serverTimestamp,
                serverAddress, gameVersion, appVersion, cloudRegion, serverDate
            );
            SessionManager.Instance.SetGameInstance(gameInstance);
        }
        else if (eventCode == GamePunEventSender.SendReconnectedEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            int actorNumber = (int)data[0];
            Player player = PhotonNetwork.PlayerListOthers.FirstOrDefault(p => p.ActorNumber == actorNumber);
            NetworkManager.Instance.OnOtherPlayerReconnected(player);
        }
        else if (eventCode == GamePunEventSender.SendReconnectionCheckEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            int sender = (int)data[0];
            int dcActorNumber = (int)data[1];
            Debug.Log($"[PunRecv] Receive reconn check from {sender} to {dcActorNumber}");
            if (dcActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                NetworkManager.Instance.PlayerRequestingCheck = sender;
                bool IsConnectedAndReady = PhotonNetwork.IsConnectedAndReady;
                bool IsInRoom = PhotonNetwork.InRoom && PhotonNetwork.InLobby;
                bool CorrectStatus = PhotonNetwork.NetworkClientState == ClientState.Joined;
                
                if (!IsConnectedAndReady)
                {
                    GamePunEventSender.SendReconnectionCheckReply(sender, (int)NetworkManager.ReconnectionCheckStatus.NOTCONNECTED);
                }
                else
                {
                    if (!IsInRoom)
                    {
                        GamePunEventSender.SendReconnectionCheckReply(sender, (int)NetworkManager.ReconnectionCheckStatus.NOTINROOM);
                    }
                    else
                    {
                        GamePunEventSender.SendReconnectionCheckReply(sender, (int)NetworkManager.ReconnectionCheckStatus.SUCCESS);
                    }
                }
            }
        }else if (eventCode == GamePunEventSender.SendReconnectionCheckReplyEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            int target = (int)data[0];
            int statusCode = (int)data[1];
            Debug.Log($"[PunRecv] Receive reconn check reply from {target}");
            NetworkManager.ReconnectionCheckStatus code = (NetworkManager.ReconnectionCheckStatus)statusCode;
            if (target == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                if (code == NetworkManager.ReconnectionCheckStatus.SUCCESS)
                {
                    // Can restart
                    GamePunEventSender.SendResumeGameAfterReconnection();
                }
            }
        }else if (eventCode == GamePunEventSender.SendResumeGameAfterReconnectionEventCode)
        {
            Debug.Log($"[PunRecv] Resume after dc!!");
            NetworkManager.Instance.ResumeGameAfterDisconnect();
        }
    }
}
