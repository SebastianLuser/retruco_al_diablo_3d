using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using GameSystems;
using Services;

namespace UI
{
    public class DeathEffect: MonoBehaviour
    {
        [Header("Managers")]
        [SerializeField] private TurnManager turnManager;
        [SerializeField] private float returnToMenuDelay = 5f;
        [SerializeField] private string menuSceneName = "Menu";
        
        private IPointSystem pointSystem;
        private IAnimationService animationService;
        private GameHider gameHider;
        
        void Start()
        {
            pointSystem = ServiceLocator.Get<IPointSystem>();
            animationService = ServiceLocator.Get<IAnimationService>();
            gameHider = ServiceLocator.Get<GameHider>();
            
            if (pointSystem != null)
                pointSystem.OnOpponentWin += PlayDeathEffects;
            else
                Debug.LogError("VictoryEffects: no encontré IPointSystem.");
            
            if (animationService == null)
                Debug.LogError("VictoryEffects: no encontré IAnimationService.");
                
        }
        
        private void PlayDeathEffects()
        {
            Debug.Log("☠️ Ejecutando efectos de derrota.");
            
            if (gameHider != null)
                gameHider.HideGameObjects();
            
            if (turnManager != null)
                turnManager.enabled = false;
            
            if (animationService != null)
                animationService.PlayDeathAnimation();
            
            StartCoroutine(ReturnToMenuAfterDelay());
        }

        private IEnumerator ReturnToMenuAfterDelay()
        {
            yield return new WaitForSeconds(returnToMenuDelay);
            SceneManager.LoadScene(menuSceneName);
        }

        void OnDestroy()
        {
            if (pointSystem != null)
                pointSystem.OnOpponentWin -= PlayDeathEffects;
        }
    }
}