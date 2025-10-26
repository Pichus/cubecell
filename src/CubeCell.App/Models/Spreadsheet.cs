using System;
using System.Collections.Generic;

using CubeCell.App.Utils;
using CubeCell.Parser;

namespace CubeCell.App.Models;

public class Spreadsheet : IReadonlyCellStorage
{
    private readonly Dictionary<CellCoordinates, Cell> _cells = new();
    private readonly DependencyGraph _dependencyGraph = new();

    public string? GetCellValueByAddress(string address)
    {
        return GetCell(address)?.Value;
    }

    public void SetCell(CellCoordinates coordinates, Cell cell)
    {
        _cells[coordinates] = cell;
        CellChanged?.Invoke(this, new CellChangedEventArgs(coordinates, cell));
    }

    public Cell? GetCell(int col, int row)
    {
        _cells.TryGetValue(new CellCoordinates(col, row), out Cell? cell);

        return cell;
    }

    public Cell? GetCell(string address)
    {
        var (col, row) = CellAddressUtils.AddressToCoordinates(address);
        return GetCell(col, row);
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
