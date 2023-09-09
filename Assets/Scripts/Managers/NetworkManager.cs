using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    #region Singleton
    private static NetworkManager _instance;
    public static NetworkManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    #endregion

    #region SceneType
    public enum SceneType
    {
        LOBBY,
        GAME
    }
    #endregion

    #region ReconnectionSendMode
    public enum ReconnectionSendMode
    {
        PUNEVENT,
        PUNCALLBACK
    }
    #endregion

    #region ReconnectionCheckStatus
    public enum ReconnectionCheckStatus
    {
        SUCCESS = 0,
        NOTCONNECTED = 1,
        NOTINROOM = 2,
        UNKNOWN = 3
    }
    #endregion


    [Header("Ping")]
    public int WarningPingValue = 70;
    public int CriticalPingvalue = 130;

    [Header("Timeout durations")]
    public int TimeoutLogin = 20;
    public int TimeoutRoomCreation = 15;
    public int TimeoutMoveSend = 20;
    public int BruteForceMaxReconnectAttempts = 6;
    public int LightMaxReconnectSeconds = 25; // How much time a player has to reconnect to a game
    public int LightMaxReconnectAttemps = 3;
    public int LightReconnectCooldownSeconds = 5;

    public bool IsConnectedToServer { get => PhotonNetwork.IsConnected; }
    public bool IsConnectedToGame { get; set; }
    public Player DisconnectedPlayer { get; private set; } = null;
    public int PlayerRequestingCheck { get; set; } = -1;
    public SceneType sceneType { get; private set; } = SceneType.LOBBY;
    public bool ImLeaving { get; internal set; }


    //// == Private vars
    // I disconnected
    private int _reconnectAttempts = 0;
    private float _reconnectTimestamp = 0;
    private bool _isReconnecting = false;
    private float _lightReconnectCooldownTimestamp;
    private int _lightReconnectAttempts = 0;
    private DisconnectCause _currentDisconnectCause;
    private int leaverPlayerActorN = -1;

    // Other player disconnected
    private float _playerDisconnectTimestamp = 0f;

    //// == Events
    public Action<Player> onOtherPlayerGameDisconnected;
    //public Action<Player> onOtherPlayerGameReconnected;
    public Action onGameDisconnected;
    //public Action onGameReconnected;
    public Action onResumeGameAfterReconnect;


    private void Start()
    {
        _isReconnecting = false;
        IsConnectedToGame = false;
        leaverPlayerActorN = -1;

        PhotonNetwork.GameVersion = VersionManager.Instance.GetVersionString();
        PhotonNetwork.MaxResendsBeforeDisconnect = 3;
        ResetMyDisconnectVars();
    }
    private void Update()
    {
        if (_isReconnecting)
        {
            Debug.Log($"[NetMng] Reconnecting: {PhotonNetwork.IsConnected}, {PhotonNetwork.LocalPlayer.IsInactive}, {PhotonNetwork.InRoom}");
            // If it can reconnect, try to reconnect
            if (CanTryToReconnect())
            {
                if (_lightReconnectCooldownTimestamp < Time.time)
                {
                    this.Recover();
                    _lightReconnectCooldownTimestamp = Time.time + LightReconnectCooldownSeconds;
                    _lightReconnectAttempts++;
                    if (_lightReconnectAttempts >= LightMaxReconnectAttemps)
                    {
                        ConfirmMyDisconnect();
                        ResetMyDisconnectVars();
                    }
                }
            }
            // Independently of reconnect success, don't make other player wait
            _reconnectTimestamp += Time.deltaTime;
            if (_reconnectTimestamp > LightMaxReconnectSeconds)
            {
                ConfirmMyDisconnect();
                ResetMyDisconnectVars();
            }
        }

        // give player X seconds to reconnect, up to Y times.. If timeout or max times, i win
        if (DisconnectedPlayer != null)
        {
            _playerDisconnectTimestamp += Time.deltaTime;
            if (_playerDisconnectTimestamp > LightMaxReconnectSeconds)
            {
                ConfirmOtherDisconnect();
            }
        }

        // This code is replaced by receiving reconnection event
        /*if (DisconnectedPlayer != null)
        {
            Debug.Log($"!!!! [NetMng] Disconnected player not null ===> IsInactive: {DisconnectedPlayer.IsInactive}, HasRejoined: {DisconnectedPlayer.HasRejoined},");
            
            if (!DisconnectedPlayer.IsInactive)
            {
                if (!DisconnectedPlayer.HasRejoined)
                {
                    Debug.Log($"[NetMng] Disconn. player left for sure!");

                    ConfirmOtherDisconnect();
                }
                else
                {
                    // Player should have reconnected
                    Debug.Log($"Player should have reconnected!");
                    GamePunEventSender.SendReconnectionCheck(PhotonNetwork.LocalPlayer, DisconnectedPlayer);
                    //OnOtherPlayerReconnected(DisconnectedPlayer);
                }
                ResetOtherDisconnectVars();
            }
        }*/
    }

    public void SwitchScene(SceneType scene)
    {
        sceneType = scene;
    }

    private void ResetMyDisconnectVars()
    {
        _reconnectAttempts = 0;
        _reconnectTimestamp = 0;
        _isReconnecting = false;
        _lightReconnectCooldownTimestamp = 0f;
        _lightReconnectAttempts = 0;
    }
    private void ResetOtherDisconnectVars()
    {
        DisconnectedPlayer = null;
        _playerDisconnectTimestamp = 0f;
    }

    public void SetLeaver(int actorNumber)
    {
        this.leaverPlayerActorN = actorNumber;
    }

    public void ConnectToEU()
    {
        AppSettings euSettings = new AppSettings();
        euSettings.UseNameServer = true;
        euSettings.FixedRegion = "eu";
        euSettings.AppIdRealtime = "8420cc2c-c728-4c55-aae1-7ef584128ec3"; // TODO: replace with your own PUN AppId unlocked for China region
        PhotonNetwork.ConnectUsingSettings(euSettings);
    }
    public void ConnectToRegion(string region)
    {
    }
    public void ConnectAutomatic()
    {
    }

    public void Disconnect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    /// <summary>
    /// If the player has the conditions to reconnect to the game. This means
    /// 1) Has an internet connection
    /// 2) Wants to reconnect (isReconnecting)
    /// 3) The disconnection cause allows for reconnection
    /// 
    /// This method works together with part of the code in the Update method and does not try to reconnect with brute force.
    /// Rather, it waits for all conditions to be true (mainly having internet connection) and tries to reconnect.
    /// </summary>
    /// <returns></returns>
    private bool CanTryToReconnect()
    {
        bool isReconnecting = _isReconnecting;
        bool hasInternet = Application.internetReachability != NetworkReachability.NotReachable;
        bool isReconnectableCause = CanRecoverFromDisconnect(_currentDisconnectCause);
        Debug.Log($"[NetMng] CanTryReconnect => isReconn. {isReconnecting}, hasInt. {hasInternet}, isReconn.Cause: {isReconnectableCause}");
        return isReconnecting && isReconnectableCause && hasInternet;
    }

    /// <summary>
    /// Photon's recoverable disconnection causes.
    /// </summary>
    /// <param name="cause"></param>
    /// <returns></returns>
    private bool CanRecoverFromDisconnect(DisconnectCause cause)
    {
        switch (cause)
        {
            // the list here may be non exhaustive and is subject to review
            case DisconnectCause.Exception:
            case DisconnectCause.ServerTimeout:
            case DisconnectCause.ClientTimeout:
            case DisconnectCause.DisconnectByServerLogic:
            case DisconnectCause.DisconnectByServerReasonUnknown:
                return true;
        }
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    private void Recover()
    {
        if (!PhotonNetwork.ReconnectAndRejoin())
        {
            Debug.LogError("[NetMng] ReconnectAndRejoin failed, trying Reconnect");
            if (!PhotonNetwork.Reconnect())
            {
                /*
                Debug.LogError("[NetMng] Reconnect failed, trying ConnectUsingSettings");
                if (!PhotonNetwork.ConnectUsingSettings())
                {
                    Debug.LogError("[NetMng] ConnectUsingSettings failed");
                }
                */
            }
        }
        else
        {
            Debug.Log($"[NetMng] Successful reconnectAndRejoin");
            _reconnectAttempts++;
            if(_reconnectAttempts > BruteForceMaxReconnectAttempts)
            {
                ConfirmMyDisconnect();
                ResetMyDisconnectVars();
            }
            // Successful
        }
    }

    private void ConfirmMyDisconnect()
    {
        Debug.Log($"[NetMng] Confirming disconnect!");
        SessionManager.Instance.SetGameInstance(null);
        SessionManager.Instance.SetOtherUser(null);
        SessionManager.Instance.SetHostUser(null);
        SessionManager.Instance.SetLoginData(null);
        PhotonNetwork.LoadLevel("Lobby");
    }

    private void ConfirmOtherDisconnect()
    {
        // I win
        GameManager.Instance.Finish(true);
    }


    public void OnOtherPlayerReconnected(Player reconnectedPlayer)
    {
        Debug.Log($"[NetMng] IsDisconnecting player null? {DisconnectedPlayer == null}");
        Debug.Log($"[NetMng] Received reconnection event ==> player: {reconnectedPlayer}, EqDcPlayer: {DisconnectedPlayer.Equals(reconnectedPlayer)}");
        if (DisconnectedPlayer != null)
        {
            if (DisconnectedPlayer.Equals(reconnectedPlayer))
            {
                Debug.Log($"Reconnecting other player");
                Managers.NotificationManager.Instance.Info($"{DisconnectedPlayer.NickName} reconnected", "Game resuming");
                ResetOtherDisconnectVars();
            }
        }
    }

    public void ResumeGameAfterDisconnect(float gameTime)
    {
        if (DisconnectedPlayer != null)
        {
            OnOtherPlayerReconnected(DisconnectedPlayer);
        }
        SudokuCanvas.Instance.SetGametime(gameTime);
        onResumeGameAfterReconnect?.Invoke();
        PauseManager.Instance.UnpauseLocal();
    }

    #region Pun Callbacks


    public override void OnConnectedToMaster()
    {
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (!_isReconnecting && sceneType == SceneType.GAME)
        {
            Debug.Log($"[NetMng] Dced -> Imleaving: {ImLeaving}");
            if (ImLeaving)
            {
                ImLeaving = false;
                PhotonNetwork.LoadLevel("Lobby");
            }
            else
            {
                IsConnectedToGame = false;
                _isReconnecting = true;
                _currentDisconnectCause = cause;
                Managers.NotificationManager.Instance.Info("Connection lost", "Trying to reconnect");
                onGameDisconnected?.Invoke();
                PauseManager.Instance.PauseLocal();
            }
            
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (newPlayer == DisconnectedPlayer && sceneType == SceneType.GAME)
        {
            Debug.Log("Player rejoined!!");
        }
    }

    /// <summary>
    /// I reconnected to the game.
    /// The other player retrieves my reconnecting status in update with player->HasRejoined.
    /// I just have to make sure we're on the same page then we can restart
    /// </summary>
    public override void OnJoinedRoom()
    {
        if (_isReconnecting && sceneType == SceneType.GAME)
        {
            Debug.Log($"[NetMng] Joined room!!");
            IsConnectedToGame = true;
            _isReconnecting = false;
            GamePunEventSender.SendReconnect(PhotonNetwork.LocalPlayer, SudokuCanvas.Instance.GameTime);
            ResetMyDisconnectVars();
        }
    }

    /// <summary>
    /// Other player disconnected from the game
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (!otherPlayer.Equals(PhotonNetwork.LocalPlayer) && sceneType == SceneType.GAME)
        {
            DisconnectedPlayer = otherPlayer;
            if (leaverPlayerActorN == otherPlayer.ActorNumber)
            {
                GameManager.Instance.Finish(true);
            }
            else
            {
                Managers.NotificationManager.Instance.Info($"{otherPlayer.NickName} lost connection", "Player is trying to reconnect");
                PauseManager.Instance.PauseLocal();
                onOtherPlayerGameDisconnected?.Invoke(DisconnectedPlayer);
            }
        }
    }

    public override void OnLeftRoom()
    {
        if (NetworkManager.Instance.ImLeaving && sceneType == SceneType.GAME)
        {
            SessionManager.Instance.SetGameInstance(null);
            SessionManager.Instance.SetOtherUser(null);
            PhotonNetwork.LoadLevel("Lobby");
        }
    }
    #endregion
}
