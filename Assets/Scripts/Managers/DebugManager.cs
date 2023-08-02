using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    #region Singleton
    private static DebugManager _instance;
    public static DebugManager Instance { get { return _instance; } }

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

    public bool DebugGame;
    public bool DebugText;

    [Header("Board Fill")]
    public bool CanvasFillForMasterClient;
    public int CanvasFillAmount;
    [Tooltip("If count, the amount does not considered the default cells")] public SudokuCanvas.AutoFillType CanvasFillType;
}
