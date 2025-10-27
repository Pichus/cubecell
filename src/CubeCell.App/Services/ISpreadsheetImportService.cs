using CubeCell.App.Models;

namespace CubeCell.App.Services;

public interface ISpreadsheetImportService
{
    public SpreadsheetSize LoadSpreadsheetFromFile(ICellWriter cellStorage, string filePath);
}
