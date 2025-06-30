using System.Collections.Generic;
using Components.Cards;

namespace GameSystems.Envido
{
    public class OptimalEnvidoSelection
    {
        public bool IsValid { get; set; }
        public List<Card> SelectedCards { get; set; } = new List<Card>();
        public int ExpectedPoints { get; set; }
        public string Description { get; set; } = "";
        public List<EnvidoCombination> AllOptions { get; set; } = new List<EnvidoCombination>();

        public static OptimalEnvidoSelection Invalid()
        {
            return new OptimalEnvidoSelection { IsValid = false };
        }
    }
}