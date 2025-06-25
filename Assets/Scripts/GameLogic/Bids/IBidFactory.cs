namespace Match.Bids
{
    public interface IBidFactory
    {
        IBid CreateBid(BidType type);
    }
}