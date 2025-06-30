using Services;
using StateMachines.Main;
using UnityEngine;

namespace StateMachines.Play
{

    public class PostBazaDelayState : IState
    {
        private TurnManager mgr;
        private float timer;

        public PostBazaDelayState(TurnManager m) => mgr = m;

        public void Enter()
        {
            timer = 0f;
            Debug.Log(
                $"[PostBazaDelay] Entré: Bazas ganadas Jugador={mgr.playerBazaWins}, IA={mgr.opponentBazaWins}, bazas jugadas={mgr.bazaCount}");
        }

        public void Update()
        {
            timer += Time.deltaTime;
            if (timer < 2f) return;

            if (mgr.playerBazaWins >= 2 || mgr.opponentBazaWins >= 2 || mgr.bazaCount >= mgr.totalBazas)
            {
                mgr.ChangeState(new EndHandState(mgr));
            }
            else
            {
                int nextPlayer = mgr.activePlayer;
                Debug.Log($"[PostBazaDelay] Iniciando nueva baza con {(nextPlayer == 0 ? "Jugador" : "IA")}");
                mgr.ChangeState(new PlayState(mgr, nextPlayer, true));
            }
        }

        public void Exit()
        {
        }
    }
}