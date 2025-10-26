using System;

using CubeCell.App.Models;

namespace CubeCell.App.Utils;

public static class CellAddressUtils
{
    public static CellCoordinates AddressToCoordinates(string address)
    {
        var i = 0;
        var col = 0;

        while (i < address.Length && char.IsLetter(address[i]))
        {
            col = (col * 26) + (char.ToUpper(address[i]) - 'A') + 1;
            i++;
        }

        col--;

        var row = int.Parse(address[i..]) - 1;

        return new CellCoordinates(col, row);
    }

    public static string CoordinatesToAddress(CellCoordinates coordinates)
    {
        int col = coordinates.Col;
        int row = coordinates.Row;

        var colLetters = ColumnIndexToLetters(col);
        
        string rowNumber = (row + 1).ToString();

        return colLetters + rowNumber;
    }

    public static string CoordinatesToAddress(int col, int row)
    {
        return CoordinatesToAddress(new CellCoordinates(col, row));
    }

    public static string ColumnIndexToLetters(int index)
    {
        string colLetters = string.Empty;
        int dividend = index + 1;

        while (dividend > 0)
        {
            int modulo = (dividend - 1) % 26;
            colLetters = (char)('A' + modulo) + colLetters;
            dividend = (dividend - 1) / 26;
        }

        return colLetters;
    }
    
    public static int ColumnLettersToIndex(string letters)
    {
        int col = 0;
        
        foreach (var letter in letters)
        {
            if (!char.IsLetter(letter))
            {
                throw new ArgumentException();
            }

            col = (col * 26) + (char.ToUpper(letter) - 'A') + 1;
        }

        col--;
        
        return col;
    }
}
