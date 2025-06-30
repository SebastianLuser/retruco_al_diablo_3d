using UnityEngine;
using UnityEngine.SceneManagement;
using Services;
using System.Collections;
using GameSystems;

namespace UI
{

    public class VictoryEffects : MonoBehaviour
    {
        [Header("Managers")] [SerializeField] private TurnManager turnManager;

        [Header("Luz")] [SerializeField] private Light sceneLight;
        [SerializeField] private string victoryHexColor = "#CF8F8E";

        [Header("Bar en escena")]
        [Tooltip("Arrastra aquí el GameObject del Bar ya colocado (desactivado al Start)")]
        [SerializeField]
        private GameObject barSceneObject;

        [Header("Fade out del Diablo")] [SerializeField]
        private string diabloName = "Diablo_01";

        [SerializeField] private float fadeDuration = 2f;

        [Header("Volver al menú")] [SerializeField]
        private float returnToMenuDelay = 5f;

        [SerializeField] private string menuSceneName = "Menu";

        private IPointSystem pointSystem;
        private GameHider gameHider;
        private IAnimationService animationService;

        void Start()
        {
            if (barSceneObject != null)
                barSceneObject.SetActive(false);

            animationService = ServiceLocator.Get<IAnimationService>();
            pointSystem = ServiceLocator.Get<IPointSystem>();
            gameHider = ServiceLocator.Get<GameHider>();

            if (pointSystem != null)
                pointSystem.OnPlayerWin += PlayVictoryEffects;
            else
                Debug.LogError("VictoryEffects: no encontré IPointSystem.");
        }

        void OnDestroy()
        {
            if (pointSystem != null)
                pointSystem.OnPlayerWin -= PlayVictoryEffects;
        }

        private void PlayVictoryEffects()
        {
            Debug.Log("🏆 Ejecutando efectos de victoria.");

            if (gameHider != null)
                gameHider.HideGameObjects();

            if (turnManager != null)
                turnManager.enabled = false;

            if (sceneLight != null && ColorUtility.TryParseHtmlString(victoryHexColor, out var c))
                sceneLight.color = c;

            if (barSceneObject != null)
                barSceneObject.SetActive(true);

            var diablo = GameObject.Find(diabloName);
            if (diablo != null)
                StartCoroutine(FadeOutAndDestroy(diablo, fadeDuration));

            StartCoroutine(PlayWinAnimation());

            StartCoroutine(ReturnToMenuAfterDelay());
        }

        private IEnumerator FadeOutAndDestroy(GameObject go, float duration)
        {
            var rends = go.GetComponentsInChildren<Renderer>();
            var mats = new Material[rends.Length];
            for (int i = 0; i < rends.Length; i++)
                mats[i] = rends[i].material = new Material(rends[i].material);

            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float a = 1f - Mathf.Clamp01(t / duration);
                foreach (var m in mats)
                    if (m.HasProperty("_Color"))
                    {
                        var col = m.color;
                        col.a = a;
                        m.color = col;
                    }

                yield return null;
            }

            Destroy(go);
        }

        private IEnumerator ReturnToMenuAfterDelay()
        {
            yield return new WaitForSeconds(returnToMenuDelay);
            SceneManager.LoadScene(menuSceneName);
        }

        private IEnumerator PlayWinAnimation()
        {
            yield return new WaitForSeconds(3f);
            animationService?.PlayWinAnimation();
        }
    }
}