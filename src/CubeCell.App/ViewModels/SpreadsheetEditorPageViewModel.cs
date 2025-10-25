using System;
using System.Collections.ObjectModel;
using System.Reactive;

using CubeCell.App.Models;
using CubeCell.Parser;

using ReactiveUI;

namespace CubeCell.App.ViewModels;

public class SpreadsheetEditorPageViewModel : ViewModelBase, IRoutableViewModel
{
    private const string FormulaPrefix = "=";
    private readonly FormulaEvaluator _formulaEvaluator;
    private int _colCount;
    private int _rowCount;

    private Cell? _selectedCell;

    public SpreadsheetEditorPageViewModel(IScreen hostScreen, int rowCount, int colCount)
    {
        HostScreen = hostScreen;
        _rowCount = rowCount;
        ColCount = colCount;

        _formulaEvaluator =
            new FormulaEvaluator(cellAddress => Cells[GetListIndexFromCellAddress(cellAddress, _colCount)].Value);

        CalculateCellFormulaCommand = ReactiveCommand.Create((Cell? cell = null) =>
        {
            if (cell is null || !cell.NeedsRecalculation)
            {
                return;
            }

            CalculateCellFormula(cell);
            cell.MarkAsCalculated();
        });

        CalculateSelectedCellFormulaCommand = ReactiveCommand.Create(() =>
        {
            if (_selectedCell is null || !_selectedCell.NeedsRecalculation)
            {
                return;
            }

            CalculateCellFormula(_selectedCell);
            _selectedCell.MarkAsCalculated();
        });

        InitializeColumnHeaders();

        InitializeRowHeaders();

        InitializeCells();
    }

    public ObservableCollection<Cell> Cells { get; } = new();

    public ReactiveCommand<Cell?, Unit> CalculateCellFormulaCommand { get; }
    public ReactiveCommand<Unit, Unit> CalculateSelectedCellFormulaCommand { get; }


    public Cell? SelectedCell
    {
        get => _selectedCell;
        set => this.RaiseAndSetIfChanged(ref _selectedCell, value);
    }

    public ObservableCollection<string> ColumnHeaders { get; } = new();
    public ObservableCollection<int> RowHeaders { get; } = new();

    public int RowCount
    {
        get => _rowCount;
        set => this.RaiseAndSetIfChanged(ref _rowCount, value);
    }

    public int ColCount
    {
        get => _colCount;
        set => this.RaiseAndSetIfChanged(ref _colCount, value);
    }

    public string? UrlPathSegment { get; } = "spreadsheetEditor";
    public IScreen HostScreen { get; }

    private void InitializeRowHeaders()
    {
        for (int r = 1; r <= RowCount; r++)
        {
            RowHeaders.Add(r);
        }
    }

    private void InitializeColumnHeaders()
    {
        for (int c = 0; c < ColCount; c++)
        {
            ColumnHeaders.Add(GetExcelColumnName(c));
        }
    }

    private void InitializeCells()
    {
        for (int r = 0; r < RowCount; r++)
        for (int c = 0; c < ColCount; c++)
        {
            Cells.Add(new Cell { Formula = string.Empty, Value = string.Empty, DisplayText = string.Empty });
        }
    }

    private void CalculateCellFormula(Cell cell)
    {
        if (cell.Formula is null || !IsFormula(cell.Formula))
        {
            return;
        }

        string? calculated = "";

        try
        {
            calculated = _formulaEvaluator.Evaluate(cell.Formula).ToString();
        }
        catch (Exception exception)
        {
            calculated = "ERROR";
        }

        Console.WriteLine(calculated); // for testing purposes
        cell.Value = calculated;
    }

    private bool IsFormula(string value)
    {
        return value.StartsWith(FormulaPrefix);
    }

    private static string GetExcelColumnName(int index)
    {
        index++;
        string columnName = string.Empty;

        while (index > 0)
        {
            int remainder = (index - 1) % 26;
            columnName = (char)('A' + remainder) + columnName;
            index = (index - 1) / 26;
        }

        return columnName;
    }

    public static int GetListIndexFromCellAddress(string cellAddress, int columnCount)
    {
        if (string.IsNullOrWhiteSpace(cellAddress))
        {
            throw new ArgumentException("Cell address cannot be null or empty.", nameof(cellAddress));
        }

        // Normalize: remove $ and uppercase
        cellAddress = cellAddress.Replace("$", "").ToUpperInvariant();

        // Split letters and digits
        int i = 0;
        while (i < cellAddress.Length && char.IsLetter(cellAddress[i]))
        {
            i++;
        }

        if (i == 0 || i == cellAddress.Length)
        {
            throw new ArgumentException($"Invalid cell address '{cellAddress}'.", nameof(cellAddress));
        }

        string columnPart = cellAddress[..i];
        string rowPart = cellAddress[i..];

        // Convert column letters to 0-based index (A → 0, Z → 25, AA → 26, etc.)
        int columnIndex = 0;
        foreach (char c in columnPart)
        {
            columnIndex = (columnIndex * 26) + (c - 'A') + 1;
        }

        columnIndex--;

        // Convert row digits to 0-based index
        if (!int.TryParse(rowPart, out int rowIndex))
        {
            throw new ArgumentException($"Invalid row number in address '{cellAddress}'.", nameof(cellAddress));
        }

        rowIndex--;

        // Linear index (row-major order)
        return (rowIndex * columnCount) + columnIndex;
    }
}
