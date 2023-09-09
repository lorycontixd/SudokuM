using DG.Tweening;
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
    [SerializeField] private TextMeshProUGUI pingText;
    [Header("Networking UI")]
    [SerializeField] private DisconnectPanel disconnectPanel;
    [SerializeField] private Image lowWifiIcon;
    [SerializeField] private AudioClip lowWifiSoundClip;
    [SerializeField] private float lowWifiFadeDuration = 0.6f;

    private bool IsDebugPanelOpen = false;
    private bool IsDisconnectPanelOpen = false;
    private int ping;


    private void Start()
    {
        NetworkManager.Instance.onOtherPlayerGameDisconnected += OnOtherPlayerDisconnected;
        //NetworkManager.Instance.onOtherPlayerGameReconnected += OnOtherPlayerReconnected;
        NetworkManager.Instance.onGameDisconnected += OnDisconnected;
        //NetworkManager.Instance.onGameReconnected += OnReconnected
        NetworkManager.Instance.onResumeGameAfterReconnect += OnResumeAfterDc;
        GameManager.Instance.onGameFinish += OnGameFinish;

        gamePanel.SetActive(true);
        halfBoardPanel.SetActive(true);
        punDebugPanelButton.gameObject.SetActive(DebugManager.Instance.ShowDebugButton) ;
        punDebugPanel.SetActive(false);
        lowWifiIcon.gameObject.SetActive(false);
        lowWifiIcon.transform.localScale = Vector3.zero;
        disconnectPanel.Close();
    }

    private void OnGameFinish(bool arg1, bool arg2, int arg3, int arg4)
    {
        disconnectPanel.Close();
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
            pingText.text = $"Ping: {ping}";
        }

        ping = PhotonNetwork.GetPing();
        if (ping >= NetworkManager.Instance.WarningPingValue && !lowWifiIcon.gameObject.activeSelf)
        {
            lowWifiIcon.gameObject.SetActive(true);
            lowWifiIcon.transform.DOScale(Vector3.one, lowWifiFadeDuration);
            //StartCoroutine(StartWifiLogoShakeCo(2f));
            if (AudioManager.Instance != null)
            {
                //AudioManager.Instance.PlayNotification
            }
        }
        if (ping < NetworkManager.Instance.WarningPingValue && lowWifiIcon.gameObject.activeSelf)
        {
            lowWifiIcon.gameObject.SetActive(false);
            lowWifiIcon.transform.DOScale(Vector3.zero, lowWifiFadeDuration);
        }
        if (lowWifiIcon.gameObject.activeSelf)
        {
        }
    }

    /*private IEnumerator StartWifiLogoShakeCo(float duration = 2f)
    {
        while ( lowWifiIcon.gameObject.activeSelf )
        {
            lowWifiIcon.transform.DOSpiral(duration);
            yield return new WaitForSeconds(duration);
        }
    }*/

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
