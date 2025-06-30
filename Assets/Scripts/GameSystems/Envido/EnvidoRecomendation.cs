using System.Collections.Generic;
using Components.Cards;

namespace GameSystems.Envido
{
    public class EnvidoRecommendation
    {
        public List<Card> RecommendedCards { get; set; } = new List<Card>();
        public int ExpectedPoints { get; set; }
        public string Description { get; set; } = "";
        public List<EnvidoCombination> AllOptions { get; set; } = new List<EnvidoCombination>();
        public bool IsValid { get; set; } = true;

        public static EnvidoRecommendation Invalid()
        {
            return new EnvidoRecommendation { IsValid = false };
        }
        
        public string GetAdvice()
        {
            if (!IsValid || RecommendedCards.Count == 0)
                return "No recommendation available";
            
            if (RecommendedCards.Count == 2 && RecommendedCards[0].suit == RecommendedCards[1].suit)
            {
                return $"Select {RecommendedCards[0]} and {RecommendedCards[1]} for {ExpectedPoints} points!";
            }
            else if (RecommendedCards.Count == 1)
            {
                return $"Select {RecommendedCards[0]} for {ExpectedPoints} points";
            }
            
            return Description;
        }
    }
}