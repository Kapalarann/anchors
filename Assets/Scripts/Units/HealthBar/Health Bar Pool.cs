using System.Collections.Generic;
using UnityEngine;

public class HealthBarPool : MonoBehaviour
{
    public static HealthBarPool Instance;

    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private int poolSize = 10;

    private Queue<GameObject> healthBarPool = new Queue<GameObject>();

    private void Awake()
    {
        Instance = this;
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(healthBarPrefab, transform);
            obj.SetActive(false);
            healthBarPool.Enqueue(obj);
        }
    }

    public GameObject GetHealthBar()
    {
        if (healthBarPool.Count > 0)
        {
            GameObject obj = healthBarPool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            GameObject obj = Instantiate(healthBarPrefab, transform);
            return obj;
        }
    }

    public void ReturnHealthBar(GameObject obj)
    {
        obj.SetActive(false);
        healthBarPool.Enqueue(obj);
    }
}
