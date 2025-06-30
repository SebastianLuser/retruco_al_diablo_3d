using UnityEngine;
using System.Collections.Generic;
using Components.Cards;
using Services;

namespace StateMachines.Envido
{
    public class EnvidoResolutionState : IState
    {
        private TurnManager mgr;
        private EnvidoManager envidoManager;
        private int playerEnvidoValue;
        private List<Card> playerSelectedCards;
        private IUIManager uiManager;
        
        private float timer = 0f;
        private const float DURATION = 4f;

        public EnvidoResolutionState(TurnManager mgr, EnvidoManager envidoManager, int playerValue, List<Card> playerCards)
        {
            this.mgr = mgr;
            this.envidoManager = envidoManager;
            this.playerEnvidoValue = playerValue;
            this.playerSelectedCards = new List<Card>(playerCards);
        }

        public void Enter()
        {
            Debug.Log("⚔️ RESOLVIENDO ENVIDO");
            
            int opponentEnvidoValue = mgr.Opponent.CalculateEnvidoPoints();
            bool playerWins = DetermineWinner(playerEnvidoValue, opponentEnvidoValue);
            int damage = envidoManager.GetAccumulatedPoints();
            
            ApplyEnvidoResult(playerWins, damage);
            ShowResult(playerEnvidoValue, opponentEnvidoValue, playerWins, damage);
            
            timer = 0f;
        }

        public void Update()
        {
            timer += Time.deltaTime;
            
            if (timer >= DURATION)
            {
                CompleteEnvido();
            }
        }

        public void Exit()
        {
            uiManager?.HideEnvidoResolutionPanel();
        }

        private bool DetermineWinner(int playerEnvido, int opponentEnvido)
        {
            if (playerEnvido > opponentEnvido) return true;
            if (opponentEnvido > playerEnvido) return false;
            
            return mgr.playerIsHand;
        }

        private void ApplyEnvidoResult(bool playerWins, int damage)
        {
            if (playerWins)
            {
                mgr.GameService.PlayerWinsEnvidoPoints(damage);
                Debug.Log($"🏆 Jugador gana {envidoManager.GetBidsDescription()} - Diablo pierde {damage} HP");
            }
            else
            {
                mgr.GameService.OpponentWinsEnvidoPoints(damage); 
                Debug.Log($"💀 Diablo gana {envidoManager.GetBidsDescription()} - Jugador pierde {damage} HP");
            }
        }

        private void ShowResult(int playerEnvido, int opponentEnvido, bool playerWins, int damage)
        {
            try
            {
                uiManager = ServiceLocator.Get<IUIManager>();
                
                string winner = playerWins ? "Jugador" : "Diablo";
                string bidName = envidoManager.GetBidsDescription();
                
                uiManager.ShowEnvidoResolutionPanel(
                    playerValue: playerEnvido,
                    opponentValue: opponentEnvido,
                    winner: winner,
                    damage: damage,
                    playerCards: playerSelectedCards,
                    bidName: bidName
                );
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"❌ Error mostrando resultado: {ex.Message}");
            }
        }

        private void CompleteEnvido()
        {
            Debug.Log("🔄 Envido completado - Volviendo al juego principal");
            
            mgr.MarcarEnvidoComoCantado();
            mgr.bloqueadoPorCanto = false;
            
            TurnManager.Instance.TransitionToPlayState();
        }
    }
}