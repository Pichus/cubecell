using System.Collections.Generic;

using CubeCell.App.Models;

namespace CubeCell.App.Service;

public interface ICellReader
{
    public Cell? GetCell(int col, int row);

    public Cell? GetCell(string address);

    public IReadOnlyDictionary<CellCoordinates, Cell> GetCells();
}
