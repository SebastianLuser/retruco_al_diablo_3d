namespace Match.Bids
{
    public class RealEnvidoBid : IBid
    {
        public int PointValue => 3;
        public IBid Next { get; } = new FaltaEnvidoBid();
        public string Name => "Real Envido";
        
        public bool CanBid(TurnManager ctx)
        {
            return ctx.bazaCount == 0;
        }
    }
}