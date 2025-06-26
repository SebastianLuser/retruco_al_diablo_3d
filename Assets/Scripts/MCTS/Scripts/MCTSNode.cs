using System.Collections.Generic;
using UnityEngine;

public class MCTSNode<TState, TAction>
    where TState : IGameState<TAction>
{
    public TState State;
    public MCTSNode<TState, TAction> Parent;
    public List<MCTSNode<TState, TAction>> Children = new();
    public TAction ActionFromParent;
    public int Visits;
    public float Wins;

    public MCTSNode(TState state, MCTSNode<TState, TAction> parent = null, TAction action = default)
    {
        State = state;
        Parent = parent;
        ActionFromParent = action;
    }

    public bool IsFullyExpanded()
        => Children.Count == State.GetLegalActions().Count;

    public MCTSNode<TState, TAction> BestChild(float c)
    {
        // UCT: winRate + c * sqrt(ln(N_parent) / N_child)
        MCTSNode<TState, TAction> best = null;
        var bestValue = float.MinValue;
        foreach (var child in Children)
        {
            var uct = (child.Wins / (child.Visits + 1e-6f))
                        + c * Mathf.Sqrt(Mathf.Log(Visits + 1) / (child.Visits + 1e-6f));
            
            if (uct > bestValue)
            {
                bestValue = uct;
                best = child;
            }
        }
        return best;
    }
}