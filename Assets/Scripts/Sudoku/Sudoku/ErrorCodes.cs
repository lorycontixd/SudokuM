using System;

public sealed class ErrorCodes
{
    /**
     * Sudoku board loading failed.
     *
     * @see SudokuSolver
     * @see SudokuSolver#loadBoard(int)
     * #see SudokuSolver#loadBoard(int[,])
     * @see SudokuSolver#loadBoard(String)
     */
    public const int SUDOKUSOLVER_LOADBOARD_LOADING_FAILED = -100;
    /**
     * Sudoku solving requested, but not started.
     *
     * @see SudokuSolver
     * @see SudokuSolver#solve()
     */
    public const int SUDOKUSOLVER_SOLVE_SOLVING_NOT_STARTED = -101;
    /**
     * Sudoku solving requested, but falied.
     *
     * @see SudokuSolver
     * @see SudokuSolver#solve()
     */
    public const int SUDOKUSOLVER_SOLVE_SOLVING_FAILED = -102;
    /**
     * Finding all Sudoku solutions requested, but not started.
     *
     * @see SudokuSolver
     * @see SudokuSolver#findAllSolutions()
     */
    public const int SUDOKUSOLVER_FINDALLSOLUTIONS_SEARCHING_NOT_STARTED = -103;
    /**
     * Finding all Sudoku solutions requested, but not started.
     *
     * @see SudokuSolver
     * @see SudokuSolver#findAllSolutions()
     */
    public const int SUDOKUSOLVER_CHECKIFUNIQUESOLUTION_CHECKING_NOT_STARTED = -104;
    /**
     * Incorrect cell definition while calling setCell method (incorrect index or incorrect digit).
     * @see SudokuSolver
     * @see SudokuSolver#setCell(int, int, int)
     */
    public const int SUDOKUSOLVER_SETCELL_INCORRECT_DEFINITION = -105;
    /**
     * Incorrect cell definition while calling getCell method (incorrect index).
     * @see SudokuSolver
     * @see SudokuSolver#getCellDigit(int, int)
     */
    public const int SUDOKUSOLVER_GETCELLDIGIT_INCORRECT_INDEX = -106;
    /**
     * Incorrect segment index while calling {@link SudokuStore#boardSegmentStartIndex(int)}
     *
     * @see SudokuStore
     * @see SudokuStore#boardSegmentStartIndex(int)
     */
    public const int SUDOKUSTORE_BOARDSEGMENTSTARTINDEX_INCORRECT_SEGMENT = -107;
    /**
     * Negative or zero parameter while calling {@link SudokuStore#randomIndex(int)}
     *
     * @see SudokuStore
     * @see SudokuStore#randomIndex(int)
     */
    public const int SUDOKUSTORE_RANDOMINDEX_INCORRECT_PARAMETER = -108;
    /**
     * Negative or zero parameter while calling {@link SudokuStore#randomNumber(int)}
     *
     * @see SudokuStore
     * @see SudokuStore#randomNumber(int)
     */
    public const int SUDOKUSTORE_RANDOMNUMBER_INCORRECT_PARAMETER = -109;
    /**
     * Board contains an error.
     * @see SudokuSolver
     */
    public const int SUDOKUSOLVER_BOARD_ERROR = -110;
    /**
     * Obvious puzzle error.
     *
     * @see SudokuStore
     * @see SudokuStore#calculatePuzzleRating(int[,])
     */
    public const int SUDOKUSTORE_CALCULATEPUZZLERATING_PUZZLE_ERROR = -111;
    /**
     * Puzzle solution does not exist.
     *
     * @see SudokuStore
     * @see SudokuStore#calculatePuzzleRating(int[,])
     */
    public const int SUDOKUSTORE_CALCULATEPUZZLERATING_NO_SOLUTION = -112;
    /**
     * Puzzle has non-unique solution.
     *
     * @see SudokuStore
     * @see SudokuStore#calculatePuzzleRating(int[,])
     */
    public const int SUDOKUSTORE_CALCULATEPUZZLERATING_NON_UNIQUE_SOLUTION = -113;
    /**
     * Threads join failed.
     *
     * @see SudokuStore
     * @see SudokuStore#calculatePuzzleRating(int[,])
     */
    public const int SUDOKUSTORE_CALCULATEPUZZLERATING_THREADS_JOIN_FAILED = -114;
    /**
     * Sudoku board loading failed.
     *
     * @see SudokuSolver
     * @see SudokuSolver#loadBoard(int)
     * @see SudokuSolver#loadBoard(int[,])
     * @see SudokuSolver#loadBoard(String)
     */
    public const String SUDOKUSOLVER_LOADBOARD_LOADING_FAILED_MSG = "Failed loading sudoku board.";
    /**
	 * Sudoku solving requested, but not started.
	 *
	 * @see SudokuSolver
	 * @see SudokuSolver#solve()
	 */
    public const String SUDOKUSOLVER_SOLVE_SOLVING_NOT_STARTED_MSG = "Sudoku solving process not started - board is not ready.";
    /**
	 * Sudoku solving requested, but falied.
	 *
	 * @see SudokuSolver
	 * @see SudokuSolver#solve()
	 */
    public const String SUDOKUSOLVER_SOLVE_SOLVING_FAILED_MSG = "Sudoku solving process started, but failed - board contains an error?";
    /**
	 * Finding all Sudoku solutions requested, but not started.
	 *
	 * @see SudokuSolver
	 * @see SudokuSolver#findAllSolutions()
	 */
    public const String SUDOKUSOLVER_FINDALLSOLUTIONS_SEARCHING_NOT_STARTED_MSG = "Searching for all solutions not started = board is not ready.";
    /**
	 * Finding all Sudoku solutions requested, but not started.
	 *
	 * @see SudokuSolver
	 * @see SudokuSolver#findAllSolutions()
	 */
    public const String SUDOKUSOLVER_CHECKIFUNIQUESOLUTION_CHECKING_NOT_STARTED_MSG = "Checking if only unique solution exists not started - board is not ready.";
    /**
	 * Incorrect cell definition while calling setCell method (incorrect index or incorrect digit).
	 * @see SudokuSolver
	 * @see SudokuSolver#setCell(int, int, int)
	 */
    public const String SUDOKUSOLVER_SETCELL_INCORRECT_DEFINITION_MSG = "Trying to access the cell, but definition contains an error.";
    /**
	 * Incorrect cell definition while calling getCell method (incorrect index).
	 * @see SudokuSolver
	 * @see SudokuSolver#getCellDigit(int, int)
	 */
    public const String SUDOKUSOLVER_GETCELLDIGIT_INCORRECT_INDEX_MSG = "Trying to access the cell, but index out of board range.";
    /**
	 * Incorrect segment index while calling {@link SudokuStore#boardSegmentStartIndex(int)}
	 *
	 * @see SudokuStore
	 * @see SudokuStore#boardSegmentStartIndex(int)
	 */
    public const String SUDOKUSTORE_BOARDSEGMENTSTARTINDEX_INCORRECT_SEGMENT_MSG = "Incorrect board segment index - should be betweem 0 and 2.";
    /**
	 * Negative or zero parameter while calling {@link SudokuStore#randomIndex(int)}
	 *
	 * @see SudokuStore
	 * @see SudokuStore#randomIndex(int)
	 */
    public const String SUDOKUSTORE_RANDOMINDEX_INCORRECT_PARAMETER_MSG = "Parameter can not be negative.";
    /**
	 * Negative or zero parameter while calling {@link SudokuStore#randomNumber(int)}
	 *
	 * @see SudokuStore
	 * @see SudokuStore#randomNumber(int)
	 */
    public const String SUDOKUSTORE_RANDOMNUMBER_INCORRECT_PARAMETER_MSG = "Parameter has to be positive.";
    /**
	 * Board contains an error.
	 * @see SudokuSolver
	 */
    public const String SUDOKUSOLVER_BOARD_ERROR_MSG = "Sudoku board contains an error.";
    /**
	 * Puzzle contains obvious puzzle error.
	 *
	 * @see SudokuStore
	 * @see SudokuStore#calculatePuzzleRating(int[,])
	 */
    public const String SUDOKUSTORE_CALCULATEPUZZLERATING_PUZZLE_ERROR_MSG = "Puzzle contains obvious puzzle error.";
    /**
	 * Puzzle solution does not exist.
	 *
	 * @see SudokuStore
	 * @see SudokuStore#calculatePuzzleRating(int[,])
	 */
    public const String SUDOKUSTORE_CALCULATEPUZZLERATING_NO_SOLUTION_MSG = "Puzzle solution does not exist.";
    /**
	 * Puzzle has non-unique solution.
	 *
	 * @see SudokuStore
	 * @see SudokuStore#calculatePuzzleRating(int[,])
	 */
    public const String SUDOKUSTORE_CALCULATEPUZZLERATING_NON_UNIQUE_SOLUTION_MSG = "Puzzle has non-unique solution.";
    /**
	 * Threads join failed.
	 *
	 * @see SudokuStore
	 * @see SudokuStore#calculatePuzzleRating(int[,])
	 */
    public const String SUDOKUSTORE_CALCULATEPUZZLERATING_THREADS_JOIN_FAILED_MSG = "Threads join failed.";

    /**
	 * Error code unknown
	 * @see #getErrorDescription(int)
	 */
    public const String ERROR_CODE_UNKNOWN_MSG = "Incorrect error code.";
    /**
	 * Return error code description.
	 *
	 * @param errorCode      Error code
	 * @return               Error code description if error code known,
	 *                       otherwise {@link #ERROR_CODE_UNKNOWN_MSG}.
	 */
    public static String getErrorDescription(int errorCode)
    {
        switch (errorCode)
        {
            case SUDOKUSOLVER_LOADBOARD_LOADING_FAILED: return SUDOKUSOLVER_LOADBOARD_LOADING_FAILED_MSG;
            case SUDOKUSOLVER_SOLVE_SOLVING_NOT_STARTED: return SUDOKUSOLVER_SOLVE_SOLVING_NOT_STARTED_MSG;
            case SUDOKUSOLVER_SOLVE_SOLVING_FAILED: return SUDOKUSOLVER_SOLVE_SOLVING_FAILED_MSG;
            case SUDOKUSOLVER_FINDALLSOLUTIONS_SEARCHING_NOT_STARTED: return SUDOKUSOLVER_FINDALLSOLUTIONS_SEARCHING_NOT_STARTED_MSG;
            case SUDOKUSOLVER_CHECKIFUNIQUESOLUTION_CHECKING_NOT_STARTED: return SUDOKUSOLVER_CHECKIFUNIQUESOLUTION_CHECKING_NOT_STARTED_MSG;
            case SUDOKUSOLVER_SETCELL_INCORRECT_DEFINITION: return SUDOKUSOLVER_SETCELL_INCORRECT_DEFINITION_MSG;
            case SUDOKUSOLVER_GETCELLDIGIT_INCORRECT_INDEX: return SUDOKUSOLVER_GETCELLDIGIT_INCORRECT_INDEX_MSG;
            case SUDOKUSTORE_BOARDSEGMENTSTARTINDEX_INCORRECT_SEGMENT: return SUDOKUSTORE_BOARDSEGMENTSTARTINDEX_INCORRECT_SEGMENT_MSG;
            case SUDOKUSTORE_RANDOMINDEX_INCORRECT_PARAMETER: return SUDOKUSTORE_RANDOMINDEX_INCORRECT_PARAMETER_MSG;
            case SUDOKUSTORE_RANDOMNUMBER_INCORRECT_PARAMETER: return SUDOKUSTORE_RANDOMNUMBER_INCORRECT_PARAMETER_MSG;
            case SUDOKUSOLVER_BOARD_ERROR: return SUDOKUSOLVER_BOARD_ERROR_MSG;
            case SUDOKUSTORE_CALCULATEPUZZLERATING_PUZZLE_ERROR: return SUDOKUSTORE_CALCULATEPUZZLERATING_PUZZLE_ERROR_MSG;
            case SUDOKUSTORE_CALCULATEPUZZLERATING_NO_SOLUTION: return SUDOKUSTORE_CALCULATEPUZZLERATING_NO_SOLUTION_MSG;
            case SUDOKUSTORE_CALCULATEPUZZLERATING_NON_UNIQUE_SOLUTION: return SUDOKUSTORE_CALCULATEPUZZLERATING_NON_UNIQUE_SOLUTION_MSG;
            case SUDOKUSTORE_CALCULATEPUZZLERATING_THREADS_JOIN_FAILED: return SUDOKUSTORE_CALCULATEPUZZLERATING_THREADS_JOIN_FAILED_MSG;
        }
        return ERROR_CODE_UNKNOWN_MSG;
    }
    /**
     * Checks whether error code exists.
     *
     * @param errorCode      Error code
     * @return               True if error code exists, otherwise false.
     */
    public static bool errorCodeExists(int errorCode)
    {
        switch (errorCode)
        {
            case SUDOKUSOLVER_LOADBOARD_LOADING_FAILED: return true;
            case SUDOKUSOLVER_SOLVE_SOLVING_NOT_STARTED: return true;
            case SUDOKUSOLVER_SOLVE_SOLVING_FAILED: return true;
            case SUDOKUSOLVER_FINDALLSOLUTIONS_SEARCHING_NOT_STARTED: return true;
            case SUDOKUSOLVER_CHECKIFUNIQUESOLUTION_CHECKING_NOT_STARTED: return true;
            case SUDOKUSOLVER_SETCELL_INCORRECT_DEFINITION: return true;
            case SUDOKUSOLVER_GETCELLDIGIT_INCORRECT_INDEX: return true;
            case SUDOKUSTORE_BOARDSEGMENTSTARTINDEX_INCORRECT_SEGMENT: return true;
            case SUDOKUSTORE_RANDOMINDEX_INCORRECT_PARAMETER: return true;
            case SUDOKUSTORE_RANDOMNUMBER_INCORRECT_PARAMETER: return true;
            case SUDOKUSOLVER_BOARD_ERROR: return true;
        }
        return false;
    }
    public static void consolePrintlnIfError(int errorCode)
    {
        if (errorCodeExists(errorCode))
            Console.WriteLine(getErrorDescription(errorCode));
    }
}