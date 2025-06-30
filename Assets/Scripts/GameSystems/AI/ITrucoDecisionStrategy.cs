using Components;
using GameSystems.Bids;

namespace GameSystems.AI
{
    public interface ITrucoDecisionStrategy
    {
        bool ShouldAcceptTruco(IBid currentBid, Player self, Player opponent);
        bool ShouldRaiseTruco(IBid currentBid, Player self, Player opponent);
    }
}