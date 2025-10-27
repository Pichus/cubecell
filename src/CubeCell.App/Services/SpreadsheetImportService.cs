using System;

using ClosedXML.Excel;

using CubeCell.App.Models;

namespace CubeCell.App.Service;

public class SpreadsheetImportService : ISpreadsheetImportService
{
    public SpreadsheetSize LoadSpreadsheetFromFile(ICellWriter cellWriter, string filePath)
    {
        using var workbook = new XLWorkbook(filePath);
        IXLWorksheet? worksheet = workbook.Worksheet(1);

        foreach (IXLCell? xlCell in worksheet.CellsUsed())
        {
            var row = xlCell.Address.RowNumber - 1;
            var col = xlCell.Address.ColumnNumber - 1;

            var cell = new Cell();

            if (!string.IsNullOrEmpty(xlCell.FormulaA1) && xlCell.FormulaA1.StartsWith("="))
            {
                cell.Formula = xlCell.FormulaA1;
            }
            else
            {
                cell.Value = xlCell.GetValue<string>();
            }

            cellWriter.SetCell(new CellCoordinates(col, row), cell);
        }

        IXLRange? usedRange = worksheet.RangeUsed();

        if (usedRange is null)
        {
            throw new NullReferenceException("used range can't be null");
        }

        var usedRows = usedRange.RowCount();
        var usedCols = usedRange.ColumnCount();


        return new SpreadsheetSize(usedCols, usedRows);
    }
}
