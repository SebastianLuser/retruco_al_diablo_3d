#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;
using GameSystems.Bids;

public static class TrucoFullSimulationTest
{
    [MenuItem("Truco/Test Full MC Simulation")]
    public static void TestFullSimulation()
    {
        // 1) Barajar el mazo
        var rng  = new System.Random(42);
        var deck = Enumerable.Range(0, 40)
            .OrderBy(_ => rng.Next())
            .ToArray();

        // 2) Repartir 3 cartas a IA y 3 al jugador
        var aiHand     = deck.Take(3).ToArray();
        var playerHand = deck.Skip(3).Take(3).ToArray();

        // 3) Crear el estado inicial
        var bidFactory = new BidFactory();
        var state = new TrucoGameState(aiHand, bidFactory)
        {
            cardsInPlayerHand = playerHand
        };

        // 4) Inicializar MCTS y simular hasta terminal
        var mcts       = new MCTS<TrucoGameState, ActionNode>();
        const int its  = 200;
        while (!state.IsTerminal)
        {
            // Elige la mejor acción tras 'its' iteraciones
            var action = mcts.RunSearch(state, its); // RunSearch(...) :contentReference[oaicite:0]{index=0}
            state = (TrucoGameState)state.ApplyAction(action);
        }

        // 5) Mostrar resultados
        var netPoints = state.trucoResult + state.envidoResult;
        Debug.Log(
            $"[MC Simulation] Ganador: {(state.WinningPlayer == 1 ? "IA" : "Jugador")}\n" + $"Truco: {state.trucoResult}, Envido: {state.envidoResult}, Net: {netPoints}"
        );
    }
}
#endif