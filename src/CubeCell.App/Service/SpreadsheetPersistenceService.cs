using System.Collections.Generic;

using ClosedXML.Excel;

using CubeCell.App.Models;

namespace CubeCell.App.Service;

public class SpreadsheetPersistenceService : ISpreadsheetPersistenceService
{
    private readonly IReadOnlyCellStorage _cellStorage;

    public SpreadsheetPersistenceService(IReadOnlyCellStorage cellStorage)
    {
        _cellStorage = cellStorage;
    }

    public void CreateSpreadsheetAndSave(string filePath, string fileName)
    {
        using var workbook = new XLWorkbook();
        IXLWorksheet? worksheet = workbook.AddWorksheet("Sheet1");
        
        AddCellsFromCellStorageToWorksheet(worksheet);

        workbook.SaveAs(filePath);
    }

    public void UpdateExistingSpreadsheetAndSave(string filepath)
    {
        using var workbook = new XLWorkbook(filepath);

        IXLWorksheet worksheet;
        if (workbook.TryGetWorksheet("Sheet1", out IXLWorksheet? existingSheet))
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
        IReadOnlyDictionary<CellCoordinates, Cell> cells = _cellStorage.GetCells();
        
        foreach ((CellCoordinates key, Cell value) in cells)
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
