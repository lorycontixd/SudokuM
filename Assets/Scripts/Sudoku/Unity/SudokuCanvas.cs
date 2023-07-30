using Michsky.MUIP;
using NUnit.Framework.Constraints;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class SudokuCanvas : MonoBehaviour
{
    #region Singleton
    private static SudokuCanvas _instance;
    public static SudokuCanvas Instance { get { return _instance; } }

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

    public static int MaxErrors = 3;
    //public int activeNumber = -1;
    public SudokuCanvasCell currentSelectedCell = null;
    public int MyCompletedCells { get => cells.Where(c => c.Value >= 1 && c.Value <= 9).Count(); }  

    [SerializeField] private GameObject gridParent = null;
    public int currentDigit;
    public List<SudokuCanvasCell> cells;
    public List<SudokuNumberButton> valueButtons = new List<SudokuNumberButton>();


    [Header("UI")]
    [SerializeField] private StatsPanel statsPanel;
    [SerializeField] private HistoryPanel historyPanel;
    [SerializeField] private GameObject finalPanel;
    [SerializeField] private TextMeshProUGUI finalText;
    [SerializeField] private TextMeshProUGUI finalWaitingForHostText;
    [SerializeField] private ButtonManager finalNewGameButton;
    [SerializeField] private ButtonManager finalRetryGameButton;
    [SerializeField] private TextMeshProUGUI errorsText;
    [SerializeField] private TextMeshProUGUI ratingText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI boardLoadingText;

    [Header("Settings")]
    [SerializeField] private bool DebugGame;
    [SerializeField] private bool DebugText;


    private int[,] currentPuzzle;
    private SudokuSolver currentSolver = null;
    private int[,] currentSolvedBoard;
    private int currentRating = -1;
    private bool IsEmpty = true;
    private bool IsPlaying = false;
    private int currentErrors = 0;
    private DateTime puzzleStartTime = DateTime.MinValue;
    private GameObject currentPanel = null;
    private SudokuCanvasCell lastEditedCell = null;


    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            boardLoadingText.gameObject.SetActive(true);
        }
        if (gridParent == null)
        {
            Transform parentT = transform.Find("Grid");
            if (parentT != null)
            {
                gridParent = parentT.gameObject;
            }
            else
            {
                Debug.LogError($"Could not find child with name Grid");
            }
        }
        cells = gridParent.GetComponentsInChildren<SudokuCanvasCell>().ToList();
        int expected = SudokuBoard.BOARD_SIZE * SudokuBoard.BOARD_SIZE;
        Debug.Log($"cells size: {cells.Count}");
        if (cells.Count != expected)
        {
            throw new UnityException($"Expected {expected} Sudoku canvas cells, but found {cells.Count} in canvas");
        }
        valueButtons = transform.GetComponentsInChildren<SudokuNumberButton>().ToList();
        valueButtons = valueButtons.OrderBy(b => b.Value).ToList();
        for (int i=0; i<valueButtons.Count; i++)
        {
            valueButtons[i].onButtonClick += OnValueButtonClick;
            valueButtons[i].SetID(i);
        }
        if (finalPanel != null)
        {
            finalPanel.SetActive(false);
        }
        if (historyPanel!= null)
        {
            historyPanel.gameObject.SetActive(true);
            historyPanel.Close();
        }
        if (statsPanel != null)
        {
            statsPanel.gameObject.SetActive(true);
            statsPanel.Close();
        }
        ResetBoard();
    }
    private void Update()
    {
        if (IsPlaying)
        {
            UpdateTimeText();
        }
    }

    public void InitializeBoard(int[,] puzzle)
    {
        ResetBoard();
        int count = 0;
        for(int i = 0; i < puzzle.GetLength(0); i++)
        {
            for (int j = 0; j < puzzle.GetLength(1); j++)
            {
                cells[count].Value = puzzle[i, j];
                cells[count].SetRowCol(i, j);
                cells[count].onCellClick.AddListener(OnCellSelect);
                cells[count].SetLocked(puzzle[i, j] != 0);
                cells[count].ID = count+1;
                cells[count].UpdateText();
                count++;
            }
        }
        currentPuzzle = puzzle;
        currentSolver = new SudokuSolver(puzzle);
        int code = currentSolver.solve();
        currentSolvedBoard = currentSolver.getSolvedBoard();
        currentRating = SudokuStore.calculatePuzzleRating(currentPuzzle);
        UpdateRatingText(currentRating);
        UpdateErrorsText();
        puzzleStartTime = DateTime.Now;
        IsEmpty = false;
        IsPlaying = true;
        finalPanel.SetActive(false);
        boardLoadingText.gameObject.SetActive(false);
    }
    public void InitializeBoard(int[,] puzzle, int[,] solution, int rating)
    {
        ResetBoard();
        int count = 0;
        for (int i = 0; i < puzzle.GetLength(0); i++)
        {
            for (int j = 0; j < puzzle.GetLength(1); j++)
            {
                cells[count].Value = puzzle[i, j];
                cells[count].SetRowCol(i, j);
                cells[count].onCellClick.AddListener(OnCellSelect);
                cells[count].SetLocked(puzzle[i, j] != 0);
                cells[count].ID = count + 1;
                cells[count].UpdateText();
                count++;
            }
        }
        currentPuzzle = puzzle;
        currentSolver = new SudokuSolver(puzzle);
        currentSolvedBoard = solution;
        currentRating = rating;
        UpdateRatingText(currentRating);
        UpdateErrorsText();
        puzzleStartTime = DateTime.Now;
        IsEmpty = false;
        IsPlaying = true;
        finalPanel.SetActive(false);
        boardLoadingText.gameObject.SetActive(false);
        if (PhotonNetwork.IsMasterClient && DebugGame)
        {
            StartCoroutine(AutoFillGradualCo(2, 3f));
        }
        
    }

    public void ResetBoard()
    {
        if (currentPuzzle != null)
        {
            int count = 0;
            for (int i = 0; i < currentPuzzle.GetLength(0); i++)
            {
                for (int j = 0; j < currentPuzzle.GetLength(1); j++)
                {
                    cells[count].Value = -1;
                    cells[count].SetRowCol(i, j);
                    cells[count].onCellClick.RemoveAllListeners();
                    cells[count].SetLocked(currentPuzzle[i, j] != 0);
                    cells[count].ID = -1;
                    cells[count].UpdateText();
                    count++;
                }
            }
        }
        currentPuzzle = new int[,] { };
        currentSolver = null;
        currentSolvedBoard = new int[,] { };
        
        currentErrors = 0;
        IsEmpty = true;
        IsPlaying = false;

        ResetValueButtons();
        historyPanel.ClearHistoryList();
    }

    private void AutoFill(int except = 1)
    {
        List<SudokuCanvasCell> empties = cells.Where(cell => !cell.IsLocked).ToList();
        List<SudokuCanvasCell> toFill = empties.OrderBy(a => new System.Random().Next()).ToList().Take(empties.Count - except).ToList();
        foreach(SudokuCanvasCell cell in toFill)
        {
            int val = currentSolvedBoard[cell.Row, cell.Col];
            cell.SetNewValue(PhotonNetwork.LocalPlayer.ActorNumber, val, true);
        }
    }
    private void AutoFillGradual(int except = 1)
    {
        List<SudokuCanvasCell> empties = cells.Where(cell => !cell.IsLocked).ToList();
        List<SudokuCanvasCell> toFill = empties.OrderBy(a => new System.Random().Next()).ToList().Take(empties.Count - except).ToList();
        foreach (SudokuCanvasCell cell in toFill)
        {
            int val = currentSolvedBoard[cell.Row, cell.Col];
            GamePunEventSender.SendMove(PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.NickName, cell.Row, cell.Col, val, true, false, MyCompletedCells, Photon.Realtime.ReceiverGroup.All);
        }
    }
    private IEnumerator AutoFillGradualCo(int except = 1, float delay = 2f)
    {
        yield return new WaitForSeconds(delay);
        AutoFillGradual(except);
    }

    #region Buttons
    public void ButtonDelete()
    {
        GamePunEventSender.SendMove(PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.NickName, currentSelectedCell.Row, currentSelectedCell.Col, -1, false, true, MyCompletedCells);
    }
    public void ButtonHistory()
    {
        if (!statsPanel.IsPanelOpen)
        {
            historyPanel.ToggleOpen();
        }
    }
    public void ButtonStats()
    {
        if (!historyPanel.IsPanelOpen)
        {
            statsPanel.ToggleOpen();
        }
    }
    public void ButtonFinalNewGame()
    {
        GameManager.Instance.NewGame();
    }
    public void ButtonFinalRetryGame()
    {
        GameManager.Instance.RetryGame();
    }
    public void ButtonFinalQuit()
    {
        GameManager.Instance.Quit();
    }
    #endregion

    /// <summary>
    /// Callback method when a board cell is pressed/selected.
    /// </summary>
    /// <param name="cell">The cell being selected</param>
    public void OnCellSelect(SudokuCanvasCell cell)
    {
        currentSelectedCell = cell;
        OnCellSelectUIUpdate(cell);
    }

    /// <summary>
    /// When a cell is selected, update all uis accordingly
    /// </summary>
    public void OnCellSelectUIUpdate(SudokuCanvasCell selectedCell)
    {
        for (int i=0; i<cells.Count;i++)
        {
            int row = i / 9;
            int col = i % 9;
            selectedCell.SetSelectedUI();
            if ( (cells[i].Row == selectedCell.Row || cells[i].Col == selectedCell.Col) && cells[i] != selectedCell) // add same square
            {
                // it's on the same row or same column as selected cell
                cells[i].SetPassiveUI();
            }
            else
            {
                cells[i].ResetUI();
                if (selectedCell.Value != GameManager.NullCellValue) // The cell contains a value => same number passive selection
                {
                    if (cells[i].Value == selectedCell.Value)
                    {
                        cells[i].SetSameValueButtonSelectedUI();
                    }
                }
            }
        }
    }



    #region Moves
    private void ApplyMove(SudokuCanvasCell cell, int digit, bool isCorrect, bool isDeleteMove)
    {
        if (isCorrect)
        {
            cell.SetNewValue(digit, true);
            bool isComplete = IsPuzzleComplete();
            if (IsPuzzleCompleteAndCorrect())
            {
                FinishAndClose(true);
            }
        }
        else
        {
            cell.SetNewValue(digit, false);
            if (digit != -1)
            {
                currentErrors++;
                UpdateErrorsText();
                if (currentErrors == MaxErrors)
                {
                    Finish(false);
                }
            }
        }
    }
    private void DeleteMove(SudokuCanvasCell cell)
    {
        if (!cell.IsLocked)
        {
            cell.DeleteValue();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userid"></param>
    /// <param name="username"></param>
    /// <param name="cell"></param>
    /// <param name="digit"></param>
    /// <param name="isCorrect"></param>
    /// <param name="isDeleteMove"></param>
    private void ApplyMoveMultiplayer(int userid, string username, SudokuCanvasCell cell, int digit, bool isCorrect, bool isDeleteMove)
    {

        if (isCorrect)
        {
            // valid move
            cell.SetNewValue(userid, digit, true);
            CheckValueButtonsToDisable();
            if (GameManager.Instance.GameMode == GameMode.COOP)
            {
                bool isComplete = IsPuzzleComplete();
                if (IsPuzzleCompleteAndCorrect())
                {
                    FinishAndClose(true);
                }
            }else if (GameManager.Instance.GameMode == GameMode.TIMERACE)
            {
                if (userid == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    bool isComplete = IsPuzzleComplete();
                    if (IsPuzzleCompleteAndCorrect())
                    {
                        GamePunEventSender.SendFinish(userid, userid);
                    }
                }
            }
            
        }
        else
        {
            cell.SetNewValue(userid, digit, false);
            if (digit != -1)
            {
                currentErrors++;
                UpdateErrorsText();
                if (currentErrors == MaxErrors)
                {
                    Finish(false);
                }
            }
        }
        currentPuzzle[cell.Row, cell.Col] = digit;
        if (GameManager.Instance.GameMode == GameMode.COOP)
        {
            SetLastEditCell(cell.Row, cell.Col);
        }
        /*if (!isDeleteMove)
        {
            HighlightValueCells(digit);
        }*/
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cell"></param>
    private void DeleteMoveMultiplayer(SudokuCanvasCell cell)
    {
        if (!cell.IsLocked)
        {
            cell.DeleteValue();
            cell.SetLastEdit(false);
        }
        else
        {
            return;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userid"></param>
    /// <param name="username"></param>
    /// <param name="cell"></param>
    /// <param name="digit"></param>
    /// <param name="isCorrect"></param>
    /// <param name="isDeleteMove"></param>
    public void SendMoveStat(int userid, string username, SudokuCanvasCell cell, int digit, bool isCorrect, bool isDeleteMove, int completedCells)
    {
        Move move = new Move(userid, username, cell.Row, cell.Col, digit, isCorrect);
        if (HistoryManager.Instance != null)
        {
            HistoryManager.Instance.AddMove(move);
        }
        if (StatsManagerCoop.Instance != null)
        {
            if (!isDeleteMove)
            {
                StatsManagerCoop.Instance.AddMove(move);
                int filledCells = cells.Where(c => c.Value != 0).ToList().Count;
                Debug.Log($"[SendMoveStat] PLAYER {userid} filled cells: {filledCells}/{StatsManagerCoop.TOTAL_CELLS}");
                StatsManagerCoop.Instance.SetFilledTiles(userid, completedCells);
            }
        }
    }
    #endregion



    /// <summary>
    /// 
    /// </summary>
    /// <param name="userid"></param>
    /// <param name="username"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="digit"></param>
    /// <param name="isCorrect"></param>
    /// <param name="isDeleteMove"></param>
    public void UpdateCellMultiplayer(int userid, string username, int row, int col, int digit, bool isCorrect, bool isDeleteMove, int completedCells)
    {
        SudokuCanvasCell cell = cells.FirstOrDefault(c => c.Row == row && c.Col == col);
        if (!isDeleteMove)
        {
            switch (GameManager.Instance.GameMode)
            {
                case GameMode.COOP:
                    ApplyMoveMultiplayer(userid, username, cell, digit, isCorrect, isDeleteMove);
                    SendMoveStat(userid, username, cell, digit, isCorrect, isDeleteMove, completedCells);
                    break;
                case GameMode.TIMERACE:
                    if (userid == PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        ApplyMoveMultiplayer(PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.LocalPlayer.NickName, cell, digit, isCorrect, isDeleteMove);
                    }
                    SendMoveStat(userid, username, cell, digit, isCorrect, isDeleteMove, completedCells);
                    break;
            }
        }
        else
        {
            DeleteMoveMultiplayer(cell);
        }
    }

    /// <summary>
    /// Event called when a value button (buttons with number from 1 to 9) is selected.
    /// </summary>
    /// <param name="number"></param>
    public void OnValueButtonClick(SudokuNumberButton number)
    {
        if (currentSelectedCell != null)
        {
            if (currentSelectedCell.IsLocked)
            {
                Managers.NotificationManager.Instance.Info($"Invalid move", "Cannot modify a correct or default cell");
                return;
            }
            bool isCorrect = (number.Value == currentSolvedBoard[currentSelectedCell.Row, currentSelectedCell.Col]);

            switch (GameManager.Instance.GameMode)
            {
                case GameMode.TIMERACE:
                    //Debug.Log($"Updating move on mode TimeRace!");
                    //UpdateCellMultiplayer(PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.NickName, currentSelectedCell.Row, currentSelectedCell.Col, number.Value, isCorrect, false, MyCompletedCells);
                    //Debug.Log($"Sending move on mode TimeRace!");
                    GamePunEventSender.SendMove(PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.NickName, currentSelectedCell.Row, currentSelectedCell.Col, number.Value, isCorrect, false, MyCompletedCells+1, Photon.Realtime.ReceiverGroup.All);
                    break;
                case GameMode.COOP:
                    GamePunEventSender.SendMove(PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.NickName, currentSelectedCell.Row, currentSelectedCell.Col, number.Value, isCorrect, false, MyCompletedCells+1, Photon.Realtime.ReceiverGroup.All);
                    break;
                default:
                    break;
            }
            
        }
    }

    public void Finish(bool isWin)
    {
        IsPlaying = false;
        if (finalPanel != null)
        {
            finalPanel.SetActive(true);
            if(finalText != null)
            {
                finalText.gameObject.SetActive(true);
                finalText.text = isWin ? "YOU WIN!" : "You lose :(";
            }
            finalNewGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
            finalRetryGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
            finalWaitingForHostText.gameObject.SetActive(!PhotonNetwork.IsMasterClient);
        }
    }
    public void FinishAndClose(bool isWin, float delay = 5f)
    {
        StartCoroutine(FinishCo(isWin, delay));
    }
    private IEnumerator FinishCo(bool isWin, float delay)
    {
        Finish(isWin);
        yield return new WaitForSeconds(delay);
        if (finalText != null)
        {
            finalText.gameObject.SetActive(false);
        }
    }

    public bool IsPuzzleComplete()
    {
        bool isComplete = cells.All(c => c.Value > 0 && c.Value < 10);
        return isComplete;
    }

    public bool IsPuzzleCompleteAndCorrect()
    {
        bool isComplete = IsPuzzleComplete();
        int[,] usersMatrix = MatrixUtilities.ComposeMatrix(cells.Select(c => c.Value).ToArray(), SudokuBoard.BOARD_SIZE);
        bool isCorrect = SudokuStore.boardsAreEqual(usersMatrix, currentSolvedBoard);
        Debug.Log($"Boards are equal ==> {isCorrect},  iscomplete: {isComplete}");
        return isComplete && isCorrect;
    }

    public bool IsSolvedPuzzleCorrect()
    {
        return SudokuStore.boardsAreEqual(currentPuzzle, currentSolvedBoard);
    }


    private void ResetValueButtons()
    {
        foreach(SudokuNumberButton vb in valueButtons)
        {
            vb.Reset();
        }
    }

    private void HighlightValueCells(int digit)
    {
        for (int i=0; i<cells.Count; i++)
        {
            if (cells[i].Value == digit)
            {
                cells[i].Highlight();
            }
            else
            {
                cells[i].Unhighlight();
            }
        }
    }
    private void UnhighlightAllCells()
    {
        for (int i=0; i<cells.Count; i++)
        {
            cells[i].Unhighlight();
        }
    }

    private void CheckValueButtonsToDisable()
    {
        for (int i=1; i<=9; i++)
        {
            int count = cells.Where(c =>  c.Value == i).Count();
            if (count == 9)
            {
                SudokuNumberButton b = valueButtons.FirstOrDefault(b => b.Value == i);
                b.Deactivate();
            }
        }
    }

        
    public void SetLastEditCell(int row, int col)
    {
        if (lastEditedCell != null)
        {
            lastEditedCell.SetLastEdit(false);
        }
        SudokuCanvasCell cell = cells.FirstOrDefault(c => c.Row == row && c.Col == col);
        cell.SetLastEdit(true);
        lastEditedCell = cell;
    }
    public void NoLastEditCell()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].SetLastEdit(false);
        }
    }


    public void UpdateErrorsText()
    {
        if (errorsText != null)
        {
            errorsText.text = $"Errors: {currentErrors}/{MaxErrors}";
        }
    }
    public void UpdateRatingText(int puzzleRating)
    {
        if (ratingText != null)
        {
            ratingText.text = $"Rating: {puzzleRating}";
        }
    }
    public void UpdateTimeText()
    {
        if (timeText != null && puzzleStartTime != DateTime.MinValue)
        {
            TimeSpan duration = DateTime.Now - puzzleStartTime;
            string durationFormat = duration.ToString("hh\\:mm\\:ss");
            timeText.text = $"Time: {durationFormat}";
        }
    }

}
