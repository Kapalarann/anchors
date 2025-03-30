using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField] public List<UnitSpawn> unitSpawn;
    [SerializeField] public List<Transform> spawnPoints;

    [Header("Spawn Settings")]
    [SerializeField] public float spawnCheckInterval = 2f;
    [SerializeField] public float maxSpawnInterval = 10f;
    [SerializeField] public float minSpawnDistance = 10f;
    [SerializeField] public float maxSpawnDistance = 30f;

    [Header("Dynamic Scaling")]
    [SerializeField] public int startMaxUnits = 5;
    [SerializeField] public float secondsPerMaxGrowth = 30f;

    private int currentMax;

    private void Start()
    {
        currentMax = startMaxUnits;
        StartCoroutine(GrowMaxEnemies());
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator GrowMaxEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(secondsPerMaxGrowth);
            currentMax++;  // Increase max enemy cap every interval
        }
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            float spawnInterval = Mathf.Lerp(spawnCheckInterval, maxSpawnInterval, (float)UnitStats.units.Count / currentMax);
            yield return new WaitForSeconds(spawnInterval);

            if (UnitStats.units.Count < currentMax)
            {
                SpawnEnemy();
            }
        }
    }

    private void SpawnEnemy()
    {
        if (GameStateManager.Instance.currentAgent == null) return;

        Transform spawnLocation = GetValidSpawnPoint();
        if (spawnLocation != null)
        {
            GameObject selectedUnit = GetWeightedRandomUnit();
            if (selectedUnit != null)
            {
                Instantiate(selectedUnit, spawnLocation.position, Quaternion.identity);
            }
        }
    }

    private Transform GetValidSpawnPoint()
    {
        List<Transform> validSpawns = new List<Transform>();

        foreach (Transform spawnPoint in spawnPoints)
        {
            float distance = Vector3.Distance(GameStateManager.Instance.currentAgent.gameObject.transform.position, spawnPoint.position);

            if (distance >= minSpawnDistance && distance <= maxSpawnDistance && !IsVisible(spawnPoint.position))
            {
                validSpawns.Add(spawnPoint);
            }
        }

        if (validSpawns.Count <= 0) return null;
            
        return validSpawns[UnityEngine.Random.Range(0, validSpawns.Count)];
    }

    private bool IsVisible(Vector3 position)
    {
        Vector3 screenPoint = GameStateManager.Instance.currentCamera.WorldToViewportPoint(position);
        bool onScreen = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1 && screenPoint.z > 0;

        if (onScreen)
        {
            RaycastHit hit;
            if (Physics.Linecast(GameStateManager.Instance.currentCamera.transform.position, position, out hit))
            {
                return hit.point == position; // Ensures it’s not blocked by terrain
            }
        }

        return false;
    }

    private GameObject GetWeightedRandomUnit()
    {
        float totalWeight = 0f;
        foreach (var unit in unitSpawn) totalWeight += unit.weight;

        float randomValue = UnityEngine.Random.Range(0, totalWeight);
        foreach (var unit in unitSpawn)
        {
            if (randomValue < unit.weight)
            {
                return unit.unitPrefab;
            }
            randomValue -= unit.weight;
        }

        return null; // This should never happen if weights are correctly set
    }
}

[Serializable]
public class UnitSpawn
{
    public GameObject unitPrefab;
    public float weight;
}
