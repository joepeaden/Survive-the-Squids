using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PIckupSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject weaponPickupPrefab;
    private GameManager gameManager;

    public void Start()
    {
        gameManager = GameManager.instance;
        gameManager.OnNewRound.AddListener(SpawnPickup);       
    }

    private void OnDestroy()
    {
        gameManager.OnNewRound.AddListener(SpawnPickup);
    }

    public void SpawnPickup()
    {
        Vector2 spawnPos = new Vector2(Random.Range(gameManager.leftBoundary, gameManager.rightBoundary) , Random.Range(gameManager.lowerBoundary, gameManager.upperBoundary));

        Instantiate(weaponPickupPrefab, spawnPos, Quaternion.identity);
    }
}
