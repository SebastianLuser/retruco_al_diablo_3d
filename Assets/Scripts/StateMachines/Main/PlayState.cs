using GameSystems.Bids;
using Services;
using StateMachines.Play;
using UnityEngine;

namespace StateMachines.Main
{
    public class PlayState : IState
    {
        readonly TurnManager   mgr;
        readonly IUIManager    ui;
        readonly int           playerId;
        readonly bool          isFirst;
        BidValidator           bidValidator;
        IBidFactory            bidFactory;

        public PlayState(TurnManager mgr, int playerId, bool isFirst)
        {
            this.mgr      = mgr;
            this.playerId = playerId;
            this.isFirst  = isFirst;
            ui            = ServiceLocator.Get<IUIManager>();
        }

        public void Enter()
        {
            Debug.Log($"▶️ PlayState: Jugador={playerId} | Primero={isFirst} | Baza={mgr.bazaCount}");

            if (playerId == 0)
            {
                HandlePlayerTurn();
            }
            else
            {
                HandleAITurn();
            }
        }

        public void Update()
        {
            if (mgr.playerMoveDone)
            {
                mgr.playerMoveDone = false;
        
                if (isFirst)
                {
                    int nextPlayer = 1 - playerId;
                    mgr.activePlayer = nextPlayer;
            
                    Debug.Log($"🔄 {(playerId == 0 ? "Jugador" : "IA")} jugó primero - Cambiando al segundo");
                    mgr.ChangeState(new PlayState(mgr, nextPlayer, false));
                }
                else
                {
                    Debug.Log("🔄 Ambos jugaron - Evaluando baza");
                    TransitionToNext();
                }
            }
        }

        public void Exit()
        {
            CleanupUI();
            mgr.EnablePlayerInput(false);
        }

        #region Turn Handling

        private void HandlePlayerTurn()
        {
            Debug.Log("🎮 Turno del jugador");

            if (mgr.bloqueadoPorCanto)
            {
                Debug.Log("⛔️ Input bloqueado por canto activo");
                mgr.EnablePlayerInput(false);
                return;
            }

            if (isFirst && mgr.bazaCount == 0 && !mgr.EnvidoCantado)
            {
                SetupBidding();
            }

            mgr.EnablePlayerInput(true);
            mgr.playerMoveDone = false;
        }

        private void HandleAITurn()
        {
            Debug.Log("🤖 Turno de la IA");
            mgr.EnablePlayerInput(false);
            
            if (isFirst && mgr.bazaCount == 0 && !mgr.EnvidoCantado)
            {
                CheckAIBidding();
            }
            else
            {
                mgr.AITurn();
            }
        }

        #endregion

        #region Bidding System

        private void SetupBidding()
        {
            try
            {
                bidValidator = new BidValidator();
                bidFactory   = ServiceLocator.Get<IBidFactory>();
                
                ui.ShowBidOptions(bidValidator, mgr);
                ui.OnBidSelected += HandleBid;
                
                Debug.Log("🎯 Opciones de bid configuradas para el jugador");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"❌ Error configurando bidding: {ex.Message}");
            }
        }

        private void CheckAIBidding()
        {
            bool shouldCallEnvido = mgr.envidoStrategy?.ShouldCallEnvido(mgr) ?? false;
            
            if (shouldCallEnvido)
            {
                Debug.Log("🤖 IA decide cantar Envido");
                
                var envidoManager = mgr.GetOrCreateEnvidoManager();
                envidoManager.AddBid(BidType.Envido);
                
                mgr.ChangeState(new AwaitingEnvidoResponseState(mgr, BidType.Envido, envidoManager));
                return;
            }

            mgr.AITurn();
        }

        private void HandleBid(BidType type)
        {
            var bid = bidFactory.CreateBid(type);
            if (!bidValidator.CanBid(bid, mgr)) 
            {
                Debug.Log($"❌ No se puede cantar {type}");
                return;
            }

            Debug.Log($"🗣️ JUGADOR canta {type}");
        
            if (IsEnvidoBid(type)) 
            {
                CleanupUI();
                
                var envidoManager = mgr.GetOrCreateEnvidoManager();
                envidoManager.AddBid(type);
                
                mgr.ChangeState(new AwaitingEnvidoResponseState(mgr, type, envidoManager));
            }
            else if (IsTrucoBid(type))
            {
                CleanupUI();
                mgr.ChangeState(new AwaitingTrucoResponseState(mgr, bid));
            }
        }

        #endregion

        #region State Transitions

        private void TransitionToNext()
        {
            if (playerId == 0)
            {
                Debug.Log("🔄 Transición: Jugador → IA");
                mgr.EnablePlayerInput(false);
            }

            if (isFirst)
            {
                mgr.ChangeState(new PlayState(mgr, 1 - playerId, false));
            }
            else
            {
                if (mgr.lastPlayedCardPlayer != null && mgr.lastPlayedCardOpponent != null)
                {
                    mgr.ChangeState(new EvaluateBazaState(mgr));
                }
                else
                {
                    Debug.LogWarning("⚠️ Faltan cartas para evaluar la baza");
                }
            }
        }

        #endregion

        #region Bid Type Helpers

        private bool IsEnvidoBid(BidType type)
        {
            return type == BidType.Envido || type == BidType.RealEnvido || type == BidType.FaltaEnvido;
        }

        private bool IsTrucoBid(BidType type)
        {
            return type == BidType.Truco || type == BidType.ReTruco || type == BidType.ValeCuatro;
        }

        #endregion

        #region Cleanup

        private void CleanupUI()
        {
            ui.OnBidSelected -= HandleBid;
            ui.HideEnvidoOptions();
            ui.HideTrucoOptions();
            ui.HideAcceptDeclinePanel();
            ui.ClearAllListeners();
        }

        #endregion
    }
}