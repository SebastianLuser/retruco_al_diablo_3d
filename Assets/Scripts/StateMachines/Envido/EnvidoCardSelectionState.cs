using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Components.Cards;
using GameSystems.Envido;
using Services;

namespace StateMachines.Envido
{
    public class EnvidoCardSelectionState : IState
    {
        private TurnManager mgr;
        private EnvidoManager envidoManager;
        private List<Card> selectedCards = new List<Card>();
        private List<EnvidoCardSelector> cardSelectors = new List<EnvidoCardSelector>();
        private IUIManager uiManager;
        
        private const int MIN_SELECTED_CARDS = 1;
        private const int MAX_SELECTED_CARDS = 2;

        public EnvidoCardSelectionState(TurnManager mgr, EnvidoManager envidoManager)
        {
            this.mgr = mgr;
            this.envidoManager = envidoManager;
        }

        public void Enter()
        {
            Debug.Log("🎯 ENVIDO: Card Selection State");
            Debug.Log($"💰 Points at stake: {envidoManager.GetAccumulatedPoints()}");
            
            CardClick.enableClicks = false;
            
            SetupCardSelectors();
            ShowSelectionUI();
            
            EnvidoCardSelector.OnCardSelectionChanged += OnCardSelectionChanged;
        }

        public void Update() { }

        public void Exit()
        {
            Debug.Log("🚪 Exiting Envido Card Selection");
            
            EnvidoCardSelector.OnCardSelectionChanged -= OnCardSelectionChanged;
            DisableCardSelectors();
            HideSelectionUI();
        }

        #region Card Selection Setup

        private void SetupCardSelectors()
        {
            cardSelectors.Clear();
            selectedCards.Clear();
            
            var playerCardObjects = Object.FindObjectsByType<CardClick>(FindObjectsSortMode.None)
                .Where(cc => cc.ownerID == 0)
                .ToArray();
            
            foreach (var cardClick in playerCardObjects)
            {
                var selector = cardClick.GetComponent<EnvidoCardSelector>();
                if (selector == null)
                {
                    selector = cardClick.gameObject.AddComponent<EnvidoCardSelector>();
                }
                
                selector.SetSelectionMode(true);
                cardSelectors.Add(selector);
            }
            
            Debug.Log($"🎯 {cardSelectors.Count} cards available for selection");
        }

        private void DisableCardSelectors()
        {
            foreach (var selector in cardSelectors)
            {
                if (selector != null)
                {
                    selector.SetSelectionMode(false);
                    selector.ForceDeselect();
                }
            }
            cardSelectors.Clear();
        }

        #endregion

        #region UI Management

        private void ShowSelectionUI()
        {
            try
            {
                uiManager = ServiceLocator.Get<IUIManager>();
        
                var recommendation = EnvidoGraphSelector.GetSelectionAdvice(mgr.Player.hand);
                Debug.Log($"🎯 RECOMMENDATION: {recommendation}");
                uiManager.ShowEnvidoRecommendation(recommendation);
        
                uiManager.ShowCantosPanel(
                    onCantar: OnCantarEnvido,
                    onNoCantar: OnNoCantarEnvido
                );
        
                Debug.Log("🎯 CantosPanel shown");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"❌ Error showing UI: {ex.Message}");
            }
        }

        private void HideSelectionUI()
        {
            uiManager?.HideCantosPanel();
        }

        #endregion

        #region Selection Logic

        private void OnCardSelectionChanged(Card card, bool isSelected)
        {
            if (isSelected)
            {
                if (selectedCards.Count >= MAX_SELECTED_CARDS)
                {
                    Debug.Log($"❌ Maximum {MAX_SELECTED_CARDS} cards allowed");
                    
                    var selector = cardSelectors.FirstOrDefault(s => s.Card == card);
                    selector?.DeselectCard();
                    return;
                }
                
                selectedCards.Add(card);
                Debug.Log($"✅ Card selected: {card}. Total: {selectedCards.Count}");
            }
            else
            {
                selectedCards.Remove(card);
                Debug.Log($"❌ Card deselected: {card}. Total: {selectedCards.Count}");
            }
            
            UpdateEnvidoValue();
        }

        private void UpdateEnvidoValue()
        {
            int envidoValue = EnvidoCalculator.CalculateEnvidoFromSelection(selectedCards);
            uiManager?.UpdateEnvidoValue(envidoValue);
            
            string description = EnvidoCalculator.GetEnvidoDescription(selectedCards);
            Debug.Log($"🎯 {description}");
        }

        #endregion

        #region UI Callbacks

        private void OnCantarEnvido()
        {
            if (selectedCards.Count < MIN_SELECTED_CARDS)
            {
                Debug.Log($"❌ You must select at least {MIN_SELECTED_CARDS} card(s)");
                return;
            }

            int playerEnvidoValue = EnvidoCalculator.CalculateEnvidoFromSelection(selectedCards);
            
            Debug.Log($"🗣️ PLAYER SINGS ENVIDO: {playerEnvidoValue}");
            Debug.Log($"📋 Selected cards: {string.Join(", ", selectedCards)}");
            
            mgr.ChangeState(new EnvidoResolutionState(mgr, envidoManager, playerEnvidoValue, selectedCards));
        }

        private void OnNoCantarEnvido()
        {
            Debug.Log("🏃 Player RETIRES from Envido - Opponent wins automatically");
            
            int damage = envidoManager.GetAccumulatedPoints();
            mgr.GameService.OpponentWinsEnvidoPoints(damage);
            
            Debug.Log($"💀 Player loses {damage} HP for retiring from Envido");
            
            mgr.MarcarEnvidoComoCantado();
            mgr.TransitionToPlayState();
        }

        #endregion
    }
}