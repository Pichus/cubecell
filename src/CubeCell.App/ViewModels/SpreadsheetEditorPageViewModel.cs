using System;
using System.Collections.ObjectModel;
using System.Reactive;
using CubeCell.App.Models;
using CubeCell.Parser;
using ReactiveUI;

namespace CubeCell.App.ViewModels;

public class SpreadsheetEditorPageViewModel : ViewModelBase, IRoutableViewModel
{
    private readonly FormulaEvaluator _formulaEvaluator = new();
    private int _colCount;
    private string _formulaText = string.Empty;
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

        for (var c = 0; c < ColCount; c++)
            ColumnHeaders.Add(GetExcelColumnName(c));

        for (var r = 1; r <= RowCount; r++)
            RowHeaders.Add(r);

        for (var r = 0; r < RowCount; r++)
        for (var c = 0; c < ColCount; c++)
            Cells.Add(new CellModel
            {
                Formula = string.Empty, Value = string.Empty, DisplayText = string.Empty
            });
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

    public string FormulaText
    {
        get => _formulaText;
        set => this.RaiseAndSetIfChanged(ref _formulaText, value);
    }

    public string? UrlPathSegment { get; } = "spreadsheetEditor";
    public IScreen HostScreen { get; }

    private void CalculateCellFormula(CellModel cell)
    {
        if (cell.Formula is null || !cell.Formula.StartsWith("="))
            return;

        var calculated = _formulaEvaluator.Evaluate(cell.Formula).ToString();
        Console.WriteLine(calculated);
        cell.Value = calculated;
    }

    private static string GetExcelColumnName(int index)
    {
        // Excel columns are 1-based (A = 1)
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