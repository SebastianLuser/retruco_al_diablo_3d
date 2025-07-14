using Components.Cards;
using UnityEngine;
using Services;
using GameSystems;
using GameSystems.Bids;
using GameSystems.Dialogs;

namespace StateMachines.Play
{
    public class AwaitingTrucoResponseState : IState
    {
        private TurnManager mgr;
        private IBid trucoBid;
        private IResponseService responseService;
        private int callerPlayerId;
        private int responderPlayerId;
        private bool _isAIResponse = false;

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
                _isAIResponse = true;
                HandleAIResponse();
            }
            else
            {
                _isAIResponse = false;
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
            
            if (_isAIResponse)
            {
                DialogueManager.Instance.EnqueueDialogue(new DialogueEntry()
                {
                    autoPass = true,
                    dialogueText = $"Quiero carajo!",
                    duration = 2f,
                    speaker = "Diablo"
                });
            }
            
            mgr.TransitionToPlayState();
        }

        private void OnDeclineTruco()
        {
            Debug.Log($"❌ {trucoBid.Name} RECHAZADO");
            
            mgr.GameService.DeclineTruco(trucoBid, callerPlayerId);
            mgr.bloqueadoPorCanto = false;
            
            if (_isAIResponse)
            {
                DialogueManager.Instance.EnqueueDialogue(new DialogueEntry()
                {
                    autoPass = true,
                    dialogueText = $"No quiero",
                    duration = 2f,
                    speaker = "Diablo"
                });
            }
            
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

            if (_isAIResponse)
            {
                DialogueManager.Instance.EnqueueDialogue(new DialogueEntry()
                {
                    autoPass = true,
                    dialogueText = $"Quiero {nextTrucoBid.Name}!",
                    duration = 2f,
                    speaker = "Diablo"
                });
            }
            
            mgr.ChangeState(new AwaitingTrucoResponseState(mgr, nextTrucoBid));
        }

        #endregion
    }
}