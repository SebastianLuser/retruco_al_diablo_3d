using UnityEngine;

namespace AI
{
    public class RandomPowerStrategy: IAIStrategy
    {
        private System.Random rnd = new System.Random();

        public Card ChooseCard(Player opponent)
        {
            if (opponent.hand.Count == 0) return null;
            int i = rnd.Next(opponent.hand.Count);
            return opponent.hand[i];
        }
    }
}