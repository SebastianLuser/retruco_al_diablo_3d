using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public GameObject coin;
    public Transform spawnCoinPoint;

    void OnMouseDown()
    {
        Instantiate(coin, spawnCoinPoint.position, Quaternion.identity);
    }
}