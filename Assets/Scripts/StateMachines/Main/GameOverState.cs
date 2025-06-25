using UnityEngine;
using Services;
using UnityEngine.SceneManagement;
using System.Collections;

namespace States
{
    public class GameOverState : IState
    {
        private TurnManager mgr;
        private bool playerWon;
        private float timer = 0f;
        private const float GAME_OVER_DURATION = 5f;
        
        private IAnimationService animationService;
        private GameHider gameHider;

        public GameOverState(TurnManager mgr)
        {
            this.mgr = mgr;
            
            var pointSystem = ServiceLocator.Get<IPointSystem>();
            int playerHP = pointSystem.GetPlayerPoints();
            int opponentHP = pointSystem.GetOpponentPoints();
            
            playerWon = opponentHP <= 0;
            
            Debug.Log($"🏁 GAME OVER: {(playerWon ? "VICTORIA" : "DERROTA")}");
            Debug.Log($"💚 HP Final - Jugador: {playerHP} | Oponente: {opponentHP}");
        }

        public void Enter()
        {
            Debug.Log($"🏁 GAME OVER STATE: {(playerWon ? "🏆 VICTORIA" : "💀 DERROTA")}");
            
            DisableGameControls();
            
            GetEffectServices();
            
            if (playerWon)
            {
                PlayVictoryEffects();
            }
            else
            {
                PlayDeathEffects();
            }
            
            timer = 0f;
        }

        public void Update()
        {
            timer += Time.deltaTime;
            
            if (timer >= GAME_OVER_DURATION)
            {
                ReturnToMenu();
            }
        }

        public void Exit()
        {
            Debug.Log("🚪 Saliendo de GameOverState");
        }

        #region Game Controls

        private void DisableGameControls()
        {
            // Desactivar clicks de cartas
            CardClick.enableClicks = false;
            
            // Desactivar input del jugador
            mgr.EnablePlayerInput(false);
            
            // Bloquear cualquier canto
            mgr.bloqueadoPorCanto = true;
            
            Debug.Log("🚫 Controles del juego desactivados");
        }

        #endregion

        #region Effect Services

        private void GetEffectServices()
        {
            try
            {
                animationService = ServiceLocator.Get<IAnimationService>();
                gameHider = ServiceLocator.Get<GameHider>();
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"⚠️ Algunos servicios de efectos no disponibles: {ex.Message}");
            }
        }

        #endregion

        #region Victory Effects

        private void PlayVictoryEffects()
        {
            Debug.Log("🏆 Ejecutando efectos de victoria");
            
            // Ocultar elementos del juego
            if (gameHider != null)
            {
                gameHider.HideGameObjects();
            }
            
            // Reproducir animación de victoria
            if (animationService != null)
            {
                animationService.PlayWinAnimation();
            }
            
        }
        
        #endregion

        #region Death Effects

        private void PlayDeathEffects()
        {
            Debug.Log("💀 Ejecutando efectos de derrota");
            
            // Ocultar elementos del juego
            if (gameHider != null)
            {
                gameHider.HideGameObjects();
            }
            
            // Reproducir animación de muerte
            if (animationService != null)
            {
                animationService.PlayDeathAnimation();
            }
        }
        

        #endregion

        #region Scene Transition

        private void ReturnToMenu()
        {
            Debug.Log("🔄 Volviendo al menú principal");
            
            try
            {
                // Cargar escena del menú
                SceneManager.LoadScene("Menu");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"❌ Error cargando menú: {ex.Message}");
                
                // Fallback: reiniciar el juego
                RestartGame();
            }
        }

        private void RestartGame()
        {
            Debug.Log("🔄 Reiniciando el juego");
            
            try
            {
                // Reiniciar la escena actual
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"❌ Error reiniciando: {ex.Message}");
            }
        }

        #endregion

        #region Public Properties

        public bool PlayerWon => playerWon;
        public float TimeRemaining => Mathf.Max(0, GAME_OVER_DURATION - timer);

        #endregion
    }
}