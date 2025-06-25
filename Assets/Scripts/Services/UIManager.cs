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

       void Awake()
       {
           ServiceLocator.Register<IUIManager>(this);
           
           acceptButton.onClick.AddListener(() => {
               OnAcceptDeclineResponse?.Invoke(true);
               HideAcceptDeclinePanel();
           });
           declineButton.onClick.AddListener(() => {
               OnAcceptDeclineResponse?.Invoke(false);
               HideAcceptDeclinePanel();
           });
           
           envidoOptionsPanel.SetActive(false);
           trucoOptionsPanel.SetActive(false);
           responsePanel.SetActive(false);
           envidoSelectionPanel?.SetActive(false);
           envidoResolutionPanel?.SetActive(false);
       }

       public void ShowAcceptDeclinePanel(Action<bool> onResponse)
       {
           OnAcceptDeclineResponse -= onResponse;
           OnAcceptDeclineResponse += onResponse;
           responsePanel.SetActive(true);
       }

       public void HideAcceptDeclinePanel()
       {
           responsePanel.SetActive(false);
       }

       public void ShowBidOptions(BidValidator validator, TurnManager mgr)
       {
           ClearEnvidoButtons();
           ClearTrucoButtons();

           var bidFactory = ServiceLocator.Get<IBidFactory>();
           
           foreach (var t in new[] { BidType.Envido, BidType.RealEnvido, BidType.FaltaEnvido })
           {
               var b = bidFactory.CreateBid(t);
               if (validator.CanBid(b, mgr))
                   CreateButton(t, envidoButtonsContainer, envidoButtons);
           }
           envidoOptionsPanel.SetActive(envidoButtons.Count > 0);
           
           foreach (var t in new[] { BidType.Truco, BidType.ReTruco, BidType.ValeCuatro })
           {
               var b = bidFactory.CreateBid(t);
               if (validator.CanBid(b, mgr))
                   CreateButton(t, trucoButtonsContainer, trucoButtons);
           }
           trucoOptionsPanel.SetActive(trucoButtons.Count > 0);
       }

       public void ShowEnvidoOptions(IEnumerable<BidType> bids)
       {
           ClearEnvidoButtons();
           envidoOptionsPanel.SetActive(true);
           foreach (var t in bids) CreateButton(t, envidoButtonsContainer, envidoButtons);
       }

       public void HideEnvidoOptions()
       {
           envidoOptionsPanel.SetActive(false);
           ClearEnvidoButtons();
       }

       public void ShowTrucoOptions(IEnumerable<BidType> bids)
       {
           ClearTrucoButtons();
           trucoOptionsPanel.SetActive(true);
           foreach (var t in bids) CreateButton(t, trucoButtonsContainer, trucoButtons);
       }

       public void HideTrucoOptions()
       {
           trucoOptionsPanel.SetActive(false);
           ClearTrucoButtons();
       }

       public void ClearAllListeners()
       {
           OnAcceptDeclineResponse = null;
           OnBidSelected = null;
       }

       public void ShowEnvidoSelectionPanel(System.Action onConfirm, System.Action onCancel, int initialValue)
       {
           envidoSelectionPanel?.SetActive(true);
           
           confirmEnvidoButton?.onClick.RemoveAllListeners();
           cancelEnvidoButton?.onClick.RemoveAllListeners();
           
           confirmEnvidoButton?.onClick.AddListener(() => onConfirm?.Invoke());
           cancelEnvidoButton?.onClick.AddListener(() => onCancel?.Invoke());
           
           UpdateEnvidoSelectionValue(initialValue);
           
           if (envidoInstructionText != null)
               envidoInstructionText.text = "Selecciona hasta 2 cartas para tu Envido\n(Preferiblemente del mismo palo)";
       }

       public void HideEnvidoSelectionPanel()
       {
           envidoSelectionPanel?.SetActive(false);
       }

       public void UpdateEnvidoSelectionValue(int value)
       {
           if (envidoValueText != null)
               envidoValueText.text = $"Envido: {value}";
       }

       public void ShowEnvidoResolutionPanel(int playerValue, int opponentValue, string winner, int damage, List<Card> playerCards, string bidName)
       {
           envidoResolutionPanel?.SetActive(true);
           
           if (playerValueText != null)
               playerValueText.text = $"Tu Envido: {playerValue}";
           
           if (opponentValueText != null)
               opponentValueText.text = $"Envido del Diablo: {opponentValue}";
           
           if (winnerText != null)
               winnerText.text = $"Ganador: {winner}";
           
           if (damageText != null)
               damageText.text = $"{bidName}: {damage} HP de daño";
       }

       public void HideEnvidoResolutionPanel()
       {
           envidoResolutionPanel?.SetActive(false);
       }

       private void CreateButton(BidType type, Transform container, List<GameObject> list)
       {
           var go = Instantiate(bidButtonPrefab, container);
           var btn = go.GetComponent<Button>();
           var txt = go.GetComponentInChildren<TextMeshProUGUI>();
           if (txt != null) txt.text = type.ToString();
           if (btn != null) btn.onClick.AddListener(() => OnBidSelected?.Invoke(type));
           list.Add(go);
       }

       private void ClearEnvidoButtons()
       {
           foreach (var go in envidoButtons) Destroy(go);
           envidoButtons.Clear();
       }

       private void ClearTrucoButtons()
       {
           foreach (var go in trucoButtons) Destroy(go);
           trucoButtons.Clear();
       }
   }
}