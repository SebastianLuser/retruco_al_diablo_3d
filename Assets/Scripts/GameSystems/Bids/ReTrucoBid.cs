using Services;

namespace GameSystems.Bids
{
    public class ReTrucoBid : IBid
    {
        public int PointValue => 3;
        public IBid Next { get; } = new ValeCuatroBid();
        public string Name => "ReTruco";
        
        public bool CanBid(TurnManager ctx)
        {
            return ctx.activePlayer == 0;
        }
    }
}