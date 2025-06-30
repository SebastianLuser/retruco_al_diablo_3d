using System;
using System.Collections.Generic;
using Components.Cards;
using GameSystems;
using StateMachines.Envido;
using UnityEditor.VersionControl;
using UnityEngine;

namespace Components
{

    public class Player
    {
        public List<Card> hand = new List<Card>();
        public int points = 30;
        private Card lastPlayedCard;
        public Card GetLastPlayedCard() => lastPlayedCard;


        public void ReceiveCard(Card card)
        {
            hand.Add(card);
        }

        public void PlayCard(Card card)
        {
            if (hand.Contains(card))
            {
                hand.Remove(card);
                lastPlayedCard = card;
                Debug.Log($"🧠 Se jugó la carta: {card}");
            }
            else
            {
                Debug.Log("❌ La carta no se encuentra en la mano. No se pudo jugar.");
            }
        }

        public int CalculateEnvidoPoints()
        {
            var handCopy = new List<Card>(hand);   
            return EnvidoCalculator.CalculateEnvido(handCopy);
        }
        public void SortHand(SortType sortType)
        {
            Debug.Log($"🔄 Sorting hand by {sortType}: BEFORE = {string.Join(", ", hand)}");
    
            CardSorter.QuickSort(hand, sortType);
    
            Debug.Log($"🏆 Sorting hand by {sortType}: AFTER = {string.Join(", ", hand)}");
        }
    }
}