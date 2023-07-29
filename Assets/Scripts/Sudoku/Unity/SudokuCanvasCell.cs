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
    public int ID;
    public int Value = 0;
    public bool IsLocked = false;
    public int Row { get; private set; }
    public int Col { get; private set; }
    public int UserIdEdited;
    public bool LocalPlayerEdited;

    [Header("UI")]
    private BoardCell boardCell = null;
    private Button button;
    private TextMeshProUGUI cellText;
    [SerializeField] private Image borderImage;

    public UnityEvent<SudokuCanvasCell> onCellClick;
    private float defaultFontSize;


    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        cellText = GetComponentInChildren<TextMeshProUGUI>();
        if ( borderImage == null)
        {
            borderImage = GetComponentInChildren<Image>();
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
        cellText.color = (isCorrect) ? Color.black : Color.red;
        cellText.faceColor = (isCorrect) ? Color.black : Color.red;
    }
    public void SetNewValue(int UserID, int value, bool isCorrect)
    {
        Value = value;
        this.UserIdEdited = UserID;
        UpdateText();
        LocalPlayerEdited = PhotonNetwork.LocalPlayer.ActorNumber == UserID;
        Color playerColour = LocalPlayerEdited ? GameManager.Instance.localPlayerCellColour : GameManager.Instance.otherPlayerCellColour;
        cellText.color = (isCorrect) ? playerColour : Color.red;
        cellText.faceColor = (isCorrect) ? playerColour : Color.red;
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
            borderImage.color = LocalPlayerEdited ? GameManager.Instance.localPlayerCellColour : GameManager.Instance.otherPlayerCellColour;
            borderImage.gameObject.SetActive(true);
        }
        else
        {

            borderImage.gameObject.SetActive(false);
        }
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
