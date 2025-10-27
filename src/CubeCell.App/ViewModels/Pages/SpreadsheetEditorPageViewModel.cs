using System.Reactive;

using CubeCell.App.Models;
using CubeCell.App.Services;
using CubeCell.App.ViewModels.Abstractions;
using CubeCell.Parser;

using ReactiveUI;

namespace CubeCell.App.ViewModels.Pages;

public class SpreadsheetEditorPageViewModel : ViewModelBase, IRoutableViewModel
{
    private readonly CellCalculationService _cellCalculationService;
    private readonly DependencyGraph _dependencyGraph = new();
    private readonly Spreadsheet _spreadsheet = new();
    private readonly ISpreadsheetPersistenceService _spreadsheetPersistenceService;
    private readonly SpreadsheetViewModel _spreadsheetViewModel;
    private int _colCount;

    private CellViewModel? _lastSelectedCell;
    private CellCoordinates? _lastSelectedCellCoordinates;
    private int _rowCount;
    private string _spreadSheetName = "";

    public SpreadsheetEditorPageViewModel(IScreen hostScreen, int rowCount, int colCount)
    {
        HostScreen = hostScreen;
        _rowCount = rowCount;
        ColCount = colCount;

        _spreadsheetViewModel = new SpreadsheetViewModel(_spreadsheet);
        _spreadsheetPersistenceService = new SpreadsheetPersistenceService(_spreadsheet);

        _cellCalculationService = new CellCalculationService(_spreadsheet, _dependencyGraph,
            new FormulaCalculator(new FormulaEvaluator(_spreadsheet)), new DependencyExtractor());

        AttachCellModelToCellViewModelCommand =
            ReactiveCommand.Create<CellCoordinates>(AttachCellModelToCellViewModelCommandHandler);
        CalculateCellFormulaCommand = ReactiveCommand.Create<CellCoordinates>(CalculateCellFormulaCommandHandler);
        UpdateLastSelectedCellCommand = ReactiveCommand.Create<CellCoordinates>(UpdateLastSelectedCellCommandHandler);
        CalculateSelectedCellFormulaCommand = ReactiveCommand.Create(CalculateSelectedCellFormulaCommandHandler);
        AttachCellModelToLastSelectedCellViewModelCommand =
            ReactiveCommand.Create(AttachCellModelToLastSelectedCellViewModelCommandHandler);
        SaveCommand = ReactiveCommand.Create(SaveCommandHandler);
        ExportAsCommand = ReactiveCommand.Create<ExportAsRequest>(ExportAsCommandHandler);
    }

    public SpreadsheetEditorPageViewModel(IScreen hostScreen, int rowCount, int colCount, Spreadsheet spreadsheet,
        string currentSpreadSheetFileLocation)
    {
        CurrentSpreadSheetFileLocation = currentSpreadSheetFileLocation;

        _spreadsheet = spreadsheet;

        HostScreen = hostScreen;
        _rowCount = rowCount;
        ColCount = colCount;

        _spreadsheetViewModel = new SpreadsheetViewModel(_spreadsheet);


        _spreadsheetPersistenceService = new SpreadsheetPersistenceService(_spreadsheet);

        InitializeSpreadsheet();

        AttachCellModelToCellViewModelCommand =
            ReactiveCommand.Create<CellCoordinates>(AttachCellModelToCellViewModelCommandHandler);
        CalculateCellFormulaCommand = ReactiveCommand.Create<CellCoordinates>(CalculateCellFormulaCommandHandler);
        UpdateLastSelectedCellCommand = ReactiveCommand.Create<CellCoordinates>(UpdateLastSelectedCellCommandHandler);
        CalculateSelectedCellFormulaCommand = ReactiveCommand.Create(CalculateSelectedCellFormulaCommandHandler);
        AttachCellModelToLastSelectedCellViewModelCommand =
            ReactiveCommand.Create(AttachCellModelToLastSelectedCellViewModelCommandHandler);
        SaveCommand = ReactiveCommand.Create(SaveCommandHandler);
        ExportAsCommand = ReactiveCommand.Create<ExportAsRequest>(ExportAsCommandHandler);
    }

    public string? CurrentSpreadSheetFileLocation { get; private set; }

    public string SpreadSheetName
    {
        get => _spreadSheetName;
        set => this.RaiseAndSetIfChanged(ref _spreadSheetName, value);
    }

    public ReactiveCommand<CellCoordinates, Unit> CalculateCellFormulaCommand { get; }
    public ReactiveCommand<Unit, Unit> CalculateSelectedCellFormulaCommand { get; }

    public ReactiveCommand<CellCoordinates, Unit> AttachCellModelToCellViewModelCommand { get; }
    public ReactiveCommand<Unit, Unit> AttachCellModelToLastSelectedCellViewModelCommand { get; }
    public ReactiveCommand<CellCoordinates, Unit> UpdateLastSelectedCellCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public ReactiveCommand<ExportAsRequest, Unit> ExportAsCommand { get; }
    public ReactiveCommand<Unit, IRoutableViewModel> ExitCommand => HostScreen.Router.NavigateBack;

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

    private void InitializeSpreadsheet()
    {
        foreach ((CellCoordinates key, Cell value) in _spreadsheet.GetCells())
        {
            Cell? cell = null;

            if (value.Formula is not null)
            {
                if (value.Formula.StartsWith("="))
                {
                    cell = new Cell { Formula = value.Formula, Value = "computed value" };
                }
                else
                {
                    cell = new Cell { Formula = value.Formula, Value = value.Formula };
                }
            }
            else if (value.Value is not null)
            {
                cell = new Cell { Formula = value.Value, Value = value.Value };
            }

            if (cell is not null)
            {
                _spreadsheet.SetCell(key, cell);
                _spreadsheetViewModel.SetCell(key, new CellViewModel(cell));
            }
        }
    }

    private void ExportAsCommandHandler(ExportAsRequest request)
    {
        _spreadsheetPersistenceService.CreateSpreadsheetAndSave(request.FilePath, request.FileName);
        CurrentSpreadSheetFileLocation = request.FilePath;
    }

    private void SaveCommandHandler()
    {
        if (CurrentSpreadSheetFileLocation is null)
        {
            return;
        }

        _spreadsheetPersistenceService.UpdateExistingSpreadsheetAndSave(CurrentSpreadSheetFileLocation);
    }

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
        if (_lastSelectedCellCoordinates is null)
        {
            return;
        }

        _cellCalculationService.CalculateAndRerenderCell(_lastSelectedCellCoordinates.Value);
    }

    private void UpdateLastSelectedCellCommandHandler(CellCoordinates cellCoordinates)
    {
        LastSelectedCellCoordinates = cellCoordinates;
        LastSelectedCell = _spreadsheetViewModel.GetCell(cellCoordinates.Col, cellCoordinates.Row);
    }

    private void CalculateCellFormulaCommandHandler(CellCoordinates cellCoordinates)
    {
        _cellCalculationService.CalculateAndRerenderCell(cellCoordinates);
    }

    private void AttachCellModelToCellViewModelCommandHandler(CellCoordinates cellCoordinates)
    {
        CellViewModel? cellViewModel = _spreadsheetViewModel.GetCell(cellCoordinates.Col, cellCoordinates.Row);

        if (cellViewModel is null || cellViewModel.HasCellModelAttached)
        {
            return;
        }

        AttachCellModelToCellViewModel(cellCoordinates, cellViewModel);
    }

    private void AttachCellModelToCellViewModel(CellCoordinates cellCoordinates, CellViewModel cellViewModel)
    {
        Cell cellModel = new() { Formula = cellViewModel.Value, Value = cellViewModel.Value };

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

public record ExportAsRequest(string FilePath, string FileName);
