using System.Collections.Generic;

using CubeCell.App.Models;
using CubeCell.App.Utils;
using CubeCell.Parser;

namespace CubeCell.App.Services;

public class CellCalculationService
{
    private readonly ICellStorage _cellStorage;
    private readonly DependencyExtractor _dependencyExtractor;
    private readonly DependencyGraph _dependencyGraph;
    private readonly FormulaCalculator _formulaCalculator;

    public CellCalculationService(ICellStorage cellStorage, DependencyGraph dependencyGraph,
        FormulaCalculator formulaCalculator, DependencyExtractor dependencyExtractor)
    {
        _cellStorage = cellStorage;
        _dependencyGraph = dependencyGraph;
        _formulaCalculator = formulaCalculator;
        _dependencyExtractor = dependencyExtractor;
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
            _cellStorage.UpdateCell(cellCoordinates, cellModel, "#ERROR");
            return;
        }

        _cellStorage.UpdateCell(cellCoordinates, cellModel, _formulaCalculator.Calculate(cellModel.Formula));

        HashSet<string> dependants = _dependencyGraph.GetCellDependants(cellAddress);

        CalculateAndRerenderDependants(dependants);
    }

    private void CalculateAndRerenderDependants(HashSet<string> dependants)
    {
        foreach (string dependant in dependants)
        {
            CellCoordinates cellCoordinates = CellAddressUtils.AddressToCoordinates(dependant);

            CalculateAndRerenderCell(cellCoordinates);
        }
    }
}
