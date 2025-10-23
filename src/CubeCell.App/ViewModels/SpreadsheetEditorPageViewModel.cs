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
    private readonly FormulaEvaluator _formulaEvaluator = new();
    private int _colCount;
    private int _rowCount;

    private CellModel? _selectedCell;

    public SpreadsheetEditorPageViewModel(IScreen hostScreen, int rowCount, int colCount)
    {
        HostScreen = hostScreen;
        _rowCount = rowCount;
        ColCount = colCount;

        CalculateCellFormulaCommand = ReactiveCommand.Create((CellModel? cell = null) =>
        {
            if (cell is null || !cell.NeedsRecalculation)
                return;
            CalculateCellFormula(cell);
            cell.MarkAsCalculated();
        });

        CalculateSelectedCellFormulaCommand = ReactiveCommand.Create(() =>
        {
            if (_selectedCell is null || !_selectedCell.NeedsRecalculation)
                return;

            CalculateCellFormula(_selectedCell);
            _selectedCell.MarkAsCalculated();
        });

        InitializeColumnHeaders();

        InitializeRowHeaders();

        InitializeCells();
    }

    public ReactiveCommand<CellModel?, Unit> CalculateCellFormulaCommand { get; }
    public ReactiveCommand<Unit, Unit> CalculateSelectedCellFormulaCommand { get; }


    public CellModel? SelectedCell
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

    public ObservableCollection<CellModel> Cells { get; } = new();

    public string? UrlPathSegment { get; } = "spreadsheetEditor";
    public IScreen HostScreen { get; }

    private void InitializeRowHeaders()
    {
        for (var r = 1; r <= RowCount; r++)
            RowHeaders.Add(r);
    }

    private void InitializeColumnHeaders()
    {
        for (var c = 0; c < ColCount; c++)
            ColumnHeaders.Add(GetExcelColumnName(c));
    }

    private void InitializeCells()
    {
        for (var r = 0; r < RowCount; r++)
        for (var c = 0; c < ColCount; c++)
            Cells.Add(new CellModel
            {
                Formula = string.Empty, Value = string.Empty, DisplayText = string.Empty
            });
    }

    private void CalculateCellFormula(CellModel cell)
    {
        if (cell.Formula is null || !IsFormula(cell.Formula))
            return;

        var calculated = _formulaEvaluator.Evaluate(cell.Formula).ToString();
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
        var columnName = string.Empty;

        while (index > 0)
        {
            var remainder = (index - 1) % 26;
            columnName = (char)('A' + remainder) + columnName;
            index = (index - 1) / 26;
        }

        return columnName;
    }
}