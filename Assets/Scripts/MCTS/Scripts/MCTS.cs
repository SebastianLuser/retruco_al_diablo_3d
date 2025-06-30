using System.Linq;
using GameSystems.Bids;

public class MCTS<TState, TAction> where TState : IGameState<TAction>
{
    private readonly System.Random _rng = new();
    private readonly float _explorationConstant;

    //Bid Factory  
    private BidFactory _bidFactory = new();

    public MCTS(float explorationConstant = 1.41f)
    {
        _explorationConstant = explorationConstant;
    }

    public TAction RunSearch(TState rootState, int iterations)
    {
        var root = new MCTSNode<TState, TAction>(rootState);

        for (var i = 0; i < iterations; i++)
        {
            // 1. SELECTION
            var node = root;
            while (!node.State.IsTerminal && node.IsFullyExpanded())
                node = node.BestChild(_explorationConstant);

            // 2. EXPANSION
            if (!node.State.IsTerminal)
            {
                var triedActions = node.Children.Select(c => c.ActionFromParent).ToHashSet();
                var legal = node.State.GetLegalActions().Where(a => !triedActions.Contains(a)).ToList();
                var action = legal[_rng.Next(legal.Count)];
                var nextState = (TState)node.State.ApplyAction(action);
                var child = new MCTSNode<TState, TAction>(nextState, node, action);
                node.Children.Add(child);
                node = child;
            }

            // 3. SIMULATION
            var rolloutResult = Simulate(node.State);

            // 4. BACKPROPAGATION
            while (node != null)
            {
                node.Visits++;
                node.Wins += rolloutResult; // asume +1 win, 0 empate, -1 pérdida
                node = node.Parent;
            }
        }

        // Elegir el hijo con más visitas
        var bestChild = root.Children.OrderByDescending(c => c.Visits).First();
        return bestChild.ActionFromParent;
    }
    
    private float Simulate(TState state)
    {
        var simState = state;
        while (!simState.IsTerminal)
        {
            var actions = simState.GetLegalActions();
            var a = actions[_rng.Next(actions.Count)];
            simState = (TState)simState.ApplyAction(a);
        }

        // Suponemos 2 jugadores: devolvemos el resultado para el jugador que inició la simulación
        return simState.GetResult(state.CurrentPlayer);
    }
}