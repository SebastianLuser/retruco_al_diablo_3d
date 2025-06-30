using System;
using Services;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HistoryController : MonoBehaviour
    {
        [SerializeField] private Transform contentTransform;
        [SerializeField] private GameObject matchPrefab;
        [SerializeField] private GameObject historyPanel;
        [SerializeField] private Button showButton;
        [SerializeField] private Button closeButton;

        private IMatchesHistoryManager historyManager;

        private void Awake()
        {
            showButton.onClick.AddListener(ToggleHistoryPanel);
            closeButton.onClick.AddListener(ToggleHistoryPanel);
        }

        private void Start()
        {
            historyPanel.SetActive(false);
            
            try
            {
                historyManager = ServiceLocator.Get<IMatchesHistoryManager>();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to get IMatchesHistoryManager: {ex.Message}");
                return;
            }
            
            historyManager.RegistryMatch(new Match
            {
                Id = 1,
                Date = DateTime.Now.AddMinutes(-30),
                Player1 = "Pedrito",
                Result = "Diablo"
            });
            historyManager.RegistryMatch(new Match
            {
                Id = 2,
                Date = DateTime.Now.AddMinutes(-10),
                Player1 = "Pedrito",
                Result = "Diablo"
            });
            historyManager.RegistryMatch(new Match
            {
                Id = 2,
                Date = DateTime.Now.AddMinutes(-40),
                Player1 = "Pedrito",
                Result = "Diablo"
            });
            historyManager.RegistryMatch(new Match
            {
                Id = 2,
                Date = DateTime.Now.AddMinutes(-50),
                Player1 = "Pedrito",
                Result = "Diablo"
            });
            historyManager.RegistryMatch(new Match
            {
                Id = 2,
                Date = DateTime.Now.AddMinutes(-70),
                Player1 = "Pedrito",
                Result = "Diablo"
            });
            historyManager.RegistryMatch(new Match
            {
                Id = 2,
                Date = DateTime.Now.AddMinutes(-100),
                Player1 = "Pedrito",
                Result = "Diablo"
            });
            ShowHistoryInUI();
        }

        private void ShowHistoryInUI()
        {
            foreach (Transform child in contentTransform)
                Destroy(child.gameObject);

            var history = historyManager.GetHistory();
            foreach (var match in history)
            {
                GameObject cardObj = Instantiate(matchPrefab, contentTransform);

                var cardUI = cardObj.GetComponent<TrucoMatchCardUI>();
                cardUI.SetMatch(match);
            }
        }

        private void ToggleHistoryPanel()
        {
            bool isActive = historyPanel.activeSelf;
            historyPanel.SetActive(!isActive);

            if (!isActive)
                ShowHistoryInUI();
        }
    }
}