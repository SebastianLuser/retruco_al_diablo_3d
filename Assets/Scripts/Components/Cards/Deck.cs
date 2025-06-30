using System.Collections.Generic;
using Components.Cards;
using UnityEngine;
using TDA;

namespace Components.Cards
{
    public class Deck
    {
        public StackTDA<Card> cards = new();
        private int capacity = 48;

        public Deck()
        {
            InitializeDeck();
            Shuffle();
        }

        public void StartNewDeck()
        {
            InitializeDeck();
            Shuffle();
        }

        public void InitializeDeck()
        {
            cards.InitializeStack(capacity);
            int countAdded = 0;
            foreach (Suit suit in System.Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in System.Enum.GetValues(typeof(Rank)))
                {
                    if (countAdded < capacity)
                    {
                        cards.Push(new Card(suit, rank));
                        countAdded++;
                    }
                }
            }
        }

        void Shuffle()
        {
            List<Card> temp = new List<Card>();
            while (!cards.IsEmpty())
            {
                temp.Add(cards.Pop());
            }

            for (int i = 0; i < temp.Count; i++)
            {
                Card tempCard = temp[i];
                int randomIndex = Random.Range(i, temp.Count);
                temp[i] = temp[randomIndex];
                temp[randomIndex] = tempCard;
            }

            for (int i = temp.Count - 1; i >= 0; i--)
            {
                cards.Push(temp[i]);
            }
        }

        public Card DrawCard()
        {
            if (cards.IsEmpty())
            {
                Debug.Log("NO cards in deck.");
                return null;
            }

            return cards.Pop();
        }
    }
}