using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Michsky.MUIP;
using System;
using Photon.Pun;

public class StatsPanel : MonoBehaviour
{
    [SerializeField] private GameObject playerStatsPanelPrefab;
    [SerializeField] private GameObject statsPanelMenu;
    private List<PlayerStatsPanel> playerPanels = new List<PlayerStatsPanel>(); 


    public bool IsPanelOpen { get { return _isPanelOpen; } }
    private bool _isPanelOpen = false;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => HistoryManager.Instance.IsSetup);
        yield return new WaitUntil(() => StatsManagerCoop.Instance.IsSetup);
        if (HistoryManager.Instance != null)
        {
            HistoryManager.Instance.onMoveReceived += OnMoveReceived;
            HistoryManager.Instance.onHistoryClear += OnHistoryClear;
        }
        Debug.Log($"Setting up stats panel");
        SetupStatsPanel();
        UpdateStats();
        Close();
    }

    private void OnHistoryClear()
    {
    }

    private void OnMoveReceived(Move move)
    {
        UpdateStats();
    }

    public void Open()
    {
        statsPanelMenu.SetActive(true);
        _isPanelOpen = true;
        UpdateStats();
    }
    public void Close()
    {
        statsPanelMenu.SetActive(false);
        _isPanelOpen = false;
    }
    public void ToggleOpen()
    {
        statsPanelMenu.SetActive(!_isPanelOpen);
        _isPanelOpen = !_isPanelOpen;
        if (_isPanelOpen)
        {
            UpdateStats();
        }
    }

    public void SetupStatsPanel()
    {
        foreach (var kvp in PhotonNetwork.CurrentRoom.Players)
        {
            GameObject clone = Instantiate(playerStatsPanelPrefab, statsPanelMenu.transform);
            PlayerStatsPanel panel = clone.GetComponent<PlayerStatsPanel>();
            panel.SetIdentity(kvp.Value);
            playerPanels.Add(panel);
        }
    }
    public void UpdateStats()
    {
        foreach(var pp in playerPanels)
        {
            pp.UpdateStats();
        }
    }
}
