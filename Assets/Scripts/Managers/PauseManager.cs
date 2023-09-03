using Managers;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    #region Singleton
    private static PauseManager _instance;
    public static PauseManager Instance { get { return _instance; } }

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

    #region Unpause Reason
    public enum UnpauseReason
    {
        TIMEOUT = 0,
        MANUAL = 1
    }
    #endregion


    public bool PauseHasDuration = true;
    public float PauseDuration = 30f;
    public int MaxPauses = 3;
    public int MyPauses { get; private set; }
    public int OtherPauses { get; private set; }
    public float PauseTime { get => PauseDuration - _pauseTimestamp; }

    public bool IsReady { get; private set; } // Is manager ready
    public bool IsPaused { get; private set; } = false;
    public Player PausingPlayer;
    private float _pauseTimestamp = 0f;

    public Action<int, bool> onGamePause; // Params = (userid, isme)
    public Action<int, bool, UnpauseReason> onGameUnpause; // Params = (userid, isme, reason)


    private void Start()
    {
        _pauseTimestamp = 0f;
        IsReady = true;
    }

    private void Update()
    {
        if (IsPaused && PauseHasDuration)
        {
            _pauseTimestamp += Time.deltaTime;
            if (_pauseTimestamp > PauseDuration)
            {
                if (PausingPlayer == PhotonNetwork.LocalPlayer)
                {
                    SendUnpause(UnpauseReason.TIMEOUT);
                }
            }
        }
    }

    public void PauseLocal()
    {
        IsPaused = true;
    }
    public void UnpauseLocal()
    {
        IsPaused = false;
    }

    public void SendPause()
    {
        Debug.Log($"My pauses: {MyPauses}, can send: {MyPauses < MaxPauses}");
        if (MyPauses < MaxPauses)
        {
            GamePunEventSender.SendPause(PhotonNetwork.LocalPlayer.ActorNumber);
            OnPauseReceived(PhotonNetwork.LocalPlayer.ActorNumber);
        }
        else
        {
            // Spawn notification
        }
    }

    public void UnpauseManual()
    {
        SendUnpause(UnpauseReason.MANUAL);
    }

    public void SendUnpause(UnpauseReason reason)
    {
        GamePunEventSender.SendUnpause(PhotonNetwork.LocalPlayer.ActorNumber, (int)reason);
        OnUnpauseReceived(PhotonNetwork.LocalPlayer.ActorNumber, reason);
    }

    public void OnPauseReceived(int userid)
    {
        Debug.Log($"Pause received by {userid}");
        bool isMe = userid == PhotonNetwork.LocalPlayer.ActorNumber;
        if (isMe)
        {
            MyPauses++;
            PausingPlayer = PhotonNetwork.LocalPlayer;
        }
        else
        {
            PausingPlayer = PhotonNetwork.CurrentRoom.GetPlayer(userid);
            OtherPauses++;
        }
        IsPaused = true;
        Debug.Log($"Invoking game pause event");
        onGamePause?.Invoke(userid, isMe);
    }

    public void OnUnpauseReceived(int userid, UnpauseReason reason)
    {
        bool isMe = userid == PhotonNetwork.LocalPlayer.ActorNumber;
        PausingPlayer = null;
        IsPaused = false;
        _pauseTimestamp = 0f;
        onGameUnpause?.Invoke(userid, isMe, reason);
    }
}
