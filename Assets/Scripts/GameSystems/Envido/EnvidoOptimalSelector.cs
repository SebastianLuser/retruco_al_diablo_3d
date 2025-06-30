using System;
using System.Collections.Generic;
using System.Linq;
using Components.Cards;
using TDA.Graphs;

namespace GameSystems.Envido
{
    public class EnvidoOptimalSelector
    {
        private readonly Graph<string> graph;
        private readonly Dictionary<string, Card> cardLookup;
        private const int MAX_WEIGHT = 1000;

        public EnvidoOptimalSelector()
        {
            graph = new Graph<string>(20);
            cardLookup = new Dictionary<string, Card>();
        }

        public OptimalEnvidoSelection FindBestSelection(List<Card> hand)
        {
            if (hand == null || hand.Count == 0)
                return OptimalEnvidoSelection.Invalid();

            ClearGraph();
            var allCombinations = GenerateAllCombinations(hand);
            var bestCombo = FindOptimalCombination(allCombinations);
            
            return new OptimalEnvidoSelection
            {
                IsValid = true,
                SelectedCards = bestCombo.Cards,
                ExpectedPoints = bestCombo.Points,
                Description = bestCombo.Description,
                AllOptions = allCombinations.OrderByDescending(c => c.Points).ToList()
            };
        }

        private void ClearGraph()
        {
            graph.Clear();
            cardLookup.Clear();
        }

        private List<EnvidoCombination> GenerateAllCombinations(List<Card> hand)
        {
            var combinations = new List<EnvidoCombination>();
            
            for (int i = 0; i < hand.Count; i++)
            {
                var singleCards = new List<Card> { hand[i] };
                combinations.Add(new EnvidoCombination
                {
                    Cards = singleCards,
                    Points = CalculateEnvidoForSelection(singleCards),
                    Description = $"Single: {hand[i]} = {CalculateEnvidoForSelection(singleCards)} points",
                    Type = ComboType.Single
                });
            }
            
            for (int i = 0; i < hand.Count; i++)
            {
                for (int j = i + 1; j < hand.Count; j++)
                {
                    var twoCards = new List<Card> { hand[i], hand[j] };
                    var points = CalculateEnvidoForSelection(twoCards);
                    var isSameSuit = hand[i].suit == hand[j].suit;
                    
                    combinations.Add(new EnvidoCombination
                    {
                        Cards = twoCards,
                        Points = points,
                        Description = GenerateTwoCardDescription(hand[i], hand[j], points, isSameSuit),
                        Type = isSameSuit ? ComboType.SameSuit : ComboType.DifferentSuit
                    });
                }
            }
            
            return combinations;
        }

        private EnvidoCombination FindOptimalCombination(List<EnvidoCombination> combinations)
        {
            BuildGraphFromCombinations(combinations);
            var optimalPath = FindShortestPath();
            
            var bestCombo = combinations.OrderByDescending(c => c.Points).FirstOrDefault();
            return bestCombo ?? EnvidoCombination.Empty();
        }

        private void BuildGraphFromCombinations(List<EnvidoCombination> combinations)
        {
            var start = "START";
            var end = "END";
            
            graph.AddVertex(start);
            graph.AddVertex(end);
            
            foreach (var combo in combinations)
            {
                string comboId = GetComboId(combo);
                graph.AddVertex(comboId);
                
                int weight = MAX_WEIGHT - combo.Points;
                graph.AddEdge(start, comboId, weight);
                graph.AddEdge(comboId, end, 0);
            }
        }

        private List<string> FindShortestPath()
        {
            var dijkstraResult = DijkstraAlgorithm.FindShortestPaths(graph, "START");
            return dijkstraResult.GetPath("END");
        }

        private int CalculateEnvidoForSelection(List<Card> selectedCards)
        {
            if (selectedCards.Count == 1)
            {
                var card = selectedCards[0];
                return GetSingleCardEnvidoValue(card);
            }
            
            if (selectedCards.Count == 2)
            {
                var card1 = selectedCards[0];
                var card2 = selectedCards[1];
                
                if (card1.suit == card2.suit)
                {
                    return 20 + GetCardEnvidoValue(card1) + GetCardEnvidoValue(card2);
                }
                else
                {
                    var val1 = GetSingleCardEnvidoValue(card1);
                    var val2 = GetSingleCardEnvidoValue(card2);
                    return Math.Max(val1, val2);
                }
            }
            
            return 0;
        }

        private int GetCardEnvidoValue(Card card)
        {
            if (IsFigure(card.rank))
                return 0;
            
            return (int)card.rank <= 7 ? (int)card.rank : 0;
        }

        private int GetSingleCardEnvidoValue(Card card)
        {
            if (IsFigure(card.rank))
                return 10;
            
            return (int)card.rank <= 7 ? (int)card.rank : 0;
        }

        private bool IsFigure(Rank rank)
        {
            return rank == Rank.Sota || rank == Rank.Caballo || rank == Rank.Rey;
        }

        private string GenerateTwoCardDescription(Card card1, Card card2, int points, bool sameSuit)
        {
            if (sameSuit)
            {
                var val1 = GetCardEnvidoValue(card1);
                var val2 = GetCardEnvidoValue(card2);
                return $"Select: {card1} + {card2} = 20 + {val1} + {val2} = {points}";
            }
            else
            {
                var val1 = GetSingleCardEnvidoValue(card1);
                var val2 = GetSingleCardEnvidoValue(card2);
                var bestCard = val1 > val2 ? card1 : card2;
                return $"Best single: {bestCard} = {points} points";
            }
        }

        private string GetComboId(EnvidoCombination combo)
        {
            return $"COMBO_{combo.Cards.Count}_{combo.Points}_{combo.Type}";
        }
    }
}