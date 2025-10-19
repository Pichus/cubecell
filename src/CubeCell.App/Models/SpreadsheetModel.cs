using System.Collections.ObjectModel;

namespace CubeCell.App.Models;

public class SpreadsheetModel
{
    public ObservableCollection<CellModel> Cells { get; set; } = new();
    public int Rows { get; set; }
    public int Columns { get; set; }
}