using System;
using UnityEngine;

namespace Match.Bids
{
    public class BidValidator
    {
        private IBid currentBid;

        public bool CanBid(IBid candidate, TurnManager ctx)
        {
            if (candidate == null || ctx == null)
            {
                Debug.LogError("❌ BidValidator: candidate o context es null");
                return false;
            }

            if (!candidate.CanBid(ctx))
            {
                Debug.Log($"❌ {candidate.Name} no puede ser cantado en el contexto actual");
                return false;
            }

            if (ctx.TrucoCantado && IsEnvidoBid(candidate))
            {
                Debug.Log("❌ No se puede cantar Envido después de Truco");
                return false;
            }

            bool playerHasPlayedCard = ctx.lastPlayedCardPlayer != null;
            if (playerHasPlayedCard && IsEnvidoBid(candidate))
            {
                Debug.Log("❌ No se puede cantar Envido: el jugador ya jugó su primera carta");
                return false;
            }

            if (ctx.EnvidoCantado && IsEnvidoBid(candidate))
            {
                Debug.Log($"❌ No se puede cantar {candidate.Name}: Envido ya fue cantado esta ronda");
                return false;
            }

            return ValidateBidSequence(candidate);
        }

        private bool ValidateBidSequence(IBid candidate)
        {
            if (currentBid == null)
            {
                if (IsInitialBid(candidate))
                {
                    Debug.Log($"✅ Primera jugada, se permite cantar {candidate.Name}");
                    return true;
                }
                else
                {
                    Debug.Log($"❌ {candidate.Name} no es un bid inicial válido");
                    return false;
                }
            }

            if (IsEnvidoBid(currentBid) && IsEnvidoBid(candidate))
            {
                return ValidateEnvidoSequence(currentBid, candidate);
            }

            if (IsTrucoBid(currentBid) && IsTrucoBid(candidate))
            {
                return ValidateTrucoSequence(currentBid, candidate);
            }

            Debug.Log($"❌ No se puede cantar {candidate.Name} después de {currentBid.Name}");
            return false;
        }

        private bool ValidateEnvidoSequence(IBid current, IBid candidate)
        {
            if (current is EnvidoBid && candidate is RealEnvidoBid)
            {
                Debug.Log("✅ Envido → RealEnvido permitido");
                return true;
            }
            
            if (current is EnvidoBid && candidate is FaltaEnvidoBid)
            {
                Debug.Log("✅ Envido → FaltaEnvido permitido");
                return true;
            }

            if (current is RealEnvidoBid && candidate is FaltaEnvidoBid)
            {
                Debug.Log("✅ RealEnvido → FaltaEnvido permitido");
                return true;
            }

            Debug.Log($"❌ Secuencia de Envido no válida: {current.Name} → {candidate.Name}");
            return false;
        }

        private bool ValidateTrucoSequence(IBid current, IBid candidate)
        {
            if (current is TrucoBid && candidate is ReTrucoBid)
            {
                Debug.Log("✅ Truco → ReTruco permitido");
                return true;
            }

            if (current is ReTrucoBid && candidate is ValeCuatroBid)
            {
                Debug.Log("✅ ReTruco → ValeCuatro permitido");
                return true;
            }

            Debug.Log($"❌ Secuencia de Truco no válida: {current.Name} → {candidate.Name}");
            return false;
        }

        private bool IsInitialBid(IBid bid)
        {
            return bid is EnvidoBid || bid is TrucoBid;
        }

        private bool IsEnvidoBid(IBid bid)
        {
            return bid is EnvidoBid || bid is RealEnvidoBid || bid is FaltaEnvidoBid;
        }

        private bool IsTrucoBid(IBid bid)
        {
            return bid is TrucoBid || bid is ReTrucoBid || bid is ValeCuatroBid;
        }

        public void AcceptBid(IBid bid)
        {
            if (bid == null)
            {
                Debug.LogError("❌ BidValidator: No se puede aceptar bid null");
                return;
            }

            currentBid = bid;
            Debug.Log($"✅ Bid aceptado: {bid.Name}");

            if (IsEnvidoBid(bid))
            {
                TurnManager.Instance.MarcarEnvidoComoCantado();
            }

            if (IsTrucoBid(bid))
            {
                TurnManager.Instance.TrucoCantado = true;
                TurnManager.Instance.GameService.AcceptTruco(bid);
            }
        }

        public void Reset()
        {
            currentBid = null;
            Debug.Log("🔄 BidValidator reseteado");
        }

        public IBid GetCurrentBid() => currentBid;
        public bool HasActiveBid() => currentBid != null;
    }
}