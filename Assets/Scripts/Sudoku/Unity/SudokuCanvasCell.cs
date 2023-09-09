using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SudokuCanvasCell : MonoBehaviour
{
    public enum CellState
    {
        DEFAULT = 0,
        PASSIVESELECTED = 1,
        SAMENUMBERSELECTED = 2,
        SELECTED = 3
    }

    public int ID;
    public int Value = 0;
    public bool IsLocked { get; private set; } = false;
    public int Row { get; private set; }
    public int Col { get; private set; }
    public int UserIdEdited = -1;
    public bool LocalPlayerEdited;
    public CellState State { get; private set; }

    [Header("UI")]
    private BoardCell boardCell = null;
    private Button button;
    private TextMeshProUGUI cellText;
    [SerializeField] private Image borderImage;
    [SerializeField] private Image cellBackground = null;

    public UnityEvent<SudokuCanvasCell> onCellClick;
    private float defaultFontSize;

    public static Dictionary<string, Color> CellTextColor = new Dictionary<string, Color>()
    {
        { "default" , new Color(1,1,1,1) },
        { "user" , new Color(0.31f, 0.31f, 0.86f, 1f) }
    };
    public static Dictionary<CellState, Color> CellBackgroundColor = new Dictionary<CellState, Color>()
    {
        { CellState.DEFAULT, new Color(1f, 1f, 1f, 0f) },
        { CellState.PASSIVESELECTED, new Color(0.43f, 0.7f, 1f, 0.21f) },
        { CellState.SAMENUMBERSELECTED, new Color(0.39f, 0.47f, 1f, 0.44f)},
        { CellState.SELECTED, new Color(0f, 0.78f, 1f, 0.55f)}
    };


    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        cellText = GetComponentInChildren<TextMeshProUGUI>();
        if ( borderImage == null)
        {
            borderImage = GetComponentInChildren<Image>();
        }
        if (cellBackground == null)
        {
            cellBackground = GetComponent<Image>();
        }
        defaultFontSize = cellText.fontSize;
    }

    public void OnClick()
    {
        onCellClick?.Invoke(this);
    }

    public void UpdateText()
    {
        cellText.text = (Value <= 9 && Value >= 1) ? Value.ToString() : string.Empty;
    }

    public void SetNewValue(int value)
    {
        Value = value;
        UpdateText();
    }
    public void SetNewValue(int value, bool isCorrect)
    {
        Value = value;
        UpdateText();
        Color userColor = (IsLocked) ? CellTextColor["default"] : CellTextColor["user"];
        Debug.Log($"User color: {userColor}");
        cellText.color = (isCorrect) ? userColor : Color.red;
        cellText.faceColor = (isCorrect) ? userColor : Color.red;
    }
    public void SetNewValue(int UserID, int value, bool isCorrect)
    {
        Value = value;
        this.UserIdEdited = UserID;
        UpdateText();
        LocalPlayerEdited = PhotonNetwork.LocalPlayer.ActorNumber == UserID;
        Color playerColour = LocalPlayerEdited ? GameManager.Instance.localPlayerCellColourCoop : GameManager.Instance.otherPlayerCellColourCoop;
        cellText.color = (isCorrect) ? playerColour : Color.red;
        cellText.faceColor = (isCorrect) ? playerColour : Color.red;
        if (isCorrect) { SetLocked(true); }
    }
    public void DeleteValue()
    {
        Value = 0;
        this.UserIdEdited = -1;
        UpdateText();
        LocalPlayerEdited = false;
        cellText.color = new Color();
        cellText.faceColor = new Color();
        SetLocked(false);
    }

    public void SetLocked(bool locked)
    {
        IsLocked = locked;
    }

    public void SetRowCol(int row, int col)
    {
        Row = row;
        Col = col;
    }

    public (int, int) GetRowColValue(int colSize = 9)
    {
        return (Row, Col);
    }

    public void SetLastEdit(bool lastEdit)
    {
        if (lastEdit)
        {
            Debug.Log($"Setlastedit => true");
            borderImage.color = LocalPlayerEdited ? GameManager.Instance.localPlayerCellColourCoop : GameManager.Instance.otherPlayerCellColourCoop;
            borderImage.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log($"Setlastedit => false");
            borderImage.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Reset the cell's UI to the default value
    /// </summary>
    public void ResetUI()
    {
        cellBackground.color = CellBackgroundColor[CellState.DEFAULT];
        Unhighlight();
    }

    /// <summary>
    /// Set the ui of the cell as passive selected, when a cell in the same square or row or column has been selected.
    /// </summary>
    public void SetPassiveUI()
    {
        cellBackground.color = CellBackgroundColor[CellState.PASSIVESELECTED];
        Unhighlight();
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetSameValueButtonSelectedUI()
    {
        cellBackground.color = CellBackgroundColor[CellState.SAMENUMBERSELECTED];
        Highlight();
    }

    /// <summary>
    /// Set the ui of the cell as selected => this cell has been selected.
    /// </summary>
    public void SetSelectedUI()
    {
        cellBackground.color = CellBackgroundColor[CellState.SELECTED];
        Highlight();
    }





    public void Highlight()
    {
        cellText.fontStyle = FontStyles.Bold;
        cellText.fontSize = defaultFontSize + 8f;
    }
    public void Unhighlight()
    {
        cellText.fontStyle = FontStyles.Normal;
        cellText.fontSize = defaultFontSize;
    }
}
