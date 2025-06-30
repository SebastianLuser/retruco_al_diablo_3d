using Components;
using Services;

namespace GameSystems.AI
{
    public class AlwaysEnvidoStrategy : IEnvidoDecisionStrategy
    {
        public bool ShouldCallEnvido(TurnManager mgr)
        {
            return mgr.bazaCount == 0;
        }

        public bool ShouldAcceptEnvido(Player self, Player opponent)
        {
            return true;
        }
    }
}