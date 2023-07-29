using Michsky.MUIP;
using NUnit.Framework.Constraints;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    public int activeNumber = -1;

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


    private int[,] currentPuzzle;
    private SudokuSolver currentSolver = null;
    private int[,] currentSolvedBoard;
    private int currentRating = -1;
    private bool IsEmpty = true;
    private bool IsPlaying = false;
    private int currentErrors = 0;
    private DateTime puzzleStartTime = DateTime.MinValue;
    private GameObject currentPanel = null;


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
                cells[count].onCellClick.AddListener(OnCellUpdate);
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
                cells[count].onCellClick.AddListener(OnCellUpdate);
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

        DeselectAllValueButtons();
        historyPanel.ClearHistoryList();
    }

    #region Buttons
    public void ButtonDelete()
    {
        activeNumber = -1;
        DeselectAllValueButtons();
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

    public void OnCellUpdate(SudokuCanvasCell cell)
    {
        if (!cell.IsLocked)
        {
            bool isCorrect = activeNumber == currentSolvedBoard[cell.Row, cell.Col];
            bool isDeleteMove = activeNumber == -1;
            Debug.Log($"[OnCellUpdate] Sending move ==> Row: {cell.Row}, col: {cell.Col}, digit: {activeNumber}, iscorrect: {isCorrect}");
            GamePunEventSender.SendMove(PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.NickName, cell.Row, cell.Col, activeNumber, isCorrect, isDeleteMove);
        }
    }

    public void UpdateCell(int row, int col, int digit, bool isCorrect, bool isDeleteMove)
    {
        SudokuCanvasCell cell = cells.FirstOrDefault(c => c.Row == row && c.Col == col);
        if (isCorrect || isDeleteMove)
        {
            // valid
            cell.SetNewValue(digit, true);
            bool isComplete = IsPuzzleComplete();
            if (IsSolvedPuzzleCorrect())
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
                    FinishAndClose(false);
                }
            }
        }
        if (activeNumber != -1)
        {
            HighlightValueCells(activeNumber);
        }
    }
    public void UpdateCellMultiplayer(int userid, string username, int row, int col, int digit, bool isCorrect, bool isDeleteMove)
    {
        SudokuCanvasCell cell = cells.FirstOrDefault(c => c.Row == row && c.Col == col);
        if (isCorrect || isDeleteMove)
        {
            // valid
            cell.SetNewValue(userid, digit, true);
            CheckValueButtonsToDisable();
            bool isComplete = IsPuzzleComplete();
            if (IsPuzzleCompleteAndCorrect())
            {
                FinishAndClose(true);
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
        SetLastEditCell(row, col);
        currentPuzzle[row, col] = digit;
        if (activeNumber != -1)
        {
            HighlightValueCells(activeNumber);
        }
        Move move = new Move(userid, username, row, col, digit, isCorrect);
        if (HistoryManager.Instance != null)
        {
            HistoryManager.Instance.AddMove(move);
        }
        if (StatsManager.Instance != null)
        {
            if (!isDeleteMove)
            {
                StatsManager.Instance.AddMove(move);
            } 
        }
    }

    public void OnValueButtonClick(SudokuNumberButton number)
    {
        if (activeNumber != -1)
        {
            if (activeNumber == number.Value)
            {
                activeNumber = -1;
                number.SetSelectedAndUpdateMask(false);
                UnhighlightAllCells();
            }
            else
            {
                SudokuNumberButton current = valueButtons.FirstOrDefault(b => b.Value == activeNumber);
                current.SetSelectedAndUpdateMask(false);
                activeNumber = number.Value;
                number.SetSelectedAndUpdateMask(true);
                HighlightValueCells(activeNumber);
            }
        }
        else
        {
            activeNumber = number.Value;
            number.SetSelectedAndUpdateMask(true);
            UnhighlightAllCells();
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

    private void DeselectAllValueButtons()
    {
        foreach(var vb in valueButtons)
        {
            vb.SetSelectedAndUpdateMask(false);
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
        for (int i=0; i<cells.Count; i++)
        {
            cells[i].SetLastEdit(cells[i].Row == row && cells[i].Col == col);
        }
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
