using System.Reactive;

using CubeCell.App.Models;

using ReactiveUI;

namespace CubeCell.App.ViewModels;

public class SpreadsheetEditorPageViewModel : ViewModelBase, IRoutableViewModel
{
    private readonly Spreadsheet _spreadsheet = new();
    private readonly SpreadsheetViewModel _spreadsheetViewModel = new();

    private int _colCount;

    private CellViewModel? _lastSelectedCell;
    private CellCoordinates? _lastSelectedCellCoordinates;
    private int _rowCount;

    public SpreadsheetEditorPageViewModel(IScreen hostScreen, int rowCount, int colCount)
    {
        HostScreen = hostScreen;
        _rowCount = rowCount;
        ColCount = colCount;

        AttachCellModelToCellViewModelCommand =
            ReactiveCommand.Create<CellCoordinates>(AttachCellModelToCellViewModelCommandHandler);
        CalculateCellFormulaCommand = ReactiveCommand.Create<CellCoordinates>(CalculateCellFormulaCommandHandler);
        UpdateLastSelectedCellCommand = ReactiveCommand.Create<CellCoordinates>(UpdateLastSelectedCellCommandHandler);
        CalculateSelectedCellFormulaCommand = ReactiveCommand.Create(CalculateSelectedCellFormulaCommandHandler);
        AttachCellModelToLastSelectedCellViewModelCommand =
            ReactiveCommand.Create(AttachCellModelToLastSelectedCellViewModelCommandHandler);
    }

    public ReactiveCommand<CellCoordinates, Unit> CalculateCellFormulaCommand { get; }
    public ReactiveCommand<Unit, Unit> CalculateSelectedCellFormulaCommand { get; }

    public ReactiveCommand<CellCoordinates, Unit> AttachCellModelToCellViewModelCommand { get; }
    public ReactiveCommand<Unit, Unit> AttachCellModelToLastSelectedCellViewModelCommand { get; }
    public ReactiveCommand<CellCoordinates, Unit> UpdateLastSelectedCellCommand { get; }

    public CellViewModel? LastSelectedCell
    {
        get => _lastSelectedCell;
        private set => this.RaiseAndSetIfChanged(ref _lastSelectedCell, value);
    }

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

    public CellCoordinates? LastSelectedCellCoordinates
    {
        get => _lastSelectedCellCoordinates;
        set => this.RaiseAndSetIfChanged(ref _lastSelectedCellCoordinates, value);
    }

    public string? UrlPathSegment { get; } = "spreadsheetEditor";
    public IScreen HostScreen { get; }

    private void AttachCellModelToLastSelectedCellViewModelCommandHandler()
    {
        if (_lastSelectedCell is null || _lastSelectedCellCoordinates is null)
        {
            return;
        }
        
        AttachCellModelToCellViewModel(_lastSelectedCellCoordinates.Value, _lastSelectedCell);
    }

    private void CalculateSelectedCellFormulaCommandHandler()
    {
        if (_lastSelectedCellCoordinates is null || _lastSelectedCell is null)
        {
            return;
        }
        
        CalculateCellFormula(_lastSelectedCellCoordinates.Value, _lastSelectedCell);
    }

    private void CalculateCellFormula(CellCoordinates cellCoordinates, CellViewModel cellViewModel)
    {
        Cell? cellModel = _spreadsheet.GetCell(cellCoordinates.Col, cellCoordinates.Row);
        
        if (cellModel is null)
        {
            return;
        }
        
        // TESTING
        if (cellModel.Formula is null || !cellModel.Formula.StartsWith("="))
        {
            return;
        }

        cellModel.Value = "Calculated value";
        // TESTING

        cellViewModel.Refresh();
    }

    private void UpdateLastSelectedCellCommandHandler(CellCoordinates cellCoordinates)
    {
        LastSelectedCellCoordinates = cellCoordinates;
        LastSelectedCell = _spreadsheetViewModel.GetCell(cellCoordinates.Col, cellCoordinates.Row);
    }

    private void CalculateCellFormulaCommandHandler(CellCoordinates cellCoordinates)
    {
        CellViewModel? cellViewModel = _spreadsheetViewModel.GetCell(cellCoordinates.Col, cellCoordinates.Row);

        if (cellViewModel is null)
        {
            return;
        }

        CalculateCellFormula(cellCoordinates, cellViewModel);
    }

    private void AttachCellModelToCellViewModelCommandHandler(CellCoordinates cellCoordinates)
    {
        CellViewModel? cellViewModel = _spreadsheetViewModel.GetCell(cellCoordinates.Col, cellCoordinates.Row);

        if (cellViewModel is null)
        {
            return;
        }

        AttachCellModelToCellViewModel(cellCoordinates, cellViewModel);
    }

    private void AttachCellModelToCellViewModel(CellCoordinates cellCoordinates, CellViewModel cellViewModel)
    {
        var cellModel = new Cell { Formula = cellViewModel.Value, Value = cellViewModel.Value };

        _spreadsheet.SetCell(cellCoordinates, cellModel);
        cellViewModel.SetCellModel(cellModel);
        cellViewModel.Refresh();
    }
    

    public CellViewModel GetCellViewModelForBinding(CellCoordinates cellCoordinates)
    {
        CellViewModel? cellViewModel = _spreadsheetViewModel.GetCell(cellCoordinates.Col, cellCoordinates.Row);

        if (cellViewModel is null)
        {
            cellViewModel = new CellViewModel();
            _spreadsheetViewModel.SetCell(cellCoordinates, cellViewModel);
        }

        return cellViewModel;
    }
}
