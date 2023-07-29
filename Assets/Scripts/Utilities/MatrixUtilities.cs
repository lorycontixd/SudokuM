using System.Linq;

public static class MatrixUtilities
{
    public static (int[], int) DecomposeMatrix(int[,] matrix)
    {
        int N = matrix.GetLength(0);
        return (matrix.Cast<int>().ToArray(), N);
    }
    public static int[,] ComposeMatrix(int[] array, int N)
    {
        return Matrix<int>(array, N);
    }
    public static T[,] Matrix<T>(T[] arr, int rows)
    {
        var cols = arr.Length / rows;
        var m = new T[rows, cols];
        for (var i = 0; i < arr.Length; i++)
            m[i / cols, i % cols] = arr[i];
        return m;
    }
}