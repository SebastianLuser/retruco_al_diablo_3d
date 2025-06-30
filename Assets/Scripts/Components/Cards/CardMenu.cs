using UnityEngine;
using UnityEngine.SceneManagement;

namespace Components.Cards
{

    [RequireComponent(typeof(Collider))]
    public class MenuCard : MonoBehaviour
    {
        public enum ActionType
        {
            LoadGame,
            QuitApp
        }

        public ActionType action = ActionType.LoadGame;

        public string sceneName = "Game";

        void OnMouseDown()
        {
            switch (action)
            {
                case ActionType.LoadGame:
                    SceneManager.LoadScene(sceneName);
                    break;
                case ActionType.QuitApp:
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                    break;
            }
        }
    }
}