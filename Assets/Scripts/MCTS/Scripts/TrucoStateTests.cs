using GameSystems.Bids;
using NUnit.Framework;

public class TrucoStateTests
{
    [Test]
    public void ResolveRound_PlayerLoses_All3Bazas()
    {
        var bf    = new BidFactory();
        var aiHand = new[] { 20, 21, 22 };
        var state = new TrucoGameState(aiHand, bf);
        state.cardsInPlayerHand = new[] { 0, 1, 2 };

        // forzamos las 3 bazas sin pasar por ApplyAction (llamamos directo)
        state.playerCardsPlayed = new[] { 0, 1, 2 };
        state.aiCardsPlayed     = new[] { 20, 21, 22 };
        state.ResolveRound();

        Assert.IsTrue(state.IsTerminal);
        Assert.AreEqual(1, state.WinningPlayer);
        Assert.AreEqual(-state.trucoLevel, state.trucoResult);
    }
}
