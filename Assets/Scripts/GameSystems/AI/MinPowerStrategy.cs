using System;
using Components;
using Components.Cards;
using UnityEngine;

namespace GameSystems.AI
{
    public class MinPowerStrategy: IAIStrategy
    {
        public Card ChooseCard(Player opponent)
        {
            Card best = null;
            foreach (var c in opponent.hand)
                if (best == null || c.GetPower() < best.GetPower())
                    best = c;
            return best;
        }
    }
}