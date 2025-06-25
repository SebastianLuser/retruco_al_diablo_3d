using UnityEngine;

namespace States
{
    public class EvaluateBazaState : IState
    {
        private TurnManager mgr;
        public EvaluateBazaState(TurnManager m) => mgr = m;
        
        public void Enter()
        {
            mgr.bazaCount++;
            
            int winner = mgr.GameService.EvaluateRound(
                mgr.lastPlayedCardPlayer,
                mgr.lastPlayedCardOpponent,
                mgr.playerIsHand
            );
            
            if (winner == 0) mgr.playerBazaWins++;
            else               mgr.opponentBazaWins++;
            
            mgr.activePlayer = winner;

            Debug.Log($"[EvaluateState] Bazas jugadas: {mgr.bazaCount}, marcador: {mgr.playerBazaWins}-{mgr.opponentBazaWins}");
            
            mgr.ChangeState(new PostBazaDelayState(mgr));
            
        }
        public void Update() { }
        public void Exit() { }
    }

}