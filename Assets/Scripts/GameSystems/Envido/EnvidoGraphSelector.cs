using System.Collections.Generic;
using System.Linq;
using Components.Cards;
using Components;
using UnityEngine;

namespace GameSystems.Envido
{
    public static class EnvidoGraphSelector
    {
        private static EnvidoOptimalSelector selector = new EnvidoOptimalSelector();

        public static EnvidoRecommendation GetOptimalCards(List<Card> hand)
        {
            var selection = selector.FindBestSelection(hand);
            
            if (!selection.IsValid)
            {
                return EnvidoRecommendation.Invalid();
            }

            var actualPoints = CalculateActualEnvido(selection.SelectedCards);

            return new EnvidoRecommendation
            {
                RecommendedCards = selection.SelectedCards,
                ExpectedPoints = actualPoints,
                Description = selection.Description,
                AllOptions = selection.AllOptions,
                IsValid = true
            };
        }

        public static List<Card> GetBestTwoCards(List<Card> hand)
        {
            var recommendation = GetOptimalCards(hand);
            
            if (!recommendation.IsValid || recommendation.RecommendedCards.Count == 0)
                return new List<Card>();
            
            return recommendation.RecommendedCards.Take(2).ToList();
        }

        public static string GetSelectionAdvice(List<Card> hand)
        {
            var recommendation = GetOptimalCards(hand);
            
            if (!recommendation.IsValid)
                return "No valid selection available";
            
            var cards = recommendation.RecommendedCards;
            if (cards.Count == 2)
            {
                if (cards[0].suit == cards[1].suit)
                {
                    return $"Select: {cards[0]} and {cards[1]} for {recommendation.ExpectedPoints} points";
                }
                else
                {
                    var bestCard = cards.OrderByDescending(c => GetSingleCardValue(c)).First();
                    return $"Select: {bestCard} for {recommendation.ExpectedPoints} points";
                }
            }
            else if (cards.Count == 1)
            {
                return $"Select: {cards[0]} for {recommendation.ExpectedPoints} points";
            }
            
            return "No optimal selection found";
        }
        private static int CalculateActualEnvido(List<Card> selectedCards)
        {
            var tempPlayer = new Player();
            foreach (var card in selectedCards)
            {
                tempPlayer.ReceiveCard(card);
            }
            
            return tempPlayer.CalculateEnvidoPoints();
        }

        private static int GetSingleCardValue(Card card)
        {
            if (IsFigure(card.rank))
                return 10;
            
            return (int)card.rank <= 7 ? (int)card.rank : 0;
        }

        private static bool IsFigure(Rank rank)
        {
            return rank == Rank.Sota || rank == Rank.Caballo || rank == Rank.Rey;
        }
    }
}