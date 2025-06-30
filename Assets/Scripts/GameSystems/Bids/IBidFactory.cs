namespace GameSystems.Bids
{
    public interface IBidFactory
    {
        IBid CreateBid(BidType type);
    }
}