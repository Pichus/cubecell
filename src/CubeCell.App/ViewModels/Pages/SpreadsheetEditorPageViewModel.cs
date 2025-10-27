using System;
using System.Collections.Generic;
using System.Reactive;

using CubeCell.App.Models;
using CubeCell.App.Service;
using CubeCell.App.Utils;
using CubeCell.App.ViewModels.Abstractions;
using CubeCell.Parser;

using ReactiveUI;

namespace CubeCell.App.ViewModels.Pages;

public class SpreadsheetEditorPageViewModel : ViewModelBase, IRoutableViewModel
{
    private readonly DependencyGraph _dependencyGraph = new();
    private readonly Spreadsheet _spreadsheet = new();
    private readonly ISpreadsheetPersistenceService _spreadsheetPersistenceService;
    private readonly SpreadsheetViewModel _spreadsheetViewModel = new();

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

        _spreadsheetPersistenceService = new SpreadsheetPersistenceService(_spreadsheet);

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
        if (_lastSelectedCellCoordinates is null || _lastSelectedCell is null)
        {
            return;
        }

        CalculateCellFormula(_lastSelectedCellCoordinates.Value, _lastSelectedCell);
    }

    private void CalculateCellFormula(CellCoordinates cellCoordinates, CellViewModel cellViewModel)
    {
        Cell? cellModel = _spreadsheet.GetCell(cellCoordinates.Col, cellCoordinates.Row);

        if (cellModel is null || cellModel.Formula is null)
        {
            return;
        }

        if (cellModel.Formula.StartsWith("="))
        {
            var evaluator = new FormulaEvaluator(_spreadsheet);

            try
            {
                var calculatedValue = evaluator.Evaluate(cellModel.Formula);
                cellModel.Value = calculatedValue.ToString();
            }
            catch (Exception exception)
            {
                cellModel.Value = "error";
            }
        }


        var dependencyExtractor = new DependencyExtractor();

        var cellAddress = CellAddressUtils.CoordinatesToAddress(cellCoordinates);

        try
        {
            HashSet<string> dependencies = dependencyExtractor.ExtractDependencies(cellModel.Formula);
            RegisterDependencies(cellAddress, dependencies);
        }
        catch (Exception exception)
        {
            cellModel.Value = "error";
        }

        cellViewModel.Refresh();

        HashSet<string> dependants = _dependencyGraph.GetCellDependents(cellAddress);
        RecalculateDependents(dependants);
    }

    private void RecalculateDependents(HashSet<string> dependants)
    {
        foreach (var dependant in dependants)
        {
            CellViewModel? cellViewModel = _spreadsheetViewModel.GetCell(dependant);

            if (cellViewModel is null)
            {
                continue;
            }

            CellCoordinates cellCoordinates = CellAddressUtils.AddressToCoordinates(dependant);

            CalculateCellFormula(cellCoordinates, cellViewModel);
        }
    }

    private void RegisterDependencies(string dependentAddress, HashSet<string> dependencies)
    {
        foreach (var dependency in dependencies)
        {
            _dependencyGraph.AddDependency(dependentAddress, dependency);
        }
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

public record ExportAsRequest(string FilePath, string FileName);
