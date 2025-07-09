using Services;
using StateMachines.Main;
using UnityEngine;

namespace StateMachines.Play
{
    public class DealHandState : IState
    {
        private TurnManager mgr;

        public DealHandState(TurnManager m) => mgr = m;

        private bool _alreadyChange;

        public void Enter()
        {
            Debug.Log("🌀 ===== NUEVA RONDA =====");

            ResetRoundState();

            mgr.PlacementService.ResetTable();
            mgr.DeckService.DealHand();

            mgr.playerIsHand = true;
            mgr.activePlayer = 0;

            mgr.actualRound++;

            Debug.Log("🎮 Nueva ronda iniciada - Jugador es mano");
            if (mgr.actualRound > 1)
            {
                mgr.ChangeState(new PlayState(mgr, 0, true));
                _alreadyChange = true;
            }
        }

        private void ResetRoundState()
        {
            mgr.bazaCount = 0;
            mgr.playerBazaWins = 0;
            mgr.opponentBazaWins = 0;
            mgr.playerMoveDone = false;

            mgr.lastPlayedCardPlayer = null;
            mgr.lastPlayedCardOpponent = null;

            mgr.ResetEnvidoFlags();

            var envidoManager = mgr.GetOrCreateEnvidoManager();
            envidoManager.Reset();

            Debug.Log("🔄 Estado de ronda reseteado completamente");
        }

        public void Update()
        {
            if (!_alreadyChange && mgr.isDiceSelected)
            {
                mgr.ChangeState(new PlayState(mgr, 0, true));
            }
        }

        public void Exit()
        {
        }
    }
}