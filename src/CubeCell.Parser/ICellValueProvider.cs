namespace CubeCell.Parser;

public interface ICellValueProvider
{
    public string? GetCellValueByAddress(string address);
}
