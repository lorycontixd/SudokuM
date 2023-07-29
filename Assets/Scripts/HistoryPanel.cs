using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryPanel : MonoBehaviour
{
    [SerializeField] private GameObject historyListItemPrefab;

    [Header("UI")]
    [SerializeField] private GameObject historyPanelMenu;
    [SerializeField] private Transform historyListItemHolder = null;

    public bool IsPanelOpen { get { return _isPanelOpen; } }
    private bool _isPanelOpen = false;


    private void Start()
    {
        if (HistoryManager.Instance != null)
        {
            HistoryManager.Instance.onMoveReceived += OnMoveReceived;
            HistoryManager.Instance.onHistoryClear += OnHistoryClear;
        }
        else
        {
            Debug.LogWarning($"[HistoryPanel] No History Manager was detected in the scene", this);
        }
        Close();
    }


    public void Open()
    {
        historyPanelMenu.SetActive(true);
        _isPanelOpen = true;
        UpdateHistoryList();
    }
    public void Close()
    {
        historyPanelMenu.SetActive(false);
        _isPanelOpen  = false;
    }
    public void ToggleOpen()
    {
        historyPanelMenu.SetActive(!_isPanelOpen);
        _isPanelOpen = !_isPanelOpen;
        if (_isPanelOpen)
        {
            UpdateHistoryList();
        }
    }

    private void OnMoveReceived(Move move)
    {
        UpdateHistoryList();
    }
    private void OnHistoryClear()
    {
        ClearHistoryList();
    }

    public void UpdateHistoryList()
    {
        if (historyListItemPrefab == null)
        {
            return;
        }
        ClearHistoryList();
        for (int i = 0; i<HistoryManager.Instance.moves.Count; i++)
        {
            Move move = HistoryManager.Instance.moves[i];
            GameObject clone = Instantiate(historyListItemPrefab, historyListItemHolder);
            HistoryListItem item = clone.GetComponent<HistoryListItem>();  
            item.SetMove(move);
            item.UpdateUI();
        }
    }

    public void ClearHistoryList()
    {
        for (int i=0; i<historyListItemHolder.transform.childCount; i++)
        {
            Destroy(historyListItemHolder.GetChild(i).gameObject);
        }
    }
}
