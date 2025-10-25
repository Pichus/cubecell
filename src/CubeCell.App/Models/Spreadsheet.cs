using System;
using System.Collections.Generic;

namespace CubeCell.App.Models;

public class Spreadsheet
{
    private readonly Dictionary<(int col, int row), Cell> _cells = new();
    private readonly Dictionary<string, HashSet<string>> _dependencies = new();
    private readonly Dictionary<string, HashSet<string>> _dependents = new();

    public bool TryAddCell((int col, int row) coordinates, Cell cell)
    {
        return _cells.TryAdd(coordinates, cell);
    }
    
    public void AddDependency(string dependentAddress, string dependencyAddress)
    {
        if (!_dependencies.TryGetValue(dependentAddress, out HashSet<string>? dependenciesSet))
        {
            dependenciesSet = _dependencies[dependentAddress] = new HashSet<string>();
        }

        dependenciesSet.Add(dependencyAddress);
        
        if (!_dependents.TryGetValue(dependencyAddress, out HashSet<string>? dependentsSet))
        {
            dependentsSet = _dependents[dependencyAddress] = new HashSet<string>();
        }

        dependentsSet.Add(dependentAddress);
    }

    public Cell? GetCell(int col, int row)
    {
        _cells.TryGetValue((col, row), out Cell? cell);
        
        return cell ?? null;
    }

    public Cell? GetCell(string address)
    {
        var (col, row) = CellAddressToCoordinates(address);
        return GetCell(col, row) ?? null;
    }

    private static (int row, int col) CellAddressToCoordinates(string address)
    {
        var i = 0;
        var col = 0;

        while (i < address.Length && char.IsLetter(address[i]))
        {
            col = (col * 26) + (char.ToUpper(address[i]) - 'A') + 1;
            i++;
        }

        col--;

        var row = int.Parse(address[i..]) - 1;

        return (row, col);
    }
}
