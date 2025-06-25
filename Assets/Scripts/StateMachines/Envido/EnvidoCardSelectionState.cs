using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Components.Cards;
using Match.Bids;
using Services;

namespace States
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
            Debug.Log("🎯 ENVIDO: Contexto especial - Selección de cartas");
            Debug.Log($"💰 Puntos acumulados en juego: {envidoManager.GetAccumulatedPoints()}");
            
            CardClick.enableClicks = false;
            
            SetupCardSelectors();
            ShowSelectionUI();
            
            EnvidoCardSelector.OnCardSelectionChanged += OnCardSelectionChanged;
        }

        public void Update() { }

        public void Exit()
        {
            Debug.Log("🚪 Saliendo de selección de cartas Envido");
            
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
            
            Debug.Log($"🎯 {cardSelectors.Count} cartas disponibles para selección");
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
                uiManager.ShowEnvidoSelectionPanel(
                    onConfirm: OnCantarEnvido,
                    onCancel: OnNoCantarEnvido,
                    initialValue: 0
                );
                
                Debug.Log("🎯 UI Envido mostrada - Botones: Cantar/No Cantar");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"❌ Error mostrando UI de Envido: {ex.Message}");
            }
        }

        private void HideSelectionUI()
        {
            uiManager?.HideEnvidoSelectionPanel();
        }

        #endregion

        #region Selection Logic

        private void OnCardSelectionChanged(Card card, bool isSelected)
        {
            if (isSelected)
            {
                if (selectedCards.Count >= MAX_SELECTED_CARDS)
                {
                    Debug.Log($"❌ Máximo {MAX_SELECTED_CARDS} cartas permitidas");
                    
                    var selector = cardSelectors.FirstOrDefault(s => s.Card == card);
                    selector?.DeselectCard();
                    return;
                }
                
                selectedCards.Add(card);
                Debug.Log($"✅ Carta seleccionada: {card}. Total: {selectedCards.Count}");
            }
            else
            {
                selectedCards.Remove(card);
                Debug.Log($"❌ Carta deseleccionada: {card}. Total: {selectedCards.Count}");
            }
            
            UpdateEnvidoValue();
        }

        private void UpdateEnvidoValue()
        {
            int envidoValue = States.EnvidoCalculator.CalculateEnvidoFromSelection(selectedCards);
            uiManager?.UpdateEnvidoSelectionValue(envidoValue);
            
            string description = States.EnvidoCalculator.GetEnvidoDescription(selectedCards);
            Debug.Log($"🎯 {description}");
        }

        #endregion

        #region UI Callbacks - CANTAR vs NO CANTAR

        private void OnCantarEnvido()
        {
            if (selectedCards.Count < MIN_SELECTED_CARDS)
            {
                Debug.Log($"❌ Debes seleccionar al menos {MIN_SELECTED_CARDS} carta(s)");
                return;
            }

            int playerEnvidoValue = States.EnvidoCalculator.CalculateEnvidoFromSelection(selectedCards);
            
            Debug.Log($"🗣️ JUGADOR CANTA SU TANTO: {playerEnvidoValue}");
            Debug.Log($"📋 Cartas seleccionadas: {string.Join(", ", selectedCards)}");
            
            mgr.ChangeState(new EnvidoResolutionState(mgr, envidoManager, playerEnvidoValue, selectedCards));
        }

        private void OnNoCantarEnvido()
        {
            Debug.Log("🏃 Jugador se RETIRA del Envido - Oponente gana automáticamente");
            
            int damage = envidoManager.GetAccumulatedPoints();
            mgr.GameService.OpponentWinsEnvidoPoints(damage);
            
            Debug.Log($"💀 Jugador pierde {damage} HP por retirarse del Envido");
            
            mgr.MarcarEnvidoComoCantado();
            mgr.TransitionToPlayState();
        }

        #endregion
    }
}