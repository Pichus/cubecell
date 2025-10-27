using CubeCell.App.Models;

namespace CubeCell.App.Services;

public interface ICellWriter
{
    public void SetCell(CellCoordinates coordinates, Cell cell);
    public void UpdateCell(CellCoordinates cellCoordinates, Cell cell, string? value = null, string? formula = null);
}
