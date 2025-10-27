using System.Collections.Generic;

using CubeCell.App.Service;
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
        var (col, row) = CellAddressUtils.AddressToCoordinates(address);
        return GetCell(col, row);
    }

    public IEnumerable<KeyValuePair<CellCoordinates, Cell>> GetCells()
    {
        return _cells.AsReadOnly();
    }

    public void SetCell(CellCoordinates coordinates, Cell cell)
    {
        _cells[coordinates] = cell;
    }

    public string? GetCellValueByAddress(string address)
    {
        return GetCell(address)?.Value;
    }
}
