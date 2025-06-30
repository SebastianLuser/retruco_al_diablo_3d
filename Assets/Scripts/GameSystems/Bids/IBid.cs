using Services;

namespace GameSystems.Bids
{
    public interface IBid
    {
        int PointValue { get; }
        IBid Next { get; }
        string Name { get; }
        bool CanBid(TurnManager context);
    }
}