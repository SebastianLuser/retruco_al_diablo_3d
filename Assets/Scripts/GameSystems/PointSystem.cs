using System;
using UnityEngine;

namespace Services
{
    public class PointSystem: MonoBehaviour, IPointSystem
    {
        [SerializeField] private int startingHP = 30;
        private int playerHP;
        private int opponentHP;

        public event Action OnPlayerWin;
        public event Action OnOpponentWin;
        public event Action<int, int> OnPointsUpdated;

        void Awake()
        {
            ServiceLocator.Register<IPointSystem>(this);
        }

        public void InitializePoints()
        {
            playerHP = startingHP;
            opponentHP = startingHP;
            NotifyPointsChanged();
            
            Debug.Log($"💚 HP inicializado: Jugador {playerHP} HP | Oponente {opponentHP} HP");
        }

        #region Métodos Principales de Daño y Curación

        public void DealDamageToPlayer(int damage)
        {
            if (damage <= 0) return;
            
            int previousHP = playerHP;
            playerHP = Mathf.Max(0, playerHP - damage);
            
            Debug.Log($"💔 Jugador recibe {damage} daño: {previousHP} → {playerHP} HP");
            NotifyPointsChanged();
            CheckVictory();
        }

        public void DealDamageToOpponent(int damage)
        {
            if (damage <= 0) return;
            
            int previousHP = opponentHP;
            opponentHP = Mathf.Max(0, opponentHP - damage);
            
            Debug.Log($"💔 Oponente recibe {damage} daño: {previousHP} → {opponentHP} HP");
            NotifyPointsChanged();
            CheckVictory();
        }

        public void HealPlayer(int healAmount)
        {
            if (healAmount <= 0) return;
            
            int previousHP = playerHP;
            playerHP += healAmount;
            
            Debug.Log($"💚 Jugador se cura {healAmount} HP: {previousHP} → {playerHP}");
            NotifyPointsChanged();
        }

        public void HealOpponent(int healAmount)
        {
            if (healAmount <= 0) return;
            
            int previousHP = opponentHP;
            opponentHP += healAmount;
            
            Debug.Log($"💚 Oponente se cura {healAmount} HP: {previousHP} → {opponentHP}");
            NotifyPointsChanged();
        }

        #endregion

        #region Métodos de Conveniencia (Semánticamente Claros)

        public void PlayerWinsPoints(int points)
        {
            Debug.Log($"🏆 Jugador gana {points} puntos - Oponente recibe daño");
            DealDamageToOpponent(points);
        }

        public void OpponentWinsPoints(int points)
        {
            Debug.Log($"💀 Oponente gana {points} puntos - Jugador recibe daño");
            DealDamageToPlayer(points);
        }

        #endregion



        public int GetPlayerPoints() => playerHP;
        public int GetOpponentPoints() => opponentHP;

        private void CheckVictory()
        {
            if (opponentHP <= 0)
            {
                Debug.Log("🏆 VICTORIA DEL JUGADOR - Oponente sin HP");
                OnPlayerWin?.Invoke();
                TriggerGameOver();
            }
            else if (playerHP <= 0)
            {
                Debug.Log("💀 DERROTA DEL JUGADOR - Sin HP");
                OnOpponentWin?.Invoke();
                TriggerGameOver();
            }
        }

        private void TriggerGameOver()
        {
            StartCoroutine(DelayedGameOver());
        }

        private System.Collections.IEnumerator DelayedGameOver()
        {
            yield return null;
            
            try
            {
                TurnManager.Instance.TransitionToGameOver();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"❌ Error transición GameOver: {ex.Message}");
            }
        }

        private void NotifyPointsChanged()
        {
            OnPointsUpdated?.Invoke(playerHP, opponentHP);
        }
    }

    public interface IPointSystem
    {
        void InitializePoints();
        
        void DealDamageToPlayer(int damage);
        void DealDamageToOpponent(int damage);
        void HealPlayer(int healAmount);
        void HealOpponent(int healAmount);
        
        void PlayerWinsPoints(int points);
        void OpponentWinsPoints(int points);
        
        int GetPlayerPoints();
        int GetOpponentPoints();
        
        event Action OnPlayerWin;
        event Action OnOpponentWin;
        event Action<int, int> OnPointsUpdated;
    }
}