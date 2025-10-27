using System.Collections.Generic;

namespace CubeCell.App.Models;

public class DependencyGraph
{
    private readonly Dictionary<string, HashSet<string>> _dependants = new();
    private readonly Dictionary<string, HashSet<string>> _dependencies = new();

    public bool TryAddDependency(string dependantAddress, string dependencyAddress)
    {
        if (dependantAddress == dependencyAddress)
        {
            return false;
        }

        _dependencies.TryAdd(dependantAddress, new HashSet<string>());
        _dependencies[dependantAddress].Add(dependencyAddress);

        _dependants.TryAdd(dependencyAddress, new HashSet<string>());
        _dependants[dependencyAddress].Add(dependantAddress);

        if (HasCycle(dependantAddress))
        {
            RemoveDependency(dependantAddress, dependencyAddress);
            return false;
        }

        return true;
    }

    public void ClearDependencies(string dependantAddress)
    {
        HashSet<string> dependencies = GetCellDependencies(dependantAddress);

        if (dependencies.Count == 0)
        {
            return;
        }

        foreach (string dependency in dependencies)
        {
            RemoveDependency(dependantAddress, dependency);
        }
    }

    public void RemoveDependency(string dependantAddress, string dependencyAddress)
    {
        GetCellDependencies(dependantAddress).Remove(dependencyAddress);
        GetCellDependants(dependencyAddress).Remove(dependantAddress);
    }

    public bool TrySetDependencies(string dependantAddress, HashSet<string> dependencies)
    {
        _dependencies[dependantAddress] = dependencies;

        foreach (string dependency in dependencies)
        {
            _dependants.TryAdd(dependency, new HashSet<string>());
            _dependants[dependency].Add(dependantAddress);
        }

        if (HasCycle(dependantAddress))
        {
            _dependencies.Remove(dependantAddress);
            foreach (string dep in dependencies)
            {
                _dependants[dep].Remove(dependantAddress);
            }

            return false;
        }

        return true;
    }

    public HashSet<string> GetCellDependencies(string cellAddress)
    {
        return _dependencies.GetValueOrDefault(cellAddress) ?? [];
    }

    public HashSet<string> GetCellDependants(string cellAddress)
    {
        return _dependants.GetValueOrDefault(cellAddress) ?? [];
    }

    private bool HasCycle(string start)
    {
        HashSet<string> visited = new();
        HashSet<string> recursionStack = new();

        bool Dfs(string node)
        {
            if (!visited.Add(node))
            {
                return false;
            }

            recursionStack.Add(node);

            foreach (string dep in _dependencies.GetValueOrDefault(node) ?? [])
            {
                if (recursionStack.Contains(dep))
                {
                    return true;
                }

                if (Dfs(dep))
                {
                    return true;
                }
            }

            recursionStack.Remove(node);
            return false;
        }

        return Dfs(start);
    }
}
