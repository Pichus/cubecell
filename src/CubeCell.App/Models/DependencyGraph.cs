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
        ClearDependencies(dependantAddress);
        
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
        // Build all nodes
        HashSet<string> allNodes = new(_dependencies.Keys);
        foreach ((string k, HashSet<string> deps) in _dependencies)
        foreach (string dep in deps)
        {
            allNodes.Add(dep);
        }

        foreach ((string k, HashSet<string> children) in _dependants)
        {
            allNodes.Add(k);
            foreach (string child in children)
            {
                allNodes.Add(child);
            }
        }

        // Indegrees (default 0)
        Dictionary<string, int> indegree = new();
        foreach (string n in allNodes)
        {
            indegree[n] = 0;
        }

        foreach ((string n, HashSet<string> deps) in _dependencies)
        {
            foreach (string d in deps)
            {
                indegree[d]++;
            }
        }

        Queue<string> q = new();
        foreach ((string n, int d) in indegree)
        {
            if (d == 0)
            {
                q.Enqueue(n);
            }
        }

        List<string> result = new();
        while (q.Count > 0)
        {
            string n = q.Dequeue();
            result.Add(n);

            if (_dependants.TryGetValue(n, out HashSet<string>? children))
            {
                foreach (string c in children)
                {
                    indegree[c]--;
                    if (indegree[c] == 0)
                    {
                        q.Enqueue(c);
                    }
                }
            }
        }

        if (result.Count != allNodes.Count)
        {
            return null;
        }

        return result;
    }


    private bool HasCycle()
    {
        // Gather all nodes: keys and values from both maps
        HashSet<string> allNodes = new(_dependencies.Keys);
        foreach ((string k, HashSet<string> deps) in _dependencies)
        foreach (string dep in deps)
        {
            allNodes.Add(dep);
        }

        foreach ((string k, HashSet<string> children) in _dependants)
        {
            allNodes.Add(k);
            foreach (string child in children)
            {
                allNodes.Add(child);
            }
        }

        HashSet<string> visited = new();
        HashSet<string> stack = new();

        foreach (string node in allNodes)
        {
            if (Dfs(node))
            {
                return true;
            }
        }

        return false;

        bool Dfs(string node)
        {
            if (visited.Contains(node))
            {
                return stack.Contains(node);
            }

            visited.Add(node);
            stack.Add(node);

            if (_dependencies.TryGetValue(node, out HashSet<string>? deps))
            {
                foreach (string dep in deps)
                {
                    if (Dfs(dep))
                    {
                        return true;
                    }
                }
            }

            stack.Remove(node);
            return false;
        }
    }
}
