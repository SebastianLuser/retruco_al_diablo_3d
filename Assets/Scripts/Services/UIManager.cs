using System;
using System.Collections.Generic;
using Components.Cards;
using GameSystems;
using GameSystems.Bids;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Services
{
    public interface IUIManager
    {
        void ShowAcceptDeclinePanel(Action<bool> onResponse);
        void HideAcceptDeclinePanel();
        void ShowBidOptions(BidValidator validator, TurnManager mgr);
        void ShowEnvidoOptions(IEnumerable<BidType> bids);
        void HideEnvidoOptions();
        void ShowTrucoOptions(IEnumerable<BidType> bids);
        void HideTrucoOptions();
        void ShowCantosPanel(Action onCantar, Action onNoCantar);
        void HideCantosPanel();
        void UpdateEnvidoValue(int value);
        void ShowEnvidoResolutionPanel(int playerValue, int opponentValue, string winner, int damage, List<Card> playerCards, string bidName);
        void HideEnvidoResolutionPanel();
        void ClearAllListeners();
        void ShowEnvidoRecommendation(string recommendation);
        
        event Action<bool> OnAcceptDeclineResponse;
        event Action<BidType> OnBidSelected;
    }

    public class UIManager : MonoBehaviour, IUIManager
    {
        [Header("Envido Bids Panel")]
        [SerializeField] private GameObject envidoBidsPanel;
        [SerializeField] private Button envidoButton;
        [SerializeField] private Button envidoEnvidoButton;
        [SerializeField] private Button realEnvidoButton;
        [SerializeField] private Button faltaEnvidoButton;
        
        [Header("Truco Bids Panel")]
        [SerializeField] private GameObject trucoBidsPanel;
        [SerializeField] private Button trucoButton;
        [SerializeField] private Button reTrucoButton;
        [SerializeField] private Button valeCuatroButton;

        [Header("Response Panel")]
        [SerializeField] private GameObject responsePanel;
        [SerializeField] private Button acceptButton;
        [SerializeField] private Button declineButton;

        [Header("Cantos Panel")]
        [SerializeField] private GameObject cantosPanel;
        [SerializeField] private Button cantarButton;
        [SerializeField] private Button noCantarButton;
        [SerializeField] private TextMeshProUGUI envidoValueText;
        [SerializeField] private TextMeshProUGUI cantarInstructionText;

        [Header("Envido Resolution Panel")]
        [SerializeField] private GameObject envidoResolutionPanel;
        [SerializeField] private TextMeshProUGUI playerValueText;
        [SerializeField] private TextMeshProUGUI playerResultValueText;
        [SerializeField] private TextMeshProUGUI opponentValueText;
        
        [Header("Sort Buttons")] 
        [SerializeField] private Button sortBySuitButton;  
        [SerializeField] private Button sortByRankButton; 
        
        public event Action<bool> OnAcceptDeclineResponse;
        public event Action<BidType> OnBidSelected;

        private Action<bool> currentAcceptDeclineAction;
        private Action currentCantarAction;
        private Action currentNoCantarAction;

        void Awake()
        {
            ServiceLocator.Register<IUIManager>(this);
            SetupStaticButtonListeners();
            InitializePanels();
        }

        void OnDestroy()
        {
            CleanupAllResources();
        }

        private void SetupStaticButtonListeners()
        {
            if (acceptButton != null)
            {
                acceptButton.onClick.RemoveAllListeners();
                acceptButton.onClick.AddListener(() => OnAcceptPressed());
            }

            if (declineButton != null)
            {
                declineButton.onClick.RemoveAllListeners();
                declineButton.onClick.AddListener(() => OnDeclinePressed());
            }

            if (envidoButton != null)
            {
                envidoButton.onClick.RemoveAllListeners();
                envidoButton.onClick.AddListener(() => OnBidSelected?.Invoke(BidType.Envido));
            }

            if (realEnvidoButton != null)
            {
                realEnvidoButton.onClick.RemoveAllListeners();
                realEnvidoButton.onClick.AddListener(() => OnBidSelected?.Invoke(BidType.RealEnvido));
            }

            if (faltaEnvidoButton != null)
            {
                faltaEnvidoButton.onClick.RemoveAllListeners();
                faltaEnvidoButton.onClick.AddListener(() => OnBidSelected?.Invoke(BidType.FaltaEnvido));
            }

            if (envidoEnvidoButton != null)
            {
                envidoEnvidoButton.onClick.RemoveAllListeners();
                envidoEnvidoButton.onClick.AddListener(() => HandleEnvidoEnvido());
            }

            if (trucoButton != null)
            {
                trucoButton.onClick.RemoveAllListeners();
                trucoButton.onClick.AddListener(() => OnBidSelected?.Invoke(BidType.Truco));
            }

            if (reTrucoButton != null)
            {
                reTrucoButton.onClick.RemoveAllListeners();
                reTrucoButton.onClick.AddListener(() => OnBidSelected?.Invoke(BidType.ReTruco));
            }

            if (valeCuatroButton != null)
            {
                valeCuatroButton.onClick.RemoveAllListeners();
                valeCuatroButton.onClick.AddListener(() => OnBidSelected?.Invoke(BidType.ValeCuatro));
            }
            
            if (sortBySuitButton != null)
            {
                sortBySuitButton.onClick.RemoveAllListeners();
                sortBySuitButton.onClick.AddListener(SortBySuit);
            }

            if (sortByRankButton != null)
            {
                sortByRankButton.onClick.RemoveAllListeners();
                sortByRankButton.onClick.AddListener(SortByRank);
            }
        }

        private void InitializePanels()
        {
            if (envidoBidsPanel != null) envidoBidsPanel.SetActive(false);
            if (trucoBidsPanel != null) trucoBidsPanel.SetActive(false);
            if (responsePanel != null) responsePanel.SetActive(false);
            if (cantosPanel != null) cantosPanel.SetActive(false);
            if (envidoResolutionPanel != null) envidoResolutionPanel.SetActive(false);
        }

        public void ShowBidOptions(BidValidator validator, TurnManager mgr)
        {
            if (validator == null || mgr == null)
            {
                Debug.LogError("UIManager: ShowBidOptions called with null parameters");
                return;
            }

            IBidFactory bidFactory;
            try
            {
                bidFactory = ServiceLocator.Get<IBidFactory>();
            }
            catch (Exception ex)
            {
                Debug.LogError($"UIManager: Failed to get BidFactory - {ex.Message}");
                return;
            }
            
            bool hasEnvidoOptions = false;
            bool hasTrucoOptions = false;

            bool isFirstPlayerFirstPlay = (mgr.activePlayer == 0) && (mgr.bazaCount == 0);
            if (isFirstPlayerFirstPlay && !mgr.EnvidoCantado)
            {
                foreach (var bidType in new[] { BidType.Envido, BidType.RealEnvido, BidType.FaltaEnvido })
                {
                    var bid = bidFactory.CreateBid(bidType);
                    if (validator.CanBid(bid, mgr))
                    {
                        hasEnvidoOptions = true;
                        break;
                    }
                }
            }
            
            foreach (var bidType in new[] { BidType.Truco, BidType.ReTruco, BidType.ValeCuatro })
            {
                var bid = bidFactory.CreateBid(bidType);
                if (validator.CanBid(bid, mgr) && !mgr.TrucoCantado)
                {
                    hasTrucoOptions = true;
                    break;
                }
            }

            if (hasEnvidoOptions)
            {
                ShowEnvidoOptions(new[] { BidType.Envido, BidType.RealEnvido, BidType.FaltaEnvido });
            }

            if (hasTrucoOptions)
            {
                ShowTrucoOptions(new[] { BidType.Truco, BidType.ReTruco, BidType.ValeCuatro });
            }
        }

        public void ShowEnvidoOptions(IEnumerable<BidType> bids)
        {
            if (bids == null)
            {
                Debug.LogWarning("UIManager: ShowEnvidoOptions called with null bids");
                return;
            }

            if (envidoBidsPanel != null)
                envidoBidsPanel.SetActive(true);
        }

        public void HideEnvidoOptions()
        {
            if (envidoBidsPanel != null)
                envidoBidsPanel.SetActive(false);
        }

        public void ShowTrucoOptions(IEnumerable<BidType> bids)
        {
            if (bids == null)
            {
                Debug.LogWarning("UIManager: ShowTrucoOptions called with null bids");
                return;
            }

            if (trucoBidsPanel != null)
                trucoBidsPanel.SetActive(true);
        }

        public void HideTrucoOptions()
        {
            if (trucoBidsPanel != null)
                trucoBidsPanel.SetActive(false);
        }

        public void ShowAcceptDeclinePanel(Action<bool> onResponse)
        {
            if (onResponse == null)
            {
                Debug.LogWarning("UIManager: ShowAcceptDeclinePanel called with null action");
                return;
            }

            currentAcceptDeclineAction = onResponse;
            
            if (responsePanel != null)
                responsePanel.SetActive(true);
        }

        public void HideAcceptDeclinePanel()
        {
            if (responsePanel != null)
                responsePanel.SetActive(false);
            
            currentAcceptDeclineAction = null;
        }

        public void ShowCantosPanel(Action onCantar, Action onNoCantar)
        {
            currentCantarAction = onCantar;
            currentNoCantarAction = onNoCantar;

            if (cantarButton != null)
            {
                cantarButton.onClick.RemoveAllListeners();
                cantarButton.onClick.AddListener(() => OnCantarPressed());
            }

            if (noCantarButton != null)
            {
                noCantarButton.onClick.RemoveAllListeners();
                noCantarButton.onClick.AddListener(() => OnNoCantarPressed());
            }

            if (cantarInstructionText != null)
                cantarInstructionText.text = "Select up to 2 cards for your Envido";

            if (cantosPanel != null)
                cantosPanel.SetActive(true);
        }

        public void HideCantosPanel()
        {
            if (cantosPanel != null)
                cantosPanel.SetActive(false);

            currentCantarAction = null;
            currentNoCantarAction = null;
        }

        public void UpdateEnvidoValue(int value)
        {
            if (envidoValueText != null)
                envidoValueText.text = $"Envido: {value}";
        }

        public void ShowEnvidoResolutionPanel(int playerValue, int opponentValue, string winner, int damage, List<Card> playerCards, string bidName)
        {
            if (envidoResolutionPanel != null)
                envidoResolutionPanel.SetActive(true);
            
            if (playerValueText != null)
                playerValueText.text = $"Your Envido: {playerValue}";
            
            if (opponentValueText != null)
                opponentValueText.text = $"{opponentValue}";
            
            if (playerResultValueText != null)
                playerResultValueText.text = $"{playerValue}";
        }

        public void HideEnvidoResolutionPanel()
        {
            if (envidoResolutionPanel != null)
                envidoResolutionPanel.SetActive(false);
        }

        public void ClearAllListeners()
        {
            OnAcceptDeclineResponse = null;
            OnBidSelected = null;
            currentAcceptDeclineAction = null;
            currentCantarAction = null;
            currentNoCantarAction = null;
        }
        
        public void ShowEnvidoRecommendation(string recommendation)
        {
            if (cantarInstructionText != null)
                cantarInstructionText.text = recommendation;
        }

        private void OnAcceptPressed()
        {
            currentAcceptDeclineAction?.Invoke(true);
            OnAcceptDeclineResponse?.Invoke(true);
            HideAcceptDeclinePanel();
        }

        private void OnDeclinePressed()
        {
            currentAcceptDeclineAction?.Invoke(false);
            OnAcceptDeclineResponse?.Invoke(false);
            HideAcceptDeclinePanel();
        }

        private void OnCantarPressed()
        {
            currentCantarAction?.Invoke();
            HideCantosPanel();
        }

        private void OnNoCantarPressed()
        {
            currentNoCantarAction?.Invoke();
            HideCantosPanel();
        }

        private void HandleEnvidoEnvido()
        {
            OnBidSelected?.Invoke(BidType.Envido);
            OnBidSelected?.Invoke(BidType.Envido);
            Debug.Log("Envido Envido sequence triggered");
        }

        private void CleanupAllResources()
        {
            Debug.Log("UIManager: Cleaning up all resources");

            ClearAllListeners();

            acceptButton?.onClick.RemoveAllListeners();
            declineButton?.onClick.RemoveAllListeners();
            cantarButton?.onClick.RemoveAllListeners();
            noCantarButton?.onClick.RemoveAllListeners();

            ServiceLocator.Unregister<IUIManager>();
        }
        
        private void SortBySuit()
        {
            Debug.Log("🔄 Sorting by Suit requested");
            var deckService = ServiceLocator.Get<IDeckService>();
            deckService.SortPlayerHand(SortType.BySuit);        }

        private void SortByRank()
        {
            Debug.Log("🔄 Sorting by Rank requested");
            var deckService = ServiceLocator.Get<IDeckService>();
            deckService.SortPlayerHand(SortType.ByPower);        
        }
        
    }
}