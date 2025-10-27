using CubeCell.App.Models;

namespace CubeCell.App.Service;

public interface ISpreadsheetImportService
{
    public SpreadsheetSize LoadSpreadsheetFromFile(ICellWriter cellStorage, string filePath);
}
