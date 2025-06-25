using System;

namespace GameSystems
{
    [System.Serializable]
    public class ResponseActions
    {
        public Action OnAccept;
        public Action OnDecline;
        public Action OnRaise;
        
        public ResponseActions(Action onAccept, Action onDecline, Action onRaise = null)
        {
            OnAccept = onAccept;
            OnDecline = onDecline;
            OnRaise = onRaise;
        }
        
        public void ExecuteAccept() => OnAccept?.Invoke();
        public void ExecuteDecline() => OnDecline?.Invoke();
        public void ExecuteRaise() => OnRaise?.Invoke();
        
        public bool HasRaiseAction => OnRaise != null;
    }
}