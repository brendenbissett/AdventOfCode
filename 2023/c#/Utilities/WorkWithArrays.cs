using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class WorkWithArrays
    {
        public static char[][] ConvertListTo2DCharArray(List<string> stringList)
        {
            int numRows = stringList.Count;
            int maxStringLength = stringList.Max(s => s.Length);

            char[][] charArray = new char[numRows][];

            for (int i = 0; i < numRows; i++)
            {
                charArray[i] = new char[maxStringLength];

                for (int j = 0; j < maxStringLength; j++)
                {
                    if (j < stringList[i].Length)
                    {
                        charArray[i][j] = stringList[i][j];
                    }
                    else
                    {
                        // If the string is shorter than maxStringLength, pad with a default character
                        charArray[i][j] = ' ';
                    }
                }
            }

            return charArray;
        }

        public static char[,] ConvertListTo2DChar_StaggeredArray(List<string> stringList)
        {
            int numRows = stringList.Count;
            int maxStringLength = stringList.Max(s => s.Length);

            char[,] charArray = new char[numRows, maxStringLength];

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < maxStringLength; j++)
                {
                    if (j < stringList[i].Length)
                    {
                        charArray[i,j] = stringList[i][j];
                    }
                    else
                    {
                        // If the string is shorter than maxStringLength, pad with a default character
                        charArray[i,j] = ' ';
                    }
                }
            }

            return charArray;
        }

        public static int[,] ConvertListTo2DInt_StaggeredArray(List<string> stringList)
        {
            int numRows = stringList.Count;
            int maxStringLength = stringList.Max(s => s.Length);

            int[,] array = new int[numRows, maxStringLength];

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < maxStringLength; j++)
                {
                    if (j < stringList[i].Length)
                    {
                        array[i, j] = Convert.ToInt32(new string(stringList[i][j], 1));
                    }
                    else
                    {
                        // If the string is shorter than maxStringLength, pad with a default character
                        array[i, j] = 0;
                    }
                }
            }

            return array;
        }

        public static string TransposeRowsAndColumns(this string str, string rowDelimiter, string columnDelimiter)
        {
            string[] rows = str.Split(new string[] { rowDelimiter }, StringSplitOptions.None);
            string[][] arr = new string[rows.Length][];
            for (int i = 0; i < rows.Length; i++)
            {
                arr[i] = rows[i].Split(new string[] { columnDelimiter }, StringSplitOptions.None);
            }
            string[][] transposed = TransposeRowsAndColumns(arr);
            string[] transposedRows = new string[transposed.Length];
            for (int i = 0; i < transposed.Length; i++)
            {
                transposedRows[i] = String.Join(columnDelimiter, transposed[i]);
            }
            return String.Join(rowDelimiter, transposedRows);
        }

        public static T[,] TransposeRowsAndColumns<T>(this T[,] arr)
        {
            int rowCount = arr.GetLength(0);
            int columnCount = arr.GetLength(1);
            T[,] transposed = new T[columnCount, rowCount];
            if (rowCount == columnCount)
            {
                transposed = (T[,])arr.Clone();
                for (int i = 1; i < rowCount; i++)
                {
                    for (int j = 0; j < i; j++)
                    {
                        T temp = transposed[i, j];
                        transposed[i, j] = transposed[j, i];
                        transposed[j, i] = temp;
                    }
                }
            }
            else
            {
                for (int column = 0; column < columnCount; column++)
                {
                    for (int row = 0; row < rowCount; row++)
                    {
                        transposed[column, row] = arr[row, column];
                    }
                }
            }
            return transposed;
        }

        public static T[][] TransposeRowsAndColumns<T>(this T[][] arr)
        {
            int rowCount = arr.Length;
            int columnCount = arr[0].Length;
            T[][] transposed = new T[columnCount][];
            if (rowCount == columnCount)
            {
                transposed = (T[][])arr.Clone();
                for (int i = 1; i < rowCount; i++)
                {
                    for (int j = 0; j < i; j++)
                    {
                        T temp = transposed[i][j];
                        transposed[i][j] = transposed[j][i];
                        transposed[j][i] = temp;
                    }
                }
            }
            else
            {
                for (int column = 0; column < columnCount; column++)
                {
                    transposed[column] = new T[rowCount];
                    for (int row = 0; row < rowCount; row++)
                    {
                        transposed[column][row] = arr[row][column];
                    }
                }
            }
            return transposed;
        }
    }
}
