namespace AI
{
    public interface IEnvidoDecisionStrategy
    {
        bool ShouldCallEnvido(TurnManager mgr);
        bool ShouldAcceptEnvido(Player self, Player opponent);
    }
}