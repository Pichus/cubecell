using System;
using System.Collections.ObjectModel;
using System.Reactive;

using CubeCell.App.Models;
using CubeCell.App.Utils;
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

        // _formulaEvaluator =
        //     new FormulaEvaluator(cellAddress => Cells[GetListIndexFromCellAddress(cellAddress, _colCount)].Value);

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
            ColumnHeaders.Add(CellAddressUtils.ColumnIndexToLetters(c));
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
}
