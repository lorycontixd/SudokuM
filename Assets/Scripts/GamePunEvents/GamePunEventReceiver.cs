using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        }else if (eventCode == GamePunEventSender.SendMoveEventCode)
        {

            object[] data = (object[])photonEvent.CustomData;
            int userid = (int)data[0];
            string username = (string)data[1];
            int row = (int)data[2];
            int col = (int)data[3];
            int digit = (int)data[4];
            bool isCorrect = (bool)data[5];
            bool isDeleteMove = (bool)data[6];
            Debug.Log($"[EventReceiver] Recived move ==> {row}, {col}, {digit}, {isCorrect}, {isDeleteMove}");
            SudokuCanvas.Instance.UpdateCellMultiplayer(userid, username, row, col, digit, isCorrect, isDeleteMove);
        }
    }
}
