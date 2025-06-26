// IGameState.cs
using System.Collections.Generic;

public interface IGameState<TAction>
{
    /// <summary>Quién tiene el turno actual (0 o 1).</summary>
    int CurrentPlayer { get; }

    /// <summary>Lista de acciones legales desde este estado.</summary>
    List<TAction> GetLegalActions();

    /// <summary>Devuelve un nuevo estado tras aplicar la acción.</summary>
    IGameState<TAction> ApplyAction(TAction action);

    /// <summary>True si el juego llegó a un estado terminal.</summary>
    bool IsTerminal { get; }

    /// <summary>
    /// Resultado de la partida desde la perspectiva de player: 
    /// 1 para victoria, 0 para empate, -1 para derrota.
    /// </summary>
    float GetResult(int forPlayer);
}