namespace Match.Bids
{
    public class FaltaEnvidoBid : IBid
    {
        public int PointValue => 8;
        public IBid Next => null;
        public string Name => "Falta Envido";
        
        public bool CanBid(TurnManager ctx)
        {
            return ctx.bazaCount == 0;
        }
    }
}