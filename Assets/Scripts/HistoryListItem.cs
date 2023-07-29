using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HistoryListItem : MonoBehaviour
{
    private Move move;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI playerText;
    [SerializeField] private TextMeshProUGUI cellText;
    [SerializeField] private TextMeshProUGUI digitText;
    [SerializeField] private Image correctImage;
    [SerializeField] private Image wrongImage;


    public void SetMove(Move move)
    {
        this.move = move;
    }

    public void UpdateUI()
    {
        playerText.text = move.Username;
        cellText.text = $"({move.CellRow+1},{move.CellColumn+1})";
        digitText.text = move.Digit.ToString();
        correctImage.gameObject.SetActive(move.IsCorrect);
        wrongImage.gameObject.SetActive(!move.IsCorrect);
    }
}
