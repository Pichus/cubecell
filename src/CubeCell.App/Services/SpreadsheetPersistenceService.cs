using ClosedXML.Excel;

namespace CubeCell.App.Services;

public class SpreadsheetPersistenceService : ISpreadsheetPersistenceService
{
    private readonly ICellReader _cellReader;

    public SpreadsheetPersistenceService(ICellReader cellReader)
    {
        _cellReader = cellReader;
    }

    public void CreateSpreadsheetAndSave(string filePath, string fileName)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.AddWorksheet("Sheet1");

        AddCellsFromCellStorageToWorksheet(worksheet);

        workbook.SaveAs(filePath);
    }

    public void UpdateExistingSpreadsheetAndSave(string filepath)
    {
        using var workbook = new XLWorkbook(filepath);

        IXLWorksheet worksheet;
        if (workbook.TryGetWorksheet("Sheet1", out var existingSheet))
        {
            worksheet = existingSheet;
        }
        else
        {
            worksheet = workbook.AddWorksheet("Sheet1");
        }

        AddCellsFromCellStorageToWorksheet(worksheet);

        workbook.Save();
    }

    private void AddCellsFromCellStorageToWorksheet(IXLWorksheet worksheet)
    {
        var cells = _cellReader.GetCells();

        foreach (var (key, value) in cells)
        {
            if (!string.IsNullOrEmpty(value.Formula) && value.Formula.StartsWith("="))
            {
                worksheet.Cell(key.Row + 1, key.Col + 1).FormulaA1 = value.Formula;
            }
            else
            {
                worksheet.Cell(key.Row + 1, key.Col + 1).Value = value.Value;
            }
        }
    }
}
