using UnityEngine;
using Services;
using Match.Bids;
using GameSystems;

namespace States
{
    public class AwaitingTrucoResponseState : IState
    {
        private TurnManager mgr;
        private IBid trucoBid;
        private IResponseService responseService;
        private int callerPlayerId;
        private int responderPlayerId;

        public AwaitingTrucoResponseState(TurnManager mgr, IBid bid)
        {
            this.mgr = mgr;
            this.trucoBid = bid;
            this.callerPlayerId = mgr.activePlayer;
            this.responderPlayerId = 1 - mgr.activePlayer;
        }

        public void Enter()
        {
            Debug.Log($"⏳ AWAITING: {(responderPlayerId == 0 ? "Jugador" : "IA")} debe responder a {trucoBid.Name}");
            
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

        public void Update() { }

        public void Exit()
        {
            responseService?.HideBidResponse();
        }

        #region AI Response

        private void HandleAIResponse()
        {
            bool shouldAccept = mgr.trucoStrategy?.ShouldAcceptTruco(trucoBid, mgr.Opponent, mgr.Player) ?? true;
            bool shouldRaise = mgr.trucoStrategy?.ShouldRaiseTruco(trucoBid, mgr.Opponent, mgr.Player) ?? false;
            
            Debug.Log($"🤖 IA decide sobre {trucoBid.Name}: Accept={shouldAccept}, Raise={shouldRaise}");
            
            if (shouldRaise && CanRaiseTruco())
            {
                OnRaiseTruco();
            }
            else if (shouldAccept)
            {
                OnAcceptTruco();
            }
            else
            {
                OnDeclineTruco();
            }
        }

        #endregion

        #region Player Response UI

        private void ShowPlayerResponseUI()
        {
            var trucoActions = new ResponseActions(
                onAccept: OnAcceptTruco,
                onDecline: OnDeclineTruco,
                onRaise: CanRaiseTruco() ? OnRaiseTruco : null
            );
            
            try
            {
                responseService = ServiceLocator.Get<IResponseService>();
                responseService.ShowBidResponse(trucoActions, trucoBid.Name);
                
                Debug.Log($"📱 Mostrando respuesta para {trucoBid.Name}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"❌ Error ResponseService Truco: {ex.Message}");
            }
        }

        #endregion

        #region Response Logic

        private bool CanRaiseTruco()
        {
            return trucoBid.Next != null;
        }

        private void OnAcceptTruco()
        {
            Debug.Log($"✅ {trucoBid.Name} ACEPTADO");
            
            mgr.GameService.AcceptTruco(trucoBid);
            mgr.bloqueadoPorCanto = false;
            
            mgr.TransitionToPlayState();
        }

        private void OnDeclineTruco()
        {
            Debug.Log($"❌ {trucoBid.Name} RECHAZADO");
            
            mgr.GameService.DeclineTruco(trucoBid, callerPlayerId);
            mgr.bloqueadoPorCanto = false;
            
            mgr.ChangeState(new DealHandState(mgr));
        }

        private void OnRaiseTruco()
        {
            if (!CanRaiseTruco())
            {
                Debug.LogWarning("⚠️ No se puede subir más - aceptando automáticamente");
                OnAcceptTruco();
                return;
            }
            
            IBid nextTrucoBid = trucoBid.Next;
            Debug.Log($"⬆️ SUBIENDO TRUCO: {trucoBid.Name} → {nextTrucoBid.Name}");
            
            mgr.activePlayer = responderPlayerId;
            
            mgr.ChangeState(new AwaitingTrucoResponseState(mgr, nextTrucoBid));
        }

        #endregion
    }
}