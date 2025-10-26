using System.Collections.Generic;

using CubeCell.App.Utils;

namespace CubeCell.App.Models;

public class Spreadsheet
{
    private readonly Dictionary<CellCoordinates, Cell> _cells = new();
    private readonly DependencyGraph _dependencyGraph = new();

    public bool TryAddCell(CellCoordinates coordinates, Cell cell)
    {
        return _cells.TryAdd(coordinates, cell);
    }

    public Cell? GetCell(int col, int row)
    {
        _cells.TryGetValue(new CellCoordinates(col, row), out Cell? cell);

        return cell ?? null;
    }

    public Cell? GetCell(string address)
    {
        var (col, row) = CellAddressUtils.AddressToCoordinates(address);
        return GetCell(col, row) ?? null;
    }
}
