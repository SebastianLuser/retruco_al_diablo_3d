using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Match.Bids;

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
       void ClearAllListeners();
       event Action<bool> OnAcceptDeclineResponse;
       event Action<BidType> OnBidSelected;
       
       void ShowEnvidoSelectionPanel(System.Action onConfirm, System.Action onCancel, int initialValue);
       void HideEnvidoSelectionPanel();
       void UpdateEnvidoSelectionValue(int value);
       void ShowEnvidoResolutionPanel(int playerValue, int opponentValue, string winner, int damage, List<Card> playerCards, string bidName);
       void HideEnvidoResolutionPanel();
   }

   public class UIManager : MonoBehaviour, IUIManager
   {
       [Header("Envido Options Panel")]
       [SerializeField] private GameObject envidoOptionsPanel;
       [SerializeField] private Transform envidoButtonsContainer;

       [Header("Truco Options Panel")]
       [SerializeField] private GameObject trucoOptionsPanel;
       [SerializeField] private Transform trucoButtonsContainer;

       [Header("Bid Button Prefab")]
       [SerializeField] private GameObject bidButtonPrefab;

       [Header("Quiero / No Quiero Panel")]
       [SerializeField] private GameObject responsePanel;
       [SerializeField] private Button acceptButton;
       [SerializeField] private Button declineButton;

       [Header("Envido Selection")]
       [SerializeField] private GameObject envidoSelectionPanel;
       [SerializeField] private Button confirmEnvidoButton;
       [SerializeField] private Button cancelEnvidoButton;
       [SerializeField] private TextMeshProUGUI envidoValueText;
       [SerializeField] private TextMeshProUGUI envidoInstructionText;

       [Header("Envido Resolution")]
       [SerializeField] private GameObject envidoResolutionPanel;
       [SerializeField] private TextMeshProUGUI playerValueText;
       [SerializeField] private TextMeshProUGUI opponentValueText;
       [SerializeField] private TextMeshProUGUI winnerText;
       [SerializeField] private TextMeshProUGUI damageText;
       
       public event Action<bool> OnAcceptDeclineResponse;
       public event Action<BidType> OnBidSelected;
       
       private readonly List<GameObject> envidoButtons = new();
       private readonly List<GameObject> trucoButtons = new();

       private System.Action currentConfirmAction;
       private System.Action currentCancelAction;
       private Action<bool> currentAcceptDeclineAction;

       void Awake()
       {
           ServiceLocator.Register<IUIManager>(this);
           SetupInitialButtonListeners();
           InitializePanels();
       }

       void OnDestroy()
       {
           CleanupAllResources();
       }

       private void SetupInitialButtonListeners()
       {
           if (acceptButton != null)
           {
               acceptButton.onClick.RemoveAllListeners();
               acceptButton.onClick.AddListener(OnAcceptButtonPressed);
           }

           if (declineButton != null)
           {
               declineButton.onClick.RemoveAllListeners();
               declineButton.onClick.AddListener(OnDeclineButtonPressed);
           }
       }

       private void InitializePanels()
       {
           if (envidoOptionsPanel != null) envidoOptionsPanel.SetActive(false);
           if (trucoOptionsPanel != null) trucoOptionsPanel.SetActive(false);
           if (responsePanel != null) responsePanel.SetActive(false);
           if (envidoSelectionPanel != null) envidoSelectionPanel.SetActive(false);
           if (envidoResolutionPanel != null) envidoResolutionPanel.SetActive(false);
       }

       private void OnAcceptButtonPressed()
       {
           currentAcceptDeclineAction?.Invoke(true);
           OnAcceptDeclineResponse?.Invoke(true);
           HideAcceptDeclinePanel();
       }

       private void OnDeclineButtonPressed()
       {
           currentAcceptDeclineAction?.Invoke(false);
           OnAcceptDeclineResponse?.Invoke(false);
           HideAcceptDeclinePanel();
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

       public void ShowBidOptions(BidValidator validator, TurnManager mgr)
       {
           if (validator == null || mgr == null)
           {
               Debug.LogError("UIManager: ShowBidOptions called with null parameters");
               return;
           }

           ClearEnvidoButtons();
           ClearTrucoButtons();

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
           
           foreach (var bidType in new[] { BidType.Envido, BidType.RealEnvido, BidType.FaltaEnvido })
           {
               var bid = bidFactory.CreateBid(bidType);
               if (validator.CanBid(bid, mgr))
                   CreateButton(bidType, envidoButtonsContainer, envidoButtons);
           }
           
           if (envidoOptionsPanel != null)
               envidoOptionsPanel.SetActive(envidoButtons.Count > 0);
           
           foreach (var bidType in new[] { BidType.Truco, BidType.ReTruco, BidType.ValeCuatro })
           {
               var bid = bidFactory.CreateBid(bidType);
               if (validator.CanBid(bid, mgr))
                   CreateButton(bidType, trucoButtonsContainer, trucoButtons);
           }
           
           if (trucoOptionsPanel != null)
               trucoOptionsPanel.SetActive(trucoButtons.Count > 0);
       }

       public void ShowEnvidoOptions(IEnumerable<BidType> bids)
       {
           if (bids == null)
           {
               Debug.LogWarning("UIManager: ShowEnvidoOptions called with null bids");
               return;
           }

           ClearEnvidoButtons();
           
           if (envidoOptionsPanel != null)
               envidoOptionsPanel.SetActive(true);
           
           foreach (var bidType in bids) 
               CreateButton(bidType, envidoButtonsContainer, envidoButtons);
       }

       public void HideEnvidoOptions()
       {
           if (envidoOptionsPanel != null)
               envidoOptionsPanel.SetActive(false);
           ClearEnvidoButtons();
       }

       public void ShowTrucoOptions(IEnumerable<BidType> bids)
       {
           if (bids == null)
           {
               Debug.LogWarning("UIManager: ShowTrucoOptions called with null bids");
               return;
           }

           ClearTrucoButtons();
           
           if (trucoOptionsPanel != null)
               trucoOptionsPanel.SetActive(true);
           
           foreach (var bidType in bids) 
               CreateButton(bidType, trucoButtonsContainer, trucoButtons);
       }

       public void HideTrucoOptions()
       {
           if (trucoOptionsPanel != null)
               trucoOptionsPanel.SetActive(false);
           ClearTrucoButtons();
       }

       public void ClearAllListeners()
       {
           OnAcceptDeclineResponse = null;
           OnBidSelected = null;
           currentAcceptDeclineAction = null;
           currentConfirmAction = null;
           currentCancelAction = null;
       }

       public void ShowEnvidoSelectionPanel(System.Action onConfirm, System.Action onCancel, int initialValue)
       {
           if (onConfirm == null || onCancel == null)
           {
               Debug.LogWarning("UIManager: ShowEnvidoSelectionPanel called with null actions");
               return;
           }

           currentConfirmAction = onConfirm;
           currentCancelAction = onCancel;

           if (envidoSelectionPanel != null)
               envidoSelectionPanel.SetActive(true);
           
           SetupEnvidoSelectionButtons();
           UpdateEnvidoSelectionValue(initialValue);
           
           if (envidoInstructionText != null)
               envidoInstructionText.text = "Select up to 2 cards for your Envido\n(Preferably of the same suit)";
       }

       private void SetupEnvidoSelectionButtons()
       {
           if (confirmEnvidoButton != null)
           {
               confirmEnvidoButton.onClick.RemoveAllListeners();
               confirmEnvidoButton.onClick.AddListener(OnConfirmEnvidoPressed);
           }

           if (cancelEnvidoButton != null)
           {
               cancelEnvidoButton.onClick.RemoveAllListeners();
               cancelEnvidoButton.onClick.AddListener(OnCancelEnvidoPressed);
           }
       }

       private void OnConfirmEnvidoPressed()
       {
           currentConfirmAction?.Invoke();
       }

       private void OnCancelEnvidoPressed()
       {
           currentCancelAction?.Invoke();
       }

       public void HideEnvidoSelectionPanel()
       {
           if (envidoSelectionPanel != null)
               envidoSelectionPanel.SetActive(false);

           CleanupEnvidoSelectionButtons();
       }

       private void CleanupEnvidoSelectionButtons()
       {
           currentConfirmAction = null;
           currentCancelAction = null;
           
           confirmEnvidoButton?.onClick.RemoveAllListeners();
           cancelEnvidoButton?.onClick.RemoveAllListeners();
       }

       public void UpdateEnvidoSelectionValue(int value)
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
               opponentValueText.text = $"Devil's Envido: {opponentValue}";
           
           if (winnerText != null)
               winnerText.text = $"Winner: {winner}";
           
           if (damageText != null)
               damageText.text = $"{bidName}: {damage} HP damage";
       }

       public void HideEnvidoResolutionPanel()
       {
           if (envidoResolutionPanel != null)
               envidoResolutionPanel.SetActive(false);
       }

       private void CreateButton(BidType type, Transform container, List<GameObject> buttonList)
       {
           if (bidButtonPrefab == null)
           {
               Debug.LogError("UIManager: bidButtonPrefab is null");
               return;
           }

           if (container == null)
           {
               Debug.LogError("UIManager: container is null");
               return;
           }

           var buttonObject = Instantiate(bidButtonPrefab, container);
           var button = buttonObject.GetComponent<Button>();
           var text = buttonObject.GetComponentInChildren<TextMeshProUGUI>();
           
           if (text != null) 
               text.text = type.ToString();
           
           if (button != null) 
           {
               button.onClick.RemoveAllListeners();
               button.onClick.AddListener(() => OnBidSelected?.Invoke(type));
           }
           
           buttonList.Add(buttonObject);
       }

       private void ClearEnvidoButtons()
       {
           ClearButtonList(envidoButtons);
       }

       private void ClearTrucoButtons()
       {
           ClearButtonList(trucoButtons);
       }

       private void ClearButtonList(List<GameObject> buttonList)
       {
           foreach (var buttonObject in buttonList)
           {
               if (buttonObject != null)
               {
                   var button = buttonObject.GetComponent<Button>();
                   button?.onClick.RemoveAllListeners();
                   Destroy(buttonObject);
               }
           }
           buttonList.Clear();
       }

       private void CleanupAllResources()
       {
           Debug.Log("UIManager: Cleaning up all resources");

           ClearAllListeners();
           ClearEnvidoButtons();
           ClearTrucoButtons();
           CleanupEnvidoSelectionButtons();

           acceptButton?.onClick.RemoveAllListeners();
           declineButton?.onClick.RemoveAllListeners();

           ServiceLocator.Unregister<IUIManager>();
       }
   }
}