using System.Collections.Generic;
using UnityEngine;
using Match.Bids;

namespace Match.Bids
{
    public class EnvidoManager
    {
        private int accumulatedPoints = 0;
        private List<BidType> calledBids = new List<BidType>();
        
        public void Reset()
        {
            accumulatedPoints = 0;
            calledBids.Clear();
            Debug.Log("🔄 EnvidoManager reseteado");
        }
        
        public void AddBid(BidType bidType)
        {
            calledBids.Add(bidType);
            
            switch (bidType)
            {
                case BidType.Envido:
                    accumulatedPoints += 2;
                    break;
                case BidType.RealEnvido:
                    accumulatedPoints += 3;
                    break;
                case BidType.FaltaEnvido:
                    accumulatedPoints += 8;
                    break;
            }
            
            Debug.Log($"🎯 {bidType} añadido. Total acumulado: {accumulatedPoints} puntos");
        }
        
        
        public int GetAccumulatedPoints() => accumulatedPoints;
        public List<BidType> GetCalledBids() => new List<BidType>(calledBids);
        public string GetBidsDescription() => string.Join(" + ", calledBids);
        public bool HasBids() => calledBids.Count > 0;
    }
}