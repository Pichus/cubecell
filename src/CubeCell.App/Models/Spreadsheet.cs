using System;
using System.Collections.Generic;

using CubeCell.App.Services;
using CubeCell.App.Utils;
using CubeCell.Parser;

namespace CubeCell.App.Models;

public class Spreadsheet : ICellStorage, ICellValueProvider
{
    private readonly Dictionary<CellCoordinates, Cell> _cells = new();

    public Cell? GetCell(int col, int row)
    {
        _cells.TryGetValue(new CellCoordinates(col, row), out Cell? cell);

        return cell;
    }

    public Cell? GetCell(string address)
    {
        (int col, int row) = CellAddressUtils.AddressToCoordinates(address);
        return GetCell(col, row);
    }

    public IEnumerable<KeyValuePair<CellCoordinates, Cell>> GetCells()
    {
        return _cells.AsReadOnly();
    }

    public void SetCell(CellCoordinates coordinates, Cell cell)
    {
        _cells[coordinates] = cell;
        CellChanged?.Invoke(this, new CellChangedEventArgs(coordinates, cell));
    }

    public string? GetCellValueByAddress(string address)
    {
        return GetCell(address)?.Value;
    }

    public void UpdateCell(CellCoordinates cellCoordinates, Cell cell, string? value = null, string? formula = null)
    {
        if (value is not null)
        {
            cell.Value = value;
        }

        if (formula is not null)
        {
            cell.Formula = formula;
        }

        CellChanged?.Invoke(this, new CellChangedEventArgs(cellCoordinates, cell));
    }

    public event EventHandler<CellChangedEventArgs>? CellChanged;
}

public class CellChangedEventArgs : EventArgs
{
    public CellChangedEventArgs(CellCoordinates cellCoordinates, Cell cell)
    {
        CellCoordinates = cellCoordinates;
        Cell = cell;
    }

    public CellCoordinates CellCoordinates { get; }
    public Cell Cell { get; }
}
