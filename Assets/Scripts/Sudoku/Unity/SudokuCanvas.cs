using GooglePlayGames;
using Managers;
using Michsky.MUIP;
using NUnit.Framework.Constraints;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.VFX;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

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
    public bool halfBoardSent { get; private set; } = false;

    [SerializeField] private GameObject gridParent = null;
    public int currentDigit;
    public List<SudokuCanvasCell> cells;
    public List<SudokuNumberButton> valueButtons = new List<SudokuNumberButton>();


    [Header("UI")]
    [SerializeField] private ButtonManager statsPanelButton;
    [SerializeField] private ButtonManager historyPanelButton;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button emojiPanelButton;
    [SerializeField] private PausePanel pausePanel;
    [SerializeField] private StatsPanel statsPanel;
    [SerializeField] private HistoryPanel historyPanel;
    [SerializeField] private EmojiPanel emojiPanel;
    [SerializeField] private TextMeshProUGUI errorsText;
    [SerializeField] private TextMeshProUGUI ratingText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI boardLoadingText;
    [SerializeField] private EnemyHalfBoard enemyHalfBoard = null;

    [Header("Final Panel")]
    [SerializeField] private GameObject finalPanel;
    [SerializeField] private TextMeshProUGUI finalText; // you lose/you win text
    [SerializeField] private TextMeshProUGUI finalScoreGainedText;
    [SerializeField] private TextMeshProUGUI finalScoreNewText;
    [SerializeField] private TextMeshProUGUI finalWaitingForHostText;
    [SerializeField] private ButtonManager finalNewGameButton;
    [SerializeField] private ButtonManager finalRetryGameButton;
    [SerializeField] private ButtonManager finalLeaveButton;

    public float GameTime { get; private set; }

    private int[,] currentPuzzle;
    private SudokuSolver currentSolver = null;
    private int[,] currentSolvedBoard;
    private int currentRating = -1;
    private bool IsEmpty = true;
    //private bool IsPlaying = false;
    private int currentErrors = 0;
    private DateTime puzzleStartTime = DateTime.MinValue;
    private GameObject currentPanel = null;
    private SudokuCanvasCell lastEditedCell = null;
    private bool WonGame = false;


    private void Start()
    {
        //LeaderboardManager.Instance.onGetPlayerScore += OnGetPlayerScore;

        Debug.Log($"[SudokuCanvas] First log");
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
            finalScoreGainedText.gameObject.SetActive(false);
            finalScoreNewText.gameObject.SetActive(false);
        }


        // Deactivate history panel in timerace mode
        if (GameManager.Instance.GameMode == GameMode.COOP)
        {
            historyPanelButton.gameObject.SetActive(true);
            if (historyPanel != null)
            {
                historyPanel.gameObject.SetActive(true);
                historyPanel.Close();
            }
        }
        else
        {
            historyPanelButton.gameObject.SetActive(false);
            if (historyPanel != null)
            {
                historyPanel.gameObject.SetActive(false);
                historyPanel.Close();
            }
        }
        // Activate but close stats panel
        if (statsPanel != null)
        {
            statsPanel.gameObject.SetActive(true);
            statsPanel.Close();
        }

        // Activate but close pause panel
        if (pausePanel != null)
        {
            pausePanel.Close();
        }

        // Activate but close emojis panel
        if (emojiPanel != null)
        {
            emojiPanel.Close();
            emojiPanel.gameObject.SetActive(false);
        }

        PauseManager.Instance.onGamePause += OnGamePause;
        PauseManager.Instance.onGameUnpause += OnGameUnpause;
        GameManager.Instance.onGameFinish += OnGameFinish;
        /*NetworkManager.Instance.onOtherPlayerGameDisconnected += OnOtherPlayerDisconnect;
        NetworkManager.Instance.onGameDisconnected += OnGameDisconnect;
        NetworkManager.Instance.onResumeGameAfterReconnect += OnGameResumeAfterDc;*/

        ResetBoard();
    }

    

    private void OnOtherPlayerDisconnect(Player player)
    {
        Pause();
    }

    private void OnGameResumeAfterDc()
    {
        Unpause();
    }

    private void OnGameDisconnect()
    {
        Pause();
    }

    private void OnGameUnpause(int arg1, bool arg2, PauseManager.UnpauseReason reason)
    {
        Unpause();
    }

    private void OnGamePause(int arg1, bool arg2)
    {
        Pause();
    }

    private void Update()
    {
        if (GameManager.Instance.IsPlaying && !PauseManager.Instance.IsPaused)
        {
            GameTime += Time.deltaTime;
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
        finalPanel.SetActive(false);
        boardLoadingText.gameObject.SetActive(false);
        // Reactivate value buttons
        foreach (SudokuNumberButton val in valueButtons)
        {
            val.Activate();
        }
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
        finalPanel.SetActive(false);
        boardLoadingText.gameObject.SetActive(false);
        // Reactivate value buttons
        foreach (SudokuNumberButton val in valueButtons)
        {
            val.Activate();
        }
        // Fill board if in debug mode
        if (PhotonNetwork.IsMasterClient && DebugManager.Instance.DebugGame && DebugManager.Instance.CanvasFillForMasterClient)
        {
            StartCoroutine(AutoFillGradualCo(DebugManager.Instance.CanvasFillAmount));
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
        GameTime = 0f;
        IsEmpty = true;
        halfBoardSent = false;

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

    public enum AutoFillType
    {
        COUNT,
        EXCEPT
    }
    private void AutoFillGradual(int n, AutoFillType type = AutoFillType.COUNT)
    {
        List<SudokuCanvasCell> empties = cells.Where(cell => !cell.IsLocked).ToList();
        List<SudokuCanvasCell> toFill;
        if (type == AutoFillType.EXCEPT)
        {
            toFill = empties.OrderBy(a => new System.Random().Next()).ToList().Take(empties.Count - n).ToList();
        }
        else
        {
            toFill = empties.OrderBy(a => new System.Random().Next()).ToList().Take(n).ToList();
        }
        foreach (SudokuCanvasCell cell in toFill)
        {
            int val = currentSolvedBoard[cell.Row, cell.Col];
            GamePunEventSender.SendMove(PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.NickName, cell.Row, cell.Col, val, true, false, MyCompletedCells, Photon.Realtime.ReceiverGroup.All);
        }
    }
    private IEnumerator AutoFillGradualCo(int n = 1, float delay = 2f)
    {
        yield return new WaitForSeconds(delay);
        AutoFillGradual(n, DebugManager.Instance.CanvasFillType);
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
    public void ButtonEmojis()
    {
        if (emojiPanel.gameObject.activeSelf)
        {
            emojiPanel.Close();
        }
        else
        {
            emojiPanel.Open();
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
                //FinishAndClose(true);+
                GameManager.Instance.Finish(true);
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
                    GameManager.Instance.Finish(false);
                    //Finish(false);
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
            
            if (GameManager.Instance.GameMode == GameMode.COOP)
            {
                cell.SetNewValue(userid, digit, true);
                CheckValueButtonsToDisable();
                bool isComplete = IsPuzzleComplete();
                if (IsPuzzleCompleteAndCorrect())
                {
                    Debug.Log($"FINISHED!");
                    GameManager.Instance.Finish(true);
                    //FinishAndClose(true);
                }
            }else if (GameManager.Instance.GameMode == GameMode.TIMERACE)
            {
                cell.SetNewValue(digit, true);
                CheckValueButtonsToDisable();
                if (userid == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    bool isComplete = IsPuzzleComplete();
                    if (IsPuzzleCompleteAndCorrect())
                    {
                        Debug.Log($"FINISHED! SENDING FINISH");
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
                    GameManager.Instance.Finish(false);
                    GamePunEventSender.SendFinish(
                        PhotonNetwork.LocalPlayer.ActorNumber,
                        PhotonNetwork.PlayerListOthers.First().ActorNumber
                    );
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
                    int requiredHalf = (int)SudokuBoard.BOARD_CELLS_NUMBER / 2;
                    if (MyCompletedCells == requiredHalf && !halfBoardSent)
                    {
                        Debug.Log($"I reached half of the board! Sending notif");
                        GamePunEventSender.SendHalfBoard(PhotonNetwork.LocalPlayer.ActorNumber);
                        halfBoardSent = true;
                    }
                    break;
                case GameMode.COOP:
                    GamePunEventSender.SendMove(PhotonNetwork.LocalPlayer.ActorNumber, PhotonNetwork.NickName, currentSelectedCell.Row, currentSelectedCell.Col, number.Value, isCorrect, false, MyCompletedCells+1, Photon.Realtime.ReceiverGroup.All);
                    break;
                default:
                    break;
            }
            
        }
    }

    private void UpdateFinalScoreText(bool isWin, int oldscore, int gscore)
    {
        int newscore = oldscore + gscore;

        finalPanel.SetActive(true);
        if (finalText != null)
        {
            finalText.gameObject.SetActive(true);
            finalText.text = isWin ? "Victory!" : "Defeat";
        }
        if (GameManager.Instance.IsRankedGame)
        {
            finalScoreNewText.gameObject.SetActive(true);
            finalScoreGainedText.gameObject.SetActive(true);
            finalScoreGainedText.text = $"{gscore} points";
            finalScoreNewText.text = $"New Score: {newscore}";
        }
        else
        {
            finalScoreNewText.gameObject.SetActive(true);
            finalScoreNewText.text = "Unranked game gives no score";
        }

        finalNewGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        finalRetryGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        finalWaitingForHostText.gameObject.SetActive(!PhotonNetwork.IsMasterClient);
    }

    private void OnGameFinish(bool isPlayerWin, bool isValid, int oldScore, int scoreChange)
    {
        pausePanel.Close();
        statsPanel.Close();
        historyPanel.Close();
        emojiPanel.Close();
        if (!isValid)
        {
            finalPanel.SetActive(true);
            finalText.gameObject.SetActive(true);
            finalText.text = isPlayerWin ? "Victory!" : "Defeat";
            finalScoreNewText.gameObject.SetActive(true);
            finalScoreNewText.text = "Unranked game gives no score";
            finalNewGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
            finalRetryGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
            finalWaitingForHostText.gameObject.SetActive(!PhotonNetwork.IsMasterClient);
        }
        else
        {
            UpdateFinalScoreText(isPlayerWin, oldScore, scoreChange);
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
            TimeSpan duration = TimeSpan.FromSeconds(GameTime);
            string durationFormat = duration.ToString("hh\\:mm\\:ss");
            timeText.text = $"Time: {durationFormat}";
        }
    }

    public void SetGametime(float time)
    {
        GameTime = time;
    }

    public void Pause()
    {
        pauseButton.interactable = false;
        pausePanel.gameObject.SetActive(true);
        pausePanel.Open();
        pausePanel.UpdateUI();
    }
    public void Unpause()
    {
        pauseButton.interactable = true;
        pausePanel.Close();
        pausePanel.gameObject.SetActive(false);
    }

    #region Enemy Half board
    public void ShowEnemyHalfBoard()
    {
        if (enemyHalfBoard != null)
        {
            StartCoroutine(ShowEnemyHalfBoardCo(5f));
        }
    }
    private IEnumerator ShowEnemyHalfBoardCo(float duration = 3f)
    {
        enemyHalfBoard.Show();
        yield return new WaitForSeconds(duration);
        enemyHalfBoard.Hide();
    }
    #endregion
}
