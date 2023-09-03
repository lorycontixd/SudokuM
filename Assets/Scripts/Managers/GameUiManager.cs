using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUiManager : MonoBehaviour
{

    #region Singleton
    private static GameUiManager _instance;
    public static GameUiManager Instance { get { return _instance; } }

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

    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject halfBoardPanel;
    [Header("Debug Panel UI")]
    [SerializeField] private Button punDebugPanelButton;
    [SerializeField] private GameObject punDebugPanel;
    [SerializeField] private TextMeshProUGUI connectedText;
    [SerializeField] private TextMeshProUGUI roomText;
    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private TextMeshProUGUI serverText;
    [Header("Disconnect Panel")]
    [SerializeField] private DisconnectPanel disconnectPanel;

    private bool IsDebugPanelOpen = false;
    private bool IsDisconnectPanelOpen = false;


    private void Start()
    {
        NetworkManager.Instance.onOtherPlayerGameDisconnected += OnOtherPlayerDisconnected;
        //NetworkManager.Instance.onOtherPlayerGameReconnected += OnOtherPlayerReconnected;
        NetworkManager.Instance.onGameDisconnected += OnDisconnected;
        //NetworkManager.Instance.onGameReconnected += OnReconnected
        NetworkManager.Instance.onResumeGameAfterReconnect += OnResumeAfterDc;

        gamePanel.SetActive(true);
        halfBoardPanel.SetActive(true);
        punDebugPanelButton.gameObject.SetActive(DebugManager.Instance.ShowDebugButton) ;
        punDebugPanel.SetActive(false);
        disconnectPanel.gameObject.SetActive(false);
    }

    private void OnResumeAfterDc()
    {
        disconnectPanel.Close();
    }

    private void Update()
    {
        if (IsDebugPanelOpen)
        {
            connectedText.text = $"Connected: {PhotonNetwork.IsConnected}";
            string roomval = PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.Name : "Not in room";
            roomText.text = $"Room: {roomval}";
            stateText.text = $"State: {PhotonNetwork.NetworkClientState}";
            serverText.text = $"Server: {PhotonNetwork.ServerAddress}";
        }
    }


    /// <summary>
    /// 
    /// </summary>
    private void OnReconnected()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnDisconnected()
    {
        Debug.Log($"[GAMEUI] i disconnected");
        disconnectPanel.Open();
        disconnectPanel.UpdateUI();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    private void OnOtherPlayerReconnected(Player player)
    {
        Debug.Log($"[GAMEUI] other player reconnect: {player.NickName} ({player.ActorNumber})");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    private void OnOtherPlayerDisconnected(Player player)
    {
        Debug.Log($"[GAMEUI] other player disconnected: {player.NickName} ({player.ActorNumber})");
        disconnectPanel.gameObject.SetActive(true);
        disconnectPanel.Open();
        disconnectPanel.UpdateUI();
    }


    public void TogglePunDebugPanelIngame()
    {
        Debug.Log($"Setting pun debug panel => open: {!IsDebugPanelOpen}");
        punDebugPanel.SetActive(!IsDebugPanelOpen);
        IsDebugPanelOpen = !IsDebugPanelOpen;
    }
    public void ClosePunDebugPanel()
    {
        punDebugPanel.SetActive(false);
        IsDebugPanelOpen = false;
    }
}
