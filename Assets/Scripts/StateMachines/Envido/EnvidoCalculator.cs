using System.Collections.Generic;
using System.Linq;
using Components.Cards;
using UnityEngine;

namespace StateMachines.Envido
{
    public static class EnvidoCalculator
    {
        public static int CalculateEnvido(List<Card> cards)
        {
            if (!IsValidHand(cards))
                return 0;

            var validCards = cards.Where(c => c != null).ToList();
            
            if (validCards.Count == 0)
                return 0;

            return CalculateMaxEnvido(validCards);
        }

        public static int CalculateEnvidoFromSelection(List<Card> selectedCards)
        {
            if (!IsValidEnvidoSelection(selectedCards))
                return 0;

            return CalculateEnvidoFromExactCards(selectedCards);
        }

        private static int CalculateEnvidoFromExactCards(List<Card> exactCards)
        {
            var validCards = exactCards.Where(c => c != null && IsValidEnvidoCard(c)).ToList();
    
            if (validCards.Count == 0) return 0;
    
            if (validCards.Count == 1)
            {
                if (IsFigure(validCards[0].rank))
                    return 10;
                return GetEnvidoValue(validCards[0].rank);
            }
    
            if (validCards.Count == 2)
            {
                if (validCards[0].suit == validCards[1].suit)
                {
                    return 20 + GetEnvidoValue(validCards[0].rank) + GetEnvidoValue(validCards[1].rank);
                }
                return Mathf.Max(GetSingleCardValue(validCards[0]), GetSingleCardValue(validCards[1]));
            }
    
            return 0;
        }

        private static bool IsFigure(Rank rank)
        {
            return rank == Rank.Sota || rank == Rank.Caballo || rank == Rank.Rey;
        }

        private static int GetSingleCardValue(Card card)
        {
            if (IsFigure(card.rank))
                return 10;
            return GetEnvidoValue(card.rank);
        }

        public static bool IsValidEnvidoSelection(List<Card> selectedCards)
        {
            if (selectedCards == null) return false;
            if (selectedCards.Count == 0 || selectedCards.Count > 2) return false;
            if (selectedCards.Any(c => c == null)) return false;
            
            return true;
        }

        public static string GetEnvidoDescription(List<Card> cards)
        {
            if (cards == null || cards.Count == 0)
                return "Sin cartas";

            if (!IsValidEnvidoSelection(cards))
                return "Selección inválida";

            int envido = CalculateEnvidoFromSelection(cards);

            switch (cards.Count)
            {
                case 1:
                    return $"{cards[0]} = {envido} puntos";

                case 2:
                    if (cards[0].suit == cards[1].suit)
                    {
                        int val1 = GetEnvidoValue(cards[0].rank);
                        int val2 = GetEnvidoValue(cards[1].rank);
                        return $"{cards[0]} + {cards[1]} (mismo palo) = 20 + {val1} + {val2} = {envido}";
                    }
                    else
                    {
                        int max = Mathf.Max(GetEnvidoValue(cards[0].rank), GetEnvidoValue(cards[1].rank));
                        var maxCard = GetEnvidoValue(cards[0].rank) > GetEnvidoValue(cards[1].rank) ? cards[0] : cards[1];
                        return $"{cards[0]} vs {cards[1]} (diferentes palos) = Mayor: {maxCard} = {max}";
                    }

                default:
                    return "Selección inválida";
            }
        }

        #region Private Methods

        private static bool IsValidHand(List<Card> cards)
        {
            if (cards == null) return false;
            if (cards.Count == 0) return false;
            if (cards.Count > 3) 
            {
                Debug.LogWarning($"[EnvidoCalculator] Mano con más de 3 cartas: {cards.Count}");
                return false;
            }
            if (cards.Any(c => c == null))
            {
                Debug.LogWarning("[EnvidoCalculator] Mano contiene cartas null");
                return false;
            }
            
            return true;
        }



        private static int CalculateMaxEnvido(List<Card> cards)
        {
            var cardsBySuit = cards
                .Where(c => c != null && IsValidEnvidoCard(c))
                .GroupBy(c => c.suit)
                .ToDictionary(g => g.Key, g => g.ToList());

            int maxEnvido = 0;

            foreach (var suitGroup in cardsBySuit)
            {
                var cardsInSuit = suitGroup.Value;
                
                if (cardsInSuit.Count >= 2)
                {
                    var values = cardsInSuit
                        .Select(c => GetEnvidoValue(c.rank))
                        .OrderByDescending(v => v)
                        .Take(2)
                        .ToList();
                    
                    int pairEnvido = 20 + values[0] + values[1];
                    maxEnvido = Mathf.Max(maxEnvido, pairEnvido);
                }
                else if (cardsInSuit.Count == 1)
                {
                    int singleEnvido = GetEnvidoValue(cardsInSuit[0].rank);
                    maxEnvido = Mathf.Max(maxEnvido, singleEnvido);
                }
            }

            return maxEnvido;
        }

        private static bool IsValidEnvidoCard(Card card)
        {
            if (card == null) return false;
            
            int rankValue = (int)card.rank;
            return rankValue >= 1 && rankValue <= 12;
        }

        private static int GetEnvidoValue(Rank rank)
        {
            switch (rank)
            {
                case Rank.As: return 1;
                case Rank.Dos: return 2;
                case Rank.Tres: return 3;
                case Rank.Cuatro: return 4;
                case Rank.Cinco: return 5;
                case Rank.Seis: return 6;
                case Rank.Siete: return 7;
                case Rank.Sota: return 0;    
                case Rank.Caballo: return 0;   
                case Rank.Rey: return 0;  
                default: return 0;
            }
        }

        #endregion

        #region Debug Helpers

        public static void LogEnvidoCalculation(List<Card> cards)
        {
            if (!IsValidHand(cards))
            {
                Debug.Log("[EnvidoCalculator] Mano inválida para logging");
                return;
            }

            Debug.Log($"[EnvidoCalculator] Calculando envido para: {string.Join(", ", cards)}");
            
            var cardsBySuit = cards
                .Where(c => c != null && IsValidEnvidoCard(c))
                .GroupBy(c => c.suit);

            foreach (var group in cardsBySuit)
            {
                var cardsInSuit = group.ToList();
                if (cardsInSuit.Count >= 2)
                {
                    var values = cardsInSuit.Select(c => GetEnvidoValue(c.rank)).OrderByDescending(v => v).Take(2).ToList();
                    Debug.Log($"  {group.Key}: {string.Join(", ", cardsInSuit)} = 20 + {values[0]} + {values[1]} = {20 + values[0] + values[1]}");
                }
                else if (cardsInSuit.Count == 1)
                {
                    Debug.Log($"  {group.Key}: {cardsInSuit[0]} = {GetEnvidoValue(cardsInSuit[0].rank)}");
                }
            }
            
            Debug.Log($"[EnvidoCalculator] Resultado final: {CalculateEnvido(cards)}");
        }

        #endregion
    }
}