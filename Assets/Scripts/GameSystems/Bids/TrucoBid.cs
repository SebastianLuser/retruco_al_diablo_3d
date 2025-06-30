using Services;

namespace GameSystems.Bids
{
    public class TrucoBid : IBid
    {
        public int PointValue => 2;
        public IBid Next { get; } = new ReTrucoBid();
        public string Name => "Truco";
        
        public bool CanBid(TurnManager ctx)
        {
            return ctx.activePlayer == 0;
        }
    }
}