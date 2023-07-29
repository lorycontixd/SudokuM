using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistoryManager : MonoBehaviour
{

    #region Singleton
    private static HistoryManager _instance;
    public static HistoryManager Instance { get { return _instance; } }

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

    public List<Move> moves = new List<Move>();
    public int UnseenMoves = 0;
    public Action<Move> onMoveReceived;
    public Action onHistoryClear;

    public bool IsSetup { get { return _isSetup; } }
    private bool _isSetup = false;


    private void Start()
    {
        _isSetup = true;
    }

    public void AddMove(Move move)
    {
        moves.Add(move);
        UnseenMoves++;
        onMoveReceived?.Invoke(move);
    }
    public void Clear()
    {
        moves.Clear();
        onHistoryClear?.Invoke();
    }
    public void SeenMoves()
    {
        UnseenMoves = 0;
    }
}
