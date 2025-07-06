using System.Collections;
using System.Collections.Generic;
using Components;
using Components.Cards;
using GameSystems;
using UnityEngine;

namespace Services
{
    public class DeckManager : MonoBehaviour, IDeckService
    {
        [SerializeField] private Transform handParent;
        [SerializeField] private CardTextureDictionary textureDict;
        [SerializeField] private int initialHandSize = 3;
        [SerializeField] private CardPlacementManager placementManager;
        [SerializeField] private Transform opponentHandParent;

        private Deck deck;
        [SerializeField] private List<GameObject> hand = new List<GameObject>();
        [SerializeField] private List<GameObject> opponentHand = new List<GameObject>();
        private Player player;
        private ICardFactory factory;
        private Player opponent;
        private List<(Card card, GameObject view)> opponentCardPairs = new List<(Card, GameObject)>();

        private static readonly Vector3[] handPositions = new Vector3[3]
        {
            new Vector3(0, -0.0041f, 0.0045f),
            new Vector3(0, -0.0014f, 0.0211f),
            new Vector3(0, -0.0041f, 0.038f)
        };

        public Player PlayerInstance
        {
            get { return player; }
        }

        public Player OpponentInstance
        {
            get { return opponent; }
        }

        public Player Opponent => opponent;
        public Player Player => player;
        public Transform OpponentHandParent => opponentHandParent;
        public List<GameObject> OpponentCardObjects => opponentHand;
        public List<(Card, GameObject)> OpponentCardPairs => opponentCardPairs;

        private void Awake()
        {
            ServiceLocator.Register<IDeckService>(this);
            deck = new Deck();
            player = new Player();
            opponent = new Player();
            factory = ServiceLocator.Get<ICardFactory>();
        }

        public void SortPlayerHand(SortType sortType)
        {
            Debug.Log($"🎯 Sorting player hand by {sortType}");

            if (player.hand.Count == 0)
            {
                Debug.LogWarning("❌ No cards in player hand to sort");
                return;
            }

            // PASO 1: Sort the logical hand
            player.SortHand(sortType);

            // PASO 2: Reorder visual GameObjects to match
            for (int i = 0; i < player.hand.Count; i++)
            {
                Card targetCard = player.hand[i];
                GameObject cardObject = FindPlayerCardObject(targetCard);

                if (cardObject != null)
                {
                    SetCardPositionAndRotation(cardObject, i);
                    Debug.Log($"✅ Moved {targetCard} to position {i}");
                }
                else
                {
                    Debug.LogWarning($"❌ Could not find GameObject for card {targetCard}");
                }
            }

            Debug.Log($"🏆 Hand sorted successfully by {sortType}");
        }

        private GameObject FindPlayerCardObject(Card targetCard)
        {
            // Search through all player card GameObjects
            foreach (Transform child in handParent)
            {
                CardClick cardClick = child.GetComponent<CardClick>();
                if (cardClick != null && cardClick.card != null &&
                    cardClick.card.suit == targetCard.suit &&
                    cardClick.card.rank == targetCard.rank)
                {
                    return child.gameObject;
                }
            }

            return null;
        }

        private void SetCardPositionAndRotation(GameObject cardObject, int index)
        {
            switch (index)
            {
                case 0:
                    cardObject.transform.localPosition = new Vector3(0, -0.0041f, 0.0045f);
                    cardObject.transform.localRotation = Quaternion.Euler(-20.379f, -6.595f, 3.382f);
                    break;
                case 1:
                    cardObject.transform.localPosition = new Vector3(0, -0.0014f, 0.0211f);
                    cardObject.transform.localRotation = Quaternion.Euler(0f, -6.595f, 3.382f);
                    break;
                case 2:
                    cardObject.transform.localPosition = new Vector3(0, -0.0041f, 0.038f);
                    cardObject.transform.localRotation = Quaternion.Euler(20.379f, -6.595f, 3.382f);
                    break;
                default:
                    Debug.LogWarning($"Invalid hand position index: {index}");
                    break;
            }
        }

        public void DrawToHand(int n)
        {
            // Player
            for (int i = 0; i < n; i++)
            {
                var c = deck.DrawCard();
                if (c == null) break;

                GameObject go = factory.Create(c, handParent, ownerID: 0);

                SetCardPositionAndRotation(go, i);

                player.ReceiveCard(c);
                hand.Add(go);
            }

            // Opponent
            for (int i = 0; i < n; i++)
            {
                var c = deck.DrawCard();
                if (c == null) break;
                opponent.ReceiveCard(c);

                GameObject go = factory.Create(c, opponentHandParent, ownerID: 1);

                switch (i)
                {
                    case 0:
                        go.transform.localPosition = new Vector3(0, 0.0157f, -0.0364f);
                        go.transform.localRotation = Quaternion.Euler(-20.379f, -6.595f, 3.382f);
                        break;
                    case 1:
                        go.transform.localPosition = new Vector3(0, 0.0184f, -0.0198f);
                        go.transform.localRotation = Quaternion.Euler(0f, -6.595f, 3.382f);
                        break;
                    case 2:
                        go.transform.localPosition = new Vector3(0, 0.0157f, -0.0029f);
                        go.transform.localRotation = Quaternion.Euler(20.379f, -6.595f, 3.382f);
                        break;
                }

                opponentHand.Add(go);
                opponentCardPairs.Add((c, go));
            }

            if (PassiveManager.Instance.IsSwapCardPassive && TurnManager.Instance.actualRound % 2 == 0)
            {
                Debug.Log("SwapCards");
                StartCoroutine(SwapDelayed());
            }
        }

        private IEnumerator SwapDelayed()
        {
            Debug.Log("Before Swap");
            yield return new WaitForSeconds(1f);
            Debug.Log("On Swap");
            
            var opponentIndex = Random.Range(0, opponentHand.Count);
            var playerIndex = Random.Range(0, hand.Count);


            opponent.SwapCard(player.SwapCard(opponent.hand[opponentIndex], playerIndex), opponentIndex);

            (opponentHand[opponentIndex], hand[playerIndex]) = (hand[playerIndex], opponentHand[opponentIndex]);
            opponentCardPairs[opponentIndex] = (opponent.hand[opponentIndex], opponentHand[opponentIndex]);
            
            hand[playerIndex].transform.parent = handParent;
            hand[playerIndex].GetComponent<CardView>().Owner = 0;
            hand[playerIndex].GetComponent<CardClick>().ownerID = 0;
            SetCardPositionAndRotation(hand[playerIndex], playerIndex);
            
            var enemyGO = opponentCardPairs[opponentIndex].view;
            enemyGO.transform.parent = opponentHandParent;
            enemyGO.GetComponent<CardView>().Owner = 1;
            enemyGO.GetComponent<CardClick>().ownerID = 1;
            switch (opponentIndex)
            {
                case 0:
                    enemyGO.transform.localPosition = new Vector3(0, 0.0157f, -0.0364f);
                    enemyGO.transform.localRotation = Quaternion.Euler(-20.379f, -6.595f, 3.382f);
                    break;
                case 1:
                    enemyGO.transform.localPosition = new Vector3(0, 0.0184f, -0.0198f);
                    enemyGO.transform.localRotation = Quaternion.Euler(0f, -6.595f, 3.382f);
                    break;
                case 2:
                    enemyGO.transform.localPosition = new Vector3(0, 0.0157f, -0.0029f);
                    enemyGO.transform.localRotation = Quaternion.Euler(20.379f, -6.595f, 3.382f);
                    break;
            }
        }

        public void DealHand()
        {
            if (placementManager != null)
                placementManager.ResetTable();

            if (player != null)
                player.hand.Clear();
            if (opponent != null)
                opponent.hand.Clear();

            ClearHand();

            StartCoroutine(DealHandDelayed());
        }

        private IEnumerator DealHandDelayed()
        {
            yield return null;

            deck.StartNewDeck();
            DrawToHand(initialHandSize);
        }

        private void ClearHand()
        {
            foreach (var go in hand)
                Destroy(go);
            foreach (Transform child in opponentHandParent)
                Destroy(child.gameObject);
            hand.Clear();
            opponentHand.Clear();
            opponentCardPairs.Clear();
        }
    }

    public interface IDeckService
    {
        void DealHand();
        Player Player { get; }
        Player Opponent { get; }
        Transform OpponentHandParent { get; }
        void SortPlayerHand(SortType sortType);
    }
}