using System;

namespace GameSystems.Bids
{
    public enum BidType
    {
        Truco, ReTruco, ValeCuatro,
        Envido, RealEnvido, FaltaEnvido,
    }

    public class BidFactory : IBidFactory
    {
        public IBid CreateBid(BidType type)
        {
            switch(type)
            {
                case BidType.Truco:       return new TrucoBid();
                case BidType.ReTruco:     return new ReTrucoBid();
                case BidType.ValeCuatro:  return new ValeCuatroBid();
                case BidType.Envido:      return new EnvidoBid();
                case BidType.RealEnvido:  return new RealEnvidoBid();
                case BidType.FaltaEnvido: return new FaltaEnvidoBid();
                default: throw new ArgumentException();
            }
        }
    }
}