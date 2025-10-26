using System.Collections.Generic;

namespace CubeCell.App.Models;

public class DependencyGraph
{
    private readonly Dictionary<string, HashSet<string>> _dependencies = new();
    private readonly Dictionary<string, HashSet<string>> _dependents = new();

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

    public HashSet<string> GetCellDependencies(string cellAddress)
    {
        if (!_dependencies.TryGetValue(cellAddress, out HashSet<string>? dependencies))
        {
            return [];
        }

        return dependencies;
    }

    public HashSet<string> GetCellDependents(string cellAddress)
    {
        if (!_dependents.TryGetValue(cellAddress, out HashSet<string>? dependents))
        {
            return [];
        }

        return dependents;
    }
}
