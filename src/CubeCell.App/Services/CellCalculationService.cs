using System.Collections.Generic;

using CubeCell.App.Models;
using CubeCell.App.Utils;
using CubeCell.App.ViewModels;
using CubeCell.Parser;

namespace CubeCell.App.Services;

public class CellCalculationService
{
    private readonly ICellStorage _cellStorage;
    private readonly DependencyExtractor _dependencyExtractor;
    private readonly DependencyGraph _dependencyGraph;
    private readonly FormulaCalculator _formulaCalculator;
    private readonly SpreadsheetViewModel _spreadsheetViewModel;

    public CellCalculationService(ICellStorage cellStorage, DependencyGraph dependencyGraph,
        FormulaCalculator formulaCalculator, DependencyExtractor dependencyExtractor,
        SpreadsheetViewModel spreadsheetViewModel)
    {
        _cellStorage = cellStorage;
        _dependencyGraph = dependencyGraph;
        _formulaCalculator = formulaCalculator;
        _dependencyExtractor = dependencyExtractor;
        _spreadsheetViewModel = spreadsheetViewModel;
    }

    public void CalculateAndRerenderCell(CellCoordinates cellCoordinates)
    {
        Cell? cellModel = _cellStorage.GetCell(cellCoordinates.Col, cellCoordinates.Row);

        if (cellModel is null || cellModel.Formula is null || !cellModel.Formula.StartsWith("="))
        {
            return;
        }

        string cellAddress = CellAddressUtils.CoordinatesToAddress(cellCoordinates);
        HashSet<string> dependencies = _dependencyExtractor.ExtractDependencies(cellModel.Formula);

        _dependencyGraph.ClearDependencies(cellAddress);
        if (!_dependencyGraph.TrySetDependencies(cellAddress, dependencies))
        {
            cellModel.Value = "#ERROR";
            return;
        }

        cellModel.Value = _formulaCalculator.Calculate(cellModel.Formula);

        RerenderViewModel(cellCoordinates);
    }

    public void CalculateAndRerenderDependants(CellCoordinates cellCoordinates)
    {
        HashSet<string> dependants =
            _dependencyGraph.GetCellDependants(CellAddressUtils.CoordinatesToAddress(cellCoordinates));

        foreach (string dependant in dependants)
        {
            CellCoordinates dependantCoordinates = CellAddressUtils.AddressToCoordinates(dependant);

            CalculateAndRerenderCell(dependantCoordinates);
        }
    }

    private void RerenderViewModel(CellCoordinates cellCoordinates)
    {
        CellViewModel? viewModel = _spreadsheetViewModel.GetCell(cellCoordinates.Col, cellCoordinates.Row);

        if (viewModel is null)
        {
            return;
        }

        viewModel.Refresh();
    }
}
