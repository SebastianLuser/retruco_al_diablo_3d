using Components;
using Components.Cards;
using UnityEngine;

namespace GameSystems.AI
{
    public class MaxPowerStrategy: IAIStrategy
    {
        public Card ChooseCard(Player opponent)
        {
            Card best = null;
            foreach (var c in opponent.hand)
                if (best == null || c.GetPower() > best.GetPower())
                    best = c;
            return best;
        }
    }
}