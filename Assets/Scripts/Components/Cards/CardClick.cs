using System;
using GameSystems;
using Services;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Components.Cards
{
    [RequireComponent(typeof(Collider))]
    public class CardClick : MonoBehaviour
    {
        public int ownerID = 0;
        public static bool enableClicks = false;
        [SerializeField] private CardPlacementManager placementManager;
        public Card card;
        
        [SerializeField] private CardView _cardView;

        private void Awake()
        {
            placementManager = FindAnyObjectByType<CardPlacementManager>();
        }

        void OnMouseDown()
        {
            if (!enableClicks) return;
            if (ownerID != 0) return;

            if (!TurnManager.Instance.isUsingActives)
            {
                TurnManager.Instance.OnPlayerCardPlayed(card, gameObject);
            }
            else
            {
                switch (ActiveManager.Instance.DiceSelected)
                {
                    case 1:
                        var ranks = (Rank[])Enum.GetValues(typeof(Rank));
                        var index = Array.IndexOf(ranks, card.rank);
                        var nextIndex = (index + 1) % ranks.Length;
                        card.rank = ranks[nextIndex];
                        _cardView.Setup(card);
                        break;
                    case 2:
                        var values = Enum.GetValues(typeof(Rank));
                        var randomIndex = Random.Range(0, values.Length);
                        card.rank = (Rank)values.GetValue(randomIndex);
                        card.suit = (Suit)Random.Range(0, 4);
                        _cardView.Setup(card);
                        break;
                    case 3:
                        card.suit = (Suit)Random.Range(0, 4);
                        _cardView.Setup(card);
                        break;
                }

                TurnManager.Instance.isUsingActives = false;
                ActiveManager.Instance.OnUseDice();
            }
        }
    }
}