using System.Collections.Generic;
using Components.Cards;

namespace GameSystems.Envido
{
    public enum ComboType
    {
        Single,
        SameSuit,
        DifferentSuit
    }

    public class EnvidoCombination
    {
        public List<Card> Cards { get; set; } = new List<Card>();
        public int Points { get; set; }
        public string Description { get; set; } = "";
        public ComboType Type { get; set; }

        public static EnvidoCombination Empty()
        {
            return new EnvidoCombination { Cards = new List<Card>(), Points = 0 };
        }
    }
}