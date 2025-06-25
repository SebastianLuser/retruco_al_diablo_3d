namespace Match.Bids
{
    public class EnvidoBid : IBid
    {
        public int PointValue => 2;
        public IBid Next { get; } = new RealEnvidoBid();
        public string Name => "Envido";
        
        public bool CanBid(TurnManager ctx)
        {
            return ctx.bazaCount == 0;
        }
    }
}