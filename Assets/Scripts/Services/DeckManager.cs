using System;
using System.Collections;
using System.Collections.Generic;
using Cards;
using UnityEngine;

public class DeckManager : MonoBehaviour, IDeckService
{
    [SerializeField] private Transform handParent;
    [SerializeField] private CardTextureDictionary textureDict;
    [SerializeField] private int initialHandSize = 3;
    [SerializeField] private CardPlacementManager placementManager;
    [SerializeField] private Transform opponentHandParent;
    
    private Deck deck;
    private List<GameObject> hand = new List<GameObject>();
    private List<GameObject> opponentHand = new List<GameObject>();
    private Player player;
    private ICardFactory factory;
    private Player opponent;
    private List<(Card card, GameObject view)> opponentCardPairs = new List<(Card, GameObject)>();
    
    public Player PlayerInstance { get { return player; } }
    public Player OpponentInstance { get { return opponent; } }
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

    public void DrawToHand(int n)
    {
        // Player
        for (int i = 0; i < n; i++)
        {
            var c = deck.DrawCard();
            if (c == null) break;
            
            GameObject go = factory.Create(c, handParent, ownerID: 0);
            
            switch (i)
            {
                case 0:
                    go.transform.localPosition = new Vector3(0, -0.0041f, 0.0045f);
                    go.transform.localRotation = Quaternion.Euler(-20.379f, -6.595f, 3.382f);
                    break;
                case 1:
                    go.transform.localPosition = new Vector3(0, -0.0014f, 0.0211f);
                    go.transform.localRotation = Quaternion.Euler(0f, -6.595f, 3.382f);
                    break;
                case 2:
                    go.transform.localPosition = new Vector3(0, -0.0041f, 0.038f);
                    go.transform.localRotation = Quaternion.Euler(20.379f, -6.595f, 3.382f);
                    break;
            }

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
    }
    
    public void DealHand()
    {
        if (placementManager != null)
            placementManager.ResetTable();
        
        if (player != null)
            player.hand.Clear();
        ClearHand();
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
}
