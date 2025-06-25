using System;
using UnityEngine;

namespace AI
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