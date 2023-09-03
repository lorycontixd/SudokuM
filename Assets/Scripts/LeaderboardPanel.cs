using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardPanel : MonoBehaviour
{
    [SerializeField] private Transform parentPanel;
    [SerializeField] private GameObject recordPrefab;
    [SerializeField] private Transform recordHolder;

    private TimeraceLeaderboard leaderboard;
    private List<GameObject> clones = new List<GameObject>();

    private void Start()
    {
        DatabaseManager.Instance.onQuery.AddListener(OnDbQuery);
    }

    private void OnDbQuery(DatabaseQueryResultType resultType, QueryData data)
    {
        if (data.QueryType == QueryType.GETTIMERACELEADERBOARD)
        {
            if (resultType == DatabaseQueryResultType.SUCCESS)
            {
                var ldata = (GetTimeraceLeaderboardQuery)data;
                this.leaderboard = ldata.leaderboard;
                UpdateUI();
            }
        }
        if (data.QueryType == QueryType.GETUSER)
        {
            // Request users
        }
    }

    public void Open()
    {
        parentPanel.gameObject.SetActive(true);
        gameObject.SetActive(true);
        DatabaseManager.Instance.GetTimeraceLeaderboard();
    }
    public void Close()
    {
        parentPanel.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void UpdateUI()
    {
        ClearItems();
        SpawnItems();
    }
    private void ClearItems()
    {
        for(int i=0;i<recordHolder.childCount; i++)
        {
            Destroy(recordHolder.GetChild(i).gameObject);
        }
        clones.Clear();
    }
    private void SpawnItems()
    {
        for(int i=0;i<leaderboard.records.Count;i++)
        {
            GameObject clone = Instantiate(recordPrefab, recordHolder);
            clones.Add(clone);
            LeaderboardPanelItem item = clone.GetComponent<LeaderboardPanelItem>();
            item.SetRecord(leaderboard.records[i]);
            item.UpdateUI();
        }
    }
}