using Components.Cards;
using UnityEngine;
using Services;
using GameSystems;
using GameSystems.Bids;
using StateMachines.Main;

namespace StateMachines.Play
{
    public class AwaitingEnvidoResponseState : IState
    {
        private TurnManager mgr;
        private BidType currentBidType;
        private EnvidoManager envidoManager;
        private IResponseService responseService;
        private int callerPlayerId;
        private int responderPlayerId;

        public AwaitingEnvidoResponseState(TurnManager mgr, BidType bidType, EnvidoManager envidoManager)
        {
            this.mgr = mgr;
            this.currentBidType = bidType;
            this.envidoManager = envidoManager;
            this.callerPlayerId = mgr.activePlayer;
            this.responderPlayerId = 1 - mgr.activePlayer;
        }

        public void Enter()
        {
            Debug.Log($"⏳ AWAITING: {(responderPlayerId == 0 ? "Jugador" : "IA")} debe responder a {currentBidType}");
            Debug.Log($"💰 Puntos acumulados hasta ahora: {envidoManager.GetAccumulatedPoints()}");
            
            CardClick.enableClicks = false;
            mgr.bloqueadoPorCanto = true;
            
            if (responderPlayerId == 1)
            {
                HandleAIResponse();
            }
            else
            {
                ShowPlayerResponseUI();
            }
        }

        private void HandleAIResponse()
        {
			bool shouldAccept = mgr.envidoStrategy?.ShouldAcceptEnvido(mgr.Opponent, mgr.Player) ?? true;
            
            Debug.Log($"🤖 IA decide: {(shouldAccept ? "QUIERO" : "NO QUIERO")} el {currentBidType}");
            
            if (shouldAccept)
            {
                OnAcceptEnvido();
            }
            else
            {
                OnDeclineEnvido();
            }
        }

        private void ShowPlayerResponseUI()
        {
            var actions = new ResponseActions(
                onAccept: OnAcceptEnvido,
                onDecline: OnDeclineEnvido,
                onRaise: CanRaise() ? OnRaiseEnvido : null
            );
            
            try
            {
                responseService = ServiceLocator.Get<IResponseService>();
                responseService.ShowBidResponse(actions, GetBidDisplayName());
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"❌ Error ResponseService: {ex.Message}");
            }
        }

        public void Update() { }

        public void Exit()
        {
            responseService?.HideBidResponse();
        }

        #region Response Logic

        private bool CanRaise()
        {
            return GetNextBidType() != null;
        }

        private string GetBidDisplayName()
        {
            string baseName = currentBidType.ToString();
            if (envidoManager.HasBids())
            {
                return $"{envidoManager.GetBidsDescription()} + {baseName}";
            }
            return baseName;
        }

        private void OnAcceptEnvido()
        {
            Debug.Log($"✅ {currentBidType} ACEPTADO");
            Debug.Log($"💰 Total acumulado: {envidoManager.GetAccumulatedPoints()} puntos");
            
            envidoManager.AddBid(currentBidType);
            
            mgr.bloqueadoPorCanto = false;
            
            mgr.TransitionToEnvidoState(envidoManager);
        }

        private void OnDeclineEnvido()
        {
            Debug.Log($"❌ {currentBidType} RECHAZADO");
            
            int accumulatedPoints = envidoManager.GetAccumulatedPoints();
            
            if (callerPlayerId == 0)
            {
                mgr.GameService.PlayerWinsEnvidoPoints(accumulatedPoints);
                Debug.Log($"🏆 Jugador gana {accumulatedPoints} puntos (IA rechazó {currentBidType})");
            }
            else
            {
                mgr.GameService.OpponentWinsEnvidoPoints(accumulatedPoints);
                Debug.Log($"💀 Oponente gana {accumulatedPoints} puntos (Jugador rechazó {currentBidType})");
            }
            
            Debug.Log($"📊 Puntos ganados: {accumulatedPoints} (ya acumulados: {envidoManager.GetBidsDescription()})");
            
            mgr.MarcarEnvidoComoCantado();
            mgr.bloqueadoPorCanto = false;
            mgr.ChangeState(new PlayState(mgr, mgr.activePlayer, true));
        }

        private void OnRaiseEnvido()
        {
            BidType? nextBid = GetNextBidType();
            
            if (nextBid == null)
            {
                Debug.LogWarning("⚠️ No se puede subir más - aceptando automáticamente");
                OnAcceptEnvido();
                return;
            }
            
            Debug.Log($"⬆️ SUBIENDO: {currentBidType} → {nextBid}");
            
            envidoManager.AddBid(currentBidType);
            
            Debug.Log($"💰 Puntos acumulados: {envidoManager.GetAccumulatedPoints()}");
            Debug.Log($"📋 Secuencia: {envidoManager.GetBidsDescription()} + {nextBid}");
            
            mgr.activePlayer = responderPlayerId;
            
            mgr.ChangeState(new AwaitingEnvidoResponseState(mgr, nextBid.Value, envidoManager));
        }

        #endregion

        #region Bid Logic

        private BidType? GetNextBidType()
        {
            switch (currentBidType)
            {
                case BidType.Envido:
                    return BidType.RealEnvido;
                    
                case BidType.RealEnvido:
                    return BidType.FaltaEnvido;
                    
                case BidType.FaltaEnvido:
                    return null; 
                    
                default:
                    Debug.LogWarning($"⚠️ BidType desconocido: {currentBidType}");
                    return null;
            }
        }

        #endregion
    }
}