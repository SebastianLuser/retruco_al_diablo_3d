using Services;
using StateMachines.Envido;
using UnityEngine;

namespace StateMachines.Main
{
    public class EnvidoState : IState
    {
        private TurnManager mgr;
        private EnvidoManager envidoManager;
        private StateMachine envidoSubMachine;

        public EnvidoState(TurnManager mgr, EnvidoManager envidoManager)
        {
            this.mgr = mgr;
            this.envidoManager = envidoManager;
        }

        public void Enter()
        {
            Debug.Log($"🎯 ENVIDO STATE: Entrando al contexto Envido");
            Debug.Log($"📋 Bids acumulados: {envidoManager.GetBidsDescription()}");
            Debug.Log($"💰 Puntos en juego: {envidoManager.GetAccumulatedPoints()}");
            
            envidoSubMachine = new StateMachine();
            
            envidoSubMachine.ChangeState(new EnvidoCardSelectionState(mgr, envidoManager));
        }

        public void Update()
        {
            envidoSubMachine?.Update();
        }

        public void Exit()
        {
            Debug.Log("🚪 ENVIDO STATE: Saliendo del contexto Envido");
            envidoSubMachine = null;
        }

        public void CompleteEnvido()
        {
            Debug.Log("✅ Envido completado - Volviendo a PlayState");
            
            TurnManager.Instance.TransitionToPlayState();
        }

        public void ChangeSubState(IState newState)
        {
            envidoSubMachine?.ChangeState(newState);
        }

        public EnvidoManager Manager => envidoManager;
        public TurnManager TurnManager => mgr;
    }
}