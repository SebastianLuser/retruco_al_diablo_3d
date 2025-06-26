using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrucoAIController : MonoBehaviour
{
    public int iterations = 1000;
    private MCTS<TrucoGameState, ActionNode> _mcts;

    void Start()
    {
        _mcts = new MCTS<TrucoGameState, ActionNode>();
    }

    /// <summary>
    /// Llamar cuando sea el turno de la IA.
    /// </summary>
     public void MakeMove(TrucoGameState currentState)
    {
        // 1) Preparo el “mazo” de posibles cartas del jugador
        var fullDeck = Enumerable.Range(0, 40);
        var seen = currentState.cardsInAIHand
            .Concat(currentState.aiCardsPlayed)
            .Concat(currentState.playerCardsPlayed)
            .Where(i => i >= 0);
        var remaining = fullDeck.Except(seen).ToArray();

        var unknownCount = currentState.cardsInPlayerHand.Count(c => c == -1);
        var guesses = Combinaciones(remaining, unknownCount);

        // 2) Para cada hipótesis de mano, instancio un estado raíz y corrijo las cartas del jugador
        var vote = new Dictionary<ActionNode, int>();
        foreach (var guess in guesses)
        {
            var root = new TrucoGameState(currentState.cardsInAIHand.ToArray(), currentState.bidFactory)
                {
                    // Relleno la mano del jugador con este guess
                    cardsInPlayerHand = guess.ToArray(),
                    // Copio también las jugadas previas
                    playerCardsPlayed = currentState.playerCardsPlayed.ToArray(),
                    aiCardsPlayed = currentState.aiCardsPlayed.ToArray(),
                    actualHand = currentState.actualHand,
                    trucoLevel = currentState.trucoLevel,
                    envidoLevel = currentState.envidoLevel,
                    waitingEnvidoResponse = currentState.waitingEnvidoResponse,
                    waitingTrucoResponse = currentState.waitingTrucoResponse,
                    trucoResult = currentState.trucoResult,
                    envidoResult = currentState.envidoResult,
                    CurrentPlayer = currentState.CurrentPlayer
                };
            root.StartingPlayer = currentState.StartingPlayer;

            // 3) Corro MCTS y cuento votos
            var action = _mcts.RunSearch(root, iterations); // :contentReference[oaicite:0]{index=0}
            if (!vote.ContainsKey(action)) vote[action] = 0;
            vote[action]++;
        }

        // 4) Elijo la acción con más votos
        var bestAction = vote.OrderByDescending(kv => kv.Value).First().Key;

        // 5) La aplico en tu GameManager
        //GameManager.Instance.ApplyAIAction(bestAction);
    }

    IEnumerable<int[]> Combinaciones(int[] arr, int len)
    {
        if (len == 0) yield return new int[0];
        else
        {
            for (int i = 0; i <= arr.Length - len; i++)
            {
                foreach (var tail in Combinaciones(arr.Skip(i + 1).ToArray(), len - 1))
                {
                    yield return (new[] { arr[i] }).Concat(tail).ToArray();
                }
            }
        }
    }
}