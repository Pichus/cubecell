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

        if (HasCycle())
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

        if (HasCycle())
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

    public List<string>? GetTopologicalOrder()
    {
        var indegree = new Dictionary<string, int>();

        foreach (var node in _dependencies.Keys)
        {
            indegree[node] = _dependencies[node].Count;
        }

        var queue = new Queue<string>();
        var result = new List<string>();
        
        foreach (var (node, deg) in indegree)
        {
            if (deg == 0)
            {
                queue.Enqueue(node);
            }
        }

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            result.Add(node);

            foreach (var dependent in _dependants[node])
            {
                indegree[dependent]--;
                if (indegree[dependent] == 0)
                {
                    queue.Enqueue(dependent);
                }
            }
        }

        if (result.Count != indegree.Count)
        {
            return null;
        }

        return result;
    }
    
    private bool HasCycle()
    {
        var visited = new HashSet<string>();
        var stack = new HashSet<string>();

        foreach (var node in _dependencies.Keys)
        {
            if (DepthFirstSearch(node))
            {
                return true;
            }
        }

        return false;

        bool DepthFirstSearch(string node)
        {
            if (stack.Contains(node))
            {
                return true;
            }

            if (!visited.Add(node))
            {
                return false;
            }

            stack.Add(node);

            foreach (var dep in _dependencies[node])
            {
                if (DepthFirstSearch(dep))
                {
                    return true;
                }
            }

            stack.Remove(node);
            return false;
        }
    }
}
