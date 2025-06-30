using Components;
using GameSystems.Bids;

namespace GameSystems.AI
{
    public class AlwaysAcceptTrucoStrategy : ITrucoDecisionStrategy
    {
        public bool ShouldAcceptTruco(IBid currentBid, Player self, Player opponent)
            => true;

        public bool ShouldRaiseTruco(IBid currentBid, Player self, Player opponent)
            => false;
    }
}
