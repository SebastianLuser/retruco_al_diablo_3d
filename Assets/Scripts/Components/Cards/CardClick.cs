using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CardClick : MonoBehaviour
{
    public int ownerID = 0;
    public static bool enableClicks = false;
    [SerializeField] private CardPlacementManager placementManager;
    public Card card;

    private void Awake()
    {
        placementManager = FindAnyObjectByType<CardPlacementManager>();
    }

    void OnMouseDown()
    {
        Debug.Log($"👆 Clic detectado en carta: {name}");

        if (!enableClicks)
        {
            Debug.Log("❌ Clicks deshabilitados.");
            return;
        }

        if (ownerID != 0)
        {
            Debug.Log("❌ Esta carta no pertenece al jugador.");
            return;
        }

        Debug.Log("✅ Ejecutando OnPlayerCardPlayed");
        TurnManager.Instance.OnPlayerCardPlayed(card, gameObject);
    }
}