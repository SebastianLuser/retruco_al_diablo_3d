using UnityEngine;
using System;

namespace Components.Cards
{
    public class EnvidoCardSelector : MonoBehaviour
    {
        [SerializeField] private float hoverHeight = 0.02f;
        [SerializeField] private float selectedHeight = 0.05f;
        [SerializeField] private float animationSpeed = 5f;
        
        [SerializeField] private GameObject selectionOutline;
        [SerializeField] private Color selectedColor = Color.yellow;
        [SerializeField] private Color hoverColor = Color.white;
        
        public static event Action<Card, bool> OnCardSelectionChanged;
        
        private bool isSelected = false;
        private bool isHovered = false;
        private bool selectionModeActive = false;
        
        private Card card;
        private Vector3 originalPosition;
        private Vector3 targetPosition;
        private CardClick cardClick;
        private Renderer cardRenderer;
        private Color originalColor;

        void Awake()
        {
            cardClick = GetComponent<CardClick>();
            cardRenderer = GetComponentInChildren<Renderer>();
            
            if (cardClick != null)
                card = cardClick.card;
                
            if (cardRenderer != null)
                originalColor = cardRenderer.material.color;
        }

        void Start()
        {
            originalPosition = transform.localPosition;
            targetPosition = originalPosition;
            
            if (selectionOutline != null)
                selectionOutline.SetActive(false);
        }

        void Update()
        {
            if (Vector3.Distance(transform.localPosition, targetPosition) > 0.001f)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * animationSpeed);
            }
        }
        
        public void SetSelectionMode(bool active)
        {
            selectionModeActive = active;
            
            if (!active)
            {
                DeselectCard();
                SetHovered(false);
            }
            
            Debug.Log($"🎯 Modo selección {(active ? "ACTIVADO" : "DESACTIVADO")} para carta {card}");
        }

        public void SelectCard()
        {
            if (!selectionModeActive || isSelected) return;
            
            isSelected = true;
            targetPosition = originalPosition + Vector3.up * selectedHeight;
            
            UpdateVisualFeedback();
            OnCardSelectionChanged?.Invoke(card, true);
            
            Debug.Log($"✅ Carta seleccionada: {card}");
        }

        public void DeselectCard()
        {
            if (!isSelected) return;
            
            isSelected = false;
            targetPosition = isHovered ? originalPosition + Vector3.up * hoverHeight : originalPosition;
            
            UpdateVisualFeedback();
            OnCardSelectionChanged?.Invoke(card, false);
            
            Debug.Log($"❌ Carta deseleccionada: {card}");
        }

        public void ForceDeselect()
        {
            isSelected = false;
            isHovered = false;
            targetPosition = originalPosition;
            UpdateVisualFeedback();
        }
        
        void OnMouseEnter()
        {
            if (!selectionModeActive || isSelected) return;
            
            SetHovered(true);
        }

        void OnMouseExit()
        {
            if (!selectionModeActive || isSelected) return;
            
            SetHovered(false);
        }

        void OnMouseDown()
        {
            if (!selectionModeActive) return;
            
            if (isSelected)
            {
                DeselectCard();
            }
            else
            {
                SelectCard();
            }
        }

        private void SetHovered(bool hovered)
        {
            isHovered = hovered;
            
            if (!isSelected)
            {
                targetPosition = hovered ? originalPosition + Vector3.up * hoverHeight : originalPosition;
                UpdateVisualFeedback();
            }
        }

        private void UpdateVisualFeedback()
        {
            if (selectionOutline != null)
                selectionOutline.SetActive(isSelected);
            
            if (cardRenderer != null)
            {
                Color targetColor = originalColor;
                
                if (isSelected)
                    targetColor = selectedColor;
                else if (isHovered)
                    targetColor = Color.Lerp(originalColor, hoverColor, 0.3f);
                
                cardRenderer.material.color = targetColor;
            }
        }
        
        public bool IsSelected => isSelected;
        public Card Card => card;
    }
}