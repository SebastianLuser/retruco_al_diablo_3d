using UnityEngine;

namespace Services
{
    public class GameHider : MonoBehaviour
    {
        [SerializeField] private GameObject[] objectsToHide;

        void Awake()
        {
            ServiceLocator.Register<GameHider>(this);
        }

        public void HideGameObjects()
        {
            Debug.Log("🚫 Ocultando objetos visuales de la partida...");

            foreach (var obj in objectsToHide)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }

        public void ShowGameObjects()
        {
            Debug.Log("✅ Mostrando objetos visuales de la partida...");

            foreach (var obj in objectsToHide)
            {
                if (obj != null)
                    obj.SetActive(true);
            }
        }
    }
}