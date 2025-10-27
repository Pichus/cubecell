using CubeCell.App.Models;

namespace CubeCell.App.Services;

public interface ICellWriter
{
    public void SetCell(CellCoordinates coordinates, Cell cell);
}
