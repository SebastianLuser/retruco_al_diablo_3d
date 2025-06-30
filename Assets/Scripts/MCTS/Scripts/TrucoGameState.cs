using System;
using System.Collections.Generic;
using System.Linq;
using GameSystems.Bids;

public enum TrucoAction
{
    PlayCard,
    CallEnvido,
    CallFaltaEnvido,
    CallRealEnvido,
    AcceptEnvido,
    RejectEnvido,
    CallTruco,
    CallRetruco,
    CallValeCuatro,
    AcceptTruco,
    RejectTruco,
    Fold,
    PlayActive
}

public struct ActionNode
{
    public TrucoAction Type;
    public int Param; // índice de carta, activo, o valor de apuesta según el tipo

    public ActionNode(TrucoAction type, int param = -1)
    {
        Type = type;
        Param = param;
    }
}

public enum CardSuit
{
    Espadas,
    Bastos,
    Oros,
    Copas
}

public struct CardData
{
    public CardSuit Suit;
    public int Number; //Card Data

    public CardData(CardSuit suit, int number)
    {
        Suit = suit;
        Number = number;
    }
}

public class TrucoGameState : IGameState<ActionNode>
{
    private static readonly CardData[] deckData = new CardData[]
    {
        // Oros
        new CardData { Suit = CardSuit.Oros, Number = 1 },
        new CardData { Suit = CardSuit.Oros, Number = 2 },
        new CardData { Suit = CardSuit.Oros, Number = 3 },
        new CardData { Suit = CardSuit.Oros, Number = 4 },
        new CardData { Suit = CardSuit.Oros, Number = 5 },
        new CardData { Suit = CardSuit.Oros, Number = 6 },
        new CardData { Suit = CardSuit.Oros, Number = 7 },
        new CardData { Suit = CardSuit.Oros, Number = 10 },
        new CardData { Suit = CardSuit.Oros, Number = 11 },
        new CardData { Suit = CardSuit.Oros, Number = 12 },

        // Copas
        new CardData { Suit = CardSuit.Copas, Number = 1 },
        new CardData { Suit = CardSuit.Copas, Number = 2 },
        new CardData { Suit = CardSuit.Copas, Number = 3 },
        new CardData { Suit = CardSuit.Copas, Number = 4 },
        new CardData { Suit = CardSuit.Copas, Number = 5 },
        new CardData { Suit = CardSuit.Copas, Number = 6 },
        new CardData { Suit = CardSuit.Copas, Number = 7 },
        new CardData { Suit = CardSuit.Copas, Number = 10 },
        new CardData { Suit = CardSuit.Copas, Number = 11 },
        new CardData { Suit = CardSuit.Copas, Number = 12 },

        // Espadas
        new CardData { Suit = CardSuit.Espadas, Number = 1 },
        new CardData { Suit = CardSuit.Espadas, Number = 2 },
        new CardData { Suit = CardSuit.Espadas, Number = 3 },
        new CardData { Suit = CardSuit.Espadas, Number = 4 },
        new CardData { Suit = CardSuit.Espadas, Number = 5 },
        new CardData { Suit = CardSuit.Espadas, Number = 6 },
        new CardData { Suit = CardSuit.Espadas, Number = 7 },
        new CardData { Suit = CardSuit.Espadas, Number = 10 },
        new CardData { Suit = CardSuit.Espadas, Number = 11 },
        new CardData { Suit = CardSuit.Espadas, Number = 12 },

        // Bastos
        new CardData { Suit = CardSuit.Bastos, Number = 1 },
        new CardData { Suit = CardSuit.Bastos, Number = 2 },
        new CardData { Suit = CardSuit.Bastos, Number = 3 },
        new CardData { Suit = CardSuit.Bastos, Number = 4 },
        new CardData { Suit = CardSuit.Bastos, Number = 5 },
        new CardData { Suit = CardSuit.Bastos, Number = 6 },
        new CardData { Suit = CardSuit.Bastos, Number = 7 },
        new CardData { Suit = CardSuit.Bastos, Number = 10 },
        new CardData { Suit = CardSuit.Bastos, Number = 11 },
        new CardData { Suit = CardSuit.Bastos, Number = 12 },
    };

    private static readonly int[] cardPower = new int[40]
    {
        // Oros (índices 0–9)
        5, // 1 de oros  
        7, // 2  
        8, // 3  
        0, // 4  
        0, // 5  
        0, // 6  
        9, // 7  
        0, // 10 
        0, // 11 
        0, // 12 

        // Copas (10–19)
        6, // 1 de copas  
        7, // 2  
        8, // 3  
        0, // 4  
        0, // 5  
        0, // 6  
        1, // 7  
        0, // 10 
        0, // 11 
        0, // 12 

        // Espadas (20–29)
        12, // 1  
        7, // 2  
        8, // 3  
        0, // 4  
        0, // 5  
        0, // 6  
        10, // 7  
        0, // 10 
        0, // 11 
        0, // 12 

        // Bastos (30–39)
        11, // 1  
        7, // 2  
        8, // 3  
        0, // 4  
        0, // 5  
        0, // 6  
        1, // 7  
        0, // 10 
        0, // 11 
        0 // 12 
    };

    //State
    public int CurrentPlayer { get; set; } //0=Player & 1=AI
    public int WinningPlayer { get; set; } //0=Player & 1=AI
    public bool IsTerminal { get; set; }

    // Cards / -1 Used / -2 Unknown
    public int[] cardsInAIHand { get; set; } = new int[3];
    public int[] cardsInPlayerHand { get; set; } = new int[3];

    public int[] playerCardsPlayed = new int[3];
    public int[] aiCardsPlayed = new int[3];

    public List<int> playerActives { get; set; } = new List<int>();
    public List<int> aiActives { get; set; } = new List<int>();

    public List<int> playerPassives { get; set; } = new List<int>();
    public List<int> aiPassives { get; set; } = new List<int>();

    public int actualHand { get; set; }

    public int trucoLevel { get; set; }
    public int trucoResult { get; set; }

    public int envidoLevel { get; set; }
    public int envidoResult { get; set; }
    public bool waitingEnvidoResponse { get; set; }
    public bool envidoPointsCount { get; set; }
    public bool waitingTrucoResponse { get; set; }
    public int otherHandPoints { get; set; }

    public BidFactory bidFactory { get; set; }
    public int StartingPlayer { get; set; }


    public TrucoGameState(TrucoGameState gameState, int cardPlayedIndex, int activePlayedIndex)
    {
        cardsInAIHand = gameState.cardsInAIHand;
        cardsInPlayerHand = gameState.cardsInPlayerHand;
        playerActives = gameState.playerActives;
        aiActives = gameState.aiActives;
        playerPassives = gameState.playerPassives;
        aiPassives = gameState.aiPassives;
        actualHand = gameState.actualHand;

        if (gameState.CurrentPlayer == 0)
        {
            cardsInPlayerHand[cardPlayedIndex] = -1;
            playerActives[activePlayedIndex] = -1;
        }
        else
        {
            cardsInAIHand[cardPlayedIndex] = -1;
            aiActives[activePlayedIndex] = -1;
        }

        CurrentPlayer = gameState.CurrentPlayer == 0 ? 1 : 0;
    }

    /// <summary>
    /// Crea un estado donde solo se conoce la mano de la IA.
    /// La mano del jugador queda [-1,-1,-1] para rellenar tú luego.
    /// </summary>
    public TrucoGameState(int[] aiHand, BidFactory bidFactory)
    {
        this.bidFactory = bidFactory;

        // 1) Asigno la mano de la IA (clonando el array)
        cardsInAIHand = (int[])aiHand.Clone();
        // 2) Mano del jugador desconcida (-1 = “no sé qué carta hay aquí”)
        cardsInPlayerHand = Enumerable.Repeat(-1, aiHand.Length).ToArray();

        // 3) Actives (por defecto todos disponibles; cámbialo si usas otro sistema)
        playerActives = new List<int> { 0, 1, 2 };
        aiActives = new List<int> { 0, 1, 2 };

        // 4) Arrays para registrar cartas jugadas en cada mano (3 manos máximo)
        playerCardsPlayed = Enumerable.Repeat(-1, 3).ToArray();
        aiCardsPlayed = Enumerable.Repeat(-1, 3).ToArray();

        // 5) Variables de control
        actualHand = 0;
        envidoLevel = 0;
        trucoLevel = 0;
        waitingEnvidoResponse = false;
        waitingTrucoResponse = false;
        envidoResult = 0;
        trucoResult = 0;
        IsTerminal = false;
        WinningPlayer = -1;

        // 6) Quién “corta” (empieza)
        StartingPlayer = 1; // por convención la IA corta
        CurrentPlayer = StartingPlayer;
    }

    public List<ActionNode> GetLegalActions()
    {
        var actions = new List<ActionNode>();
        var hand = CurrentPlayer == 0 ? cardsInPlayerHand : cardsInAIHand;
        var actives = CurrentPlayer == 0 ? playerActives : aiActives;

        if (waitingEnvidoResponse && envidoLevel > 0)
        {
            switch (envidoLevel)
            {
                case 1:
                    actions.Add(new ActionNode(TrucoAction.CallRealEnvido));
                    actions.Add(new ActionNode(TrucoAction.AcceptEnvido));
                    actions.Add(new ActionNode(TrucoAction.RejectEnvido));
                    break;
                case 2:
                    actions.Add(new ActionNode(TrucoAction.CallFaltaEnvido));
                    actions.Add(new ActionNode(TrucoAction.AcceptEnvido));
                    actions.Add(new ActionNode(TrucoAction.RejectEnvido));
                    break;
                case 3:
                    actions.Add(new ActionNode(TrucoAction.AcceptEnvido));
                    actions.Add(new ActionNode(TrucoAction.RejectEnvido));
                    break;
            }

            return actions;
        }

        if (waitingTrucoResponse && trucoLevel > 0)
        {
            switch (trucoLevel)
            {
                case 1:
                    actions.Add(new ActionNode(TrucoAction.CallRetruco));
                    actions.Add(new ActionNode(TrucoAction.AcceptTruco));
                    actions.Add(new ActionNode(TrucoAction.RejectTruco));
                    break;
                case 2:
                    actions.Add(new ActionNode(TrucoAction.CallValeCuatro));
                    actions.Add(new ActionNode(TrucoAction.AcceptTruco));
                    actions.Add(new ActionNode(TrucoAction.RejectTruco));
                    break;
                case 3:
                    actions.Add(new ActionNode(TrucoAction.AcceptTruco));
                    actions.Add(new ActionNode(TrucoAction.RejectTruco));
                    break;
            }

            return actions;
        }

        for (var i = 0; i < hand.Length; i++)
        {
            if (hand[i] != -1)
            {
                actions.Add(new ActionNode(TrucoAction.PlayCard, i));
            }
        }


        for (var i = 0; i < actives.Count; i++)
        {
            if (actives[i] != -1)
            {
                actions.Add(new ActionNode(TrucoAction.PlayActive, i));
            }
        }

        if (actualHand < 2 && envidoResult == 0)
        {
            actions.Add(new ActionNode(TrucoAction.CallEnvido));
        }

        if (trucoResult == 0 && (envidoLevel == 0 || envidoResult > 0))
            actions.Add(new ActionNode(TrucoAction.CallTruco));

        actions.Add(new ActionNode(TrucoAction.Fold));

        return actions;
    }

    public IGameState<ActionNode> ApplyAction(ActionNode action)
    {
        var copy = Clone();

        switch (action.Type)
        {
            case TrucoAction.PlayActive:
                // Consumir el active
                if (copy.CurrentPlayer == 0)
                    copy.playerActives[action.Param] = -1;
                else
                    copy.aiActives[action.Param] = -1;
                break;

            case TrucoAction.PlayCard:
                int baza = copy.actualHand / 2;
                // Registrar carta jugada en la mano actual
                if (copy.CurrentPlayer == 0)
                {
                    copy.playerCardsPlayed[baza] = copy.cardsInPlayerHand[action.Param];
                    copy.cardsInPlayerHand[action.Param] = -1;
                }
                else
                {
                    copy.aiCardsPlayed[baza] = copy.cardsInAIHand[action.Param];
                    copy.cardsInAIHand[action.Param] = -1;
                }

                copy.actualHand++;

                if (copy.actualHand >= 6)
                    copy.ResolveRound();

                break;

            case TrucoAction.CallEnvido:
            case TrucoAction.CallRealEnvido:
            case TrucoAction.CallFaltaEnvido:
                copy.envidoLevel++;
                copy.waitingEnvidoResponse = true;
                break;

            case TrucoAction.AcceptEnvido:
                if (copy.waitingEnvidoResponse)
                {
                    // Ambos aceptaron: calcular puntajes y ajustar envidoResult
                    var playerScore = CalculateEnvido(copy.cardsInPlayerHand);
                    var aiScore = CalculateEnvido(copy.cardsInAIHand);
                    var points = bidFactory.CreateBid(
                        copy.envidoLevel == 1 ? BidType.Envido :
                        copy.envidoLevel == 2 ? BidType.RealEnvido :
                        BidType.FaltaEnvido
                    ).PointValue;

                    if (playerScore > aiScore) copy.envidoResult += points;
                    else copy.envidoResult -= points;

                    copy.waitingEnvidoResponse = false;
                }

                break;

            case TrucoAction.RejectEnvido:
                // Rechazar otorga puntos al oponente
                var bidPoints = copy.trucoResult += bidFactory.CreateBid(
                    copy.envidoLevel == 1 ? BidType.Envido :
                    copy.envidoLevel == 2 ? BidType.RealEnvido :
                    BidType.FaltaEnvido
                ).PointValue;
                ;
                if (copy.CurrentPlayer == 0) copy.envidoResult -= bidPoints;
                else copy.envidoResult += bidPoints;
                copy.waitingEnvidoResponse = false;
                break;

            case TrucoAction.CallTruco:
            case TrucoAction.CallRetruco:
            case TrucoAction.CallValeCuatro:
                copy.trucoLevel++;
                copy.waitingTrucoResponse = true;
                break;

            case TrucoAction.AcceptTruco:
                // Similar a Envido, usar bidFactory para puntaje de Truco…
                copy.trucoResult += bidFactory.CreateBid(
                    copy.trucoLevel == 1 ? BidType.Truco :
                    copy.trucoLevel == 2 ? BidType.ReTruco :
                    BidType.ValeCuatro
                ).PointValue;
                copy.waitingTrucoResponse = false;
                break;

            case TrucoAction.RejectTruco:
                // Rechazar Truco da puntos al otro y termina la mano
                copy.trucoResult += copy.trucoLevel == 1
                    ? 1
                    : bidFactory.CreateBid(BidType.Truco).PointValue;
                copy.IsTerminal = true;
                copy.WinningPlayer = copy.CurrentPlayer == 0 ? 1 : 0;
                break;

            case TrucoAction.Fold:
                // Al retirarse, el otro gana
                
                var foldPoints = copy.trucoLevel > 0 ?bidFactory.CreateBid(
                    copy.trucoLevel == 1 ? BidType.Truco :
                    copy.trucoLevel == 2 ? BidType.ReTruco :
                    BidType.ValeCuatro
                ).PointValue :
                        1;

                // 2. Ajustamos trucoResult para que net != 0
                if (copy.CurrentPlayer == 0)
                    copy.trucoResult -= foldPoints;   // el jugador abandona → IA suma puntos
                else
                    copy.trucoResult += foldPoints;   // IA abandona → jugador suma puntos
                
                copy.IsTerminal = true;
                copy.WinningPlayer = copy.CurrentPlayer == 0 ? 1 : 0;
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        // Cambiar turno (salvo que sea terminal)
        if (!copy.IsTerminal)
            copy.CurrentPlayer = 1 - copy.CurrentPlayer;

        return copy;
    }

    /// <summary>
    /// Calcula el puntaje de envido: busca dos cartas del mismo palo y suma sus valores +20;
    /// si no hay par, devuelve el valor más alto (cartas ≥10 valen 0).
    /// </summary>
    private int CalculateEnvido(int[] hand)
    {
        // Supongo que tienes acceso a una colección CardData[] llamada deckData
        // donde deckData[cardIndex].Type es el palo y deckData[cardIndex].Number el número.
        var bySuit = hand
            .Where(i => i >= 0)
            .GroupBy(i => deckData[i].Suit)
            .ToDictionary(g => g.Key, g => g.Select(i =>
                deckData[i].Number < 10 ? deckData[i].Number : 0
            ).ToList());

        int best = 0;
        foreach (var ranks in bySuit.Values)
        {
            if (ranks.Count >= 2)
            {
                var top2 = ranks.OrderByDescending(r => r).Take(2).Sum() + 20;
                best = Math.Max(best, top2);
            }
        }

        if (best == 0)
        {
            // Ningún par: tomo la carta más alta
            best = bySuit.Values
                .SelectMany(v => v)
                .DefaultIfEmpty(0)
                .Max();
        }

        return best;
    }

    public float GetResult(int forPlayer)
    {
        // 1) Calculamos la diferencia total de puntos desde la perspectiva del jugador 0:
        int net = trucoResult + envidoResult;

        // 2) Si ‘forPlayer’ es el jugador 1, invertimos el signo:
        if (forPlayer == 1) net = -net;

        // 3) Si hay diferencia, devolvemos la magnitud:
        if (net != 0)
            return net;

        // 4) Empate exacto → gana quien cortó:
        return forPlayer == StartingPlayer ? 1f : -1f;
    }

    private TrucoGameState Clone()
    {
        var copy = (TrucoGameState)this.MemberwiseClone();

        // Clonar arrays de manos y jugadas
        copy.cardsInPlayerHand = (int[])this.cardsInPlayerHand.Clone();
        copy.cardsInAIHand = (int[])this.cardsInAIHand.Clone();
        copy.playerCardsPlayed = (int[])this.playerCardsPlayed.Clone();
        copy.aiCardsPlayed = (int[])this.aiCardsPlayed.Clone();

        // Clonar listas de actives/passives
        copy.playerActives = new List<int>(this.playerActives);
        copy.aiActives = new List<int>(this.aiActives);
        copy.playerPassives = new List<int>(this.playerPassives);
        copy.aiPassives = new List<int>(this.aiPassives);

        // Los campos de valor (ints, bools) ya están clonados por MemberwiseClone
        return copy;
    }

    /// <summary>
    /// Compara dos cartas devolviendo +1 si a > b, –1 si a < b, 0 si igual.
    /// </summary>
    private int CompareCard(int a, int b)
    {
        int pa = cardPower[a];
        int pb = cardPower[b];
        if (pa > pb) return 1;
        if (pa < pb) return -1;
        return 0;
    }

    /// <summary>
    /// Recorre las 3 bazas, acumula bazas ganadas y decide ganador final de Truco.
    /// </summary>
    public void ResolveRound()
    {
        int playerWins = 0, aiWins = 0;

        for (int i = 0; i < 3; i++)
        {
            int cp = playerCardsPlayed[i];
            int ca = aiCardsPlayed[i];
            int cmp = CompareCard(cp, ca);

            if (cmp > 0) playerWins++;
            else if (cmp < 0) aiWins++;
            else
            {
                // Empate en la baza → gana quien cortó
                if (StartingPlayer == 0) playerWins++;
                else aiWins++;
            }
        }

        // Determinar ganador y puntaje de Truco
        if (playerWins >= 2)
        {
            WinningPlayer = 0;
            trucoResult += trucoLevel;
        }
        else
        {
            WinningPlayer = 1;
            trucoResult -= trucoLevel;
        }

        IsTerminal = true;
    }
}