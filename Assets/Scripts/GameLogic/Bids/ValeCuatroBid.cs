namespace Match.Bids
{
    public class ValeCuatroBid : IBid
    {
        public int PointValue => 4;
        public IBid Next => null;
        public string Name => "Vale Cuatro";
        
        public bool CanBid(TurnManager ctx)
        {
            return ctx.activePlayer == 0;
        }
    }
}