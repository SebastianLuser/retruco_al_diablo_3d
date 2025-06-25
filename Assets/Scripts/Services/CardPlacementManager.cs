using UnityEngine;

public class CardPlacementManager : MonoBehaviour, IPlacementService
{
    [SerializeField] private Transform[] playerPoints;
    [SerializeField] private Transform[] opponentPoints;
    
    private int playerIndex = 0;
    private int opponentIndex = 0;
    
    void Awake() => ServiceLocator.Register<IPlacementService>(this);
    
    public void PlacePlayerCard(GameObject card)
    {
        if (playerIndex < playerPoints.Length)
        {
            card.transform.SetParent(playerPoints[playerIndex], false);
            card.transform.localPosition = Vector3.zero;
            card.transform.localRotation = Quaternion.Euler(90, -90, 0);
            playerIndex++;
        }
        else
        {
            Debug.LogWarning("No more free slots for player.");
        }
    }
    
    public void PlaceOpponentCard(GameObject card)
    {
        if (opponentIndex < opponentPoints.Length)
        {
            card.transform.SetParent(opponentPoints[opponentIndex], false);
            card.transform.localPosition = Vector3.zero;
            card.transform.localRotation = Quaternion.Euler(90, -90, 0);
            opponentIndex++;
        }
        else
        {
            Debug.LogWarning("no more free slots for oponent.");
        }
    }
    
    public void ResetTable()
    {
        playerIndex = 0;
        opponentIndex = 0;
    }
    
    public void ClearTable()
    {
        foreach (Transform slot in playerPoints)
        {
            for (int i = slot.childCount - 1; i >= 0; i--)
            {
                Destroy(slot.GetChild(i).gameObject);
            }
        }
        
        foreach (Transform slot in opponentPoints)
        {
            for (int i = slot.childCount - 1; i >= 0; i--)
            {
                Destroy(slot.GetChild(i).gameObject);
            }
        }
    }
}

public interface IPlacementService
{
    void ResetTable();
    void PlacePlayerCard(GameObject card);
    void PlaceOpponentCard(GameObject card);
    void ClearTable();
}
