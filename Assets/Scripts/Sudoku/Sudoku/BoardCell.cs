using System;

public class BoardCell
{
    /**
     * Empty cell.
     */
    public const int EMPTY = EmptyCell.CELL_ID;
    /**
     * Cell is not pointing to any cells on the board.
     */
    public const int INDEX_NULL = -1;
    /**
     * Row index of board entry.
     */
    public int rowIndex;
    /**
     * Column index of board entry.
     */
    public int colIndex;
    /**
     * Entry digit.
     */
    public int digit;
    /**
     * Random seed.
     */
    internal double randomSeed;
    /**
     * Digits still free number.
     */
    internal int digitsStillFreeNumber;
    /**
     * Default constructor - uninitialized entry.
     */
    /**
     * Marker if analyzed digit 0...9 is still not used.
     */
    internal const int DIGIT_STILL_FREE = 1;
    /**
     * Digit 0...9 can not be used in that place.
     */
    internal const int DIGIT_IN_USE = 2;
    /**
     * Cell is not pointing to any cells on the board.
     */
    public BoardCell()
    {
        rowIndex = INDEX_NULL;
        colIndex = INDEX_NULL;
        digit = EMPTY;
        randomSeed = SudokuStore.nextRandom();
        digitsStillFreeNumber = -1;
    }
    /**
     * Constructor - initialized entry.
     * @param rowIndex   Row index.
     * @param colIndex   Column index.
     * @param digit    Entry digit.
     */
    public BoardCell(int rowIndex, int colIndex, int digit)
    {
        this.rowIndex = rowIndex;
        this.colIndex = colIndex;
        this.digit = digit;
        randomSeed = SudokuStore.nextRandom();
        digitsStillFreeNumber = -1;
    }
    /**
     * Package level method
     * @return
     */
    internal int order()
    {
        return digitsStillFreeNumber;
    }
    /**
     * Package level method
     * @return
     */
    internal double orderPlusRndSeed()
    {
        return digitsStillFreeNumber + randomSeed;
    }
}