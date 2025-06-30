using GameSystems;
using Services;
using UnityEngine;

namespace Components.Cards
{
    [RequireComponent(typeof(Collider))]
    public class CardClick : MonoBehaviour
    {
        public int ownerID = 0;
        public static bool enableClicks = false;
        [SerializeField] private CardPlacementManager placementManager;
        public Card card;

        private void Awake()
        {
            placementManager = FindAnyObjectByType<CardPlacementManager>();
        }

        void OnMouseDown()
        {
            if (!enableClicks) return;
            if (ownerID != 0) return;
            
            TurnManager.Instance.OnPlayerCardPlayed(card, gameObject);
        }
    }
}