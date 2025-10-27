using CubeCell.App.Models;

namespace CubeCell.App.Service;

public interface ICellWriter
{
    public void SetCell(CellCoordinates coordinates, Cell cell);
}
