using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Move
{
    public int UserID;
    public string Username;
    public int CellRow;
    public int CellColumn;
    public int Digit;
    public bool IsCorrect;
    public bool IsDeleteMove { get { return Digit == -1; } }
    public DateTime DateTime;

    public Move()
    {
        UserID = 0;
        Username = string.Empty;
        CellRow = 0;
        CellColumn = 0;
        DateTime = DateTime.MinValue;
        Digit = 0;
    }

    public Move(int userID, string username, int cellRow, int cellColumn, int digit, bool isCorrect, DateTime dateTime)
    {
        UserID = userID;
        Username = username;
        CellRow = cellRow;
        CellColumn = cellColumn;
        Digit = digit;
        IsCorrect = isCorrect;
        DateTime = dateTime;
    }

    public Move(int userID, string username, int cellRow, int cellColumn, int digit, bool isCorrect) : this(userID, username, cellRow, cellColumn, digit, isCorrect, DateTime.Now)
    {
    }

    public (int, int) GetRowCol()
    {
        return (CellRow, CellColumn);
    }
}
