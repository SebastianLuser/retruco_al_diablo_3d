using Services;
using UnityEngine;

namespace StateMachines.Play
{
    public class EndHandState : IState
    {
        private TurnManager mgr;
        private float timer;

        public EndHandState(TurnManager m) => mgr = m;

        public void Enter()
        {
            Debug.Log("🛑 Fin de mano");
            mgr.PlacementService.ClearTable();

            int handWinner;
            if (mgr.playerBazaWins == mgr.opponentBazaWins)
            {
                Debug.Log("🟰 Mano empatada: gana el jugador que fue mano.");
                handWinner = mgr.playerIsHand ? 0 : 1;
            }
            else
            {
                handWinner = mgr.playerBazaWins > mgr.opponentBazaWins ? 0 : 1;
            }

            mgr.GameService.ResolveHand(handWinner);
            mgr.playerIsHand = !mgr.playerIsHand;
            timer = 0f;
        }


        public void Update()
        {
            timer += Time.deltaTime;
            if (timer >= 2f)
            {
                mgr.ChangeState(new DealHandState(mgr));
            }
        }

        public void Exit() { }
    }

}