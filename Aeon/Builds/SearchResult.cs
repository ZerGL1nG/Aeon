﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Aeon.Core;

namespace Aeon.Builds;

internal class SearchResult
{
    private readonly Dictionary<Stats, SearchNode> _nodes = new();
    private readonly Queue<SearchNode> _pending = new();

    private readonly int _steps;
    public string hc;

    public SearchResult(string hc, SearchNode initnode)
    {
        hc = this.hc;
        _pending.Enqueue(initnode);
        while (_pending.Count > 0) {
            Step();
            ++_steps;
            if (_steps%1000 == 0) {
                Console.SetCursorPosition(0, Console.GetCursorPosition().Top);
                Console.Write($"Paths: {_steps,10} step, {_pending.Count,10} count, {_nodes.Count,10} nodes");
            }
        }
        Console.SetCursorPosition(0, Console.GetCursorPosition().Top);
        Console.Write($"Paths: {_steps,10} step, {_pending.Count,10} count, {_nodes.Count,10} nodes");
        Console.WriteLine();
    }

    public IReadOnlyDictionary<Stats, SearchNode> Nodes => _nodes;

    public void Step()
    {
        var node = _pending.Dequeue();
        if (!TryAddNode(node)) return;
        foreach (var newnode in node.Init()) _pending.Enqueue(newnode);
    }

    public List<SearchNode> ValuableNodes() => _nodes.Select(x => x.Value).Where(v => v.MoneyHere < 7).ToList();

    private bool TryAddNode(SearchNode node)
    {
        if (_nodes.TryGetValue(node.Stats, out var old)) {
            var x                     = IsReplBetter(old, node);
            if (x) _nodes[node.Stats] = node;
            return x;
        }
        return _nodes.TryAdd(node.Stats, node);
    }

    private static bool IsReplBetter(SearchNode old, SearchNode repl) => repl.MoneyHere > old.MoneyHere;

    public List<SearchNode> Condition(Func<SearchNode, bool> func) => _nodes.Select(x => x.Value).Where(func).ToList();

    public void Reset()
    {
        foreach (var nodesKey in _nodes.Keys) _nodes[nodesKey].matchups = new ConcurrentDictionary<string, NodeData>();
    }
}