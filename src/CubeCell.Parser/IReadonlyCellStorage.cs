namespace CubeCell.Parser;

public interface IReadonlyCellStorage
{
    public string? GetCellValueByAddress(string address);
}
