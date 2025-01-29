using UnityEngine;
using System.Collections.Generic;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance; // Singleton for easy access

    private HashSet<Transform> crystals = new HashSet<Transform>();
    private HashSet<Transform> bases = new HashSet<Transform>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterCrystal(Transform crystal)
    {
        if (!crystals.Contains(crystal))
        {
            crystals.Add(crystal);
        }
    }

    public void UnregisterCrystal(Transform crystal)
    {
        crystals.Remove(crystal);
    }

    public void RegisterBase(Transform basePoint)
    {
        if (!bases.Contains(basePoint))
        {
            bases.Add(basePoint);
        }
    }

    public void UnregisterBase(Transform basePoint)
    {
        bases.Remove(basePoint);
    }

    public Transform GetNearestCrystal(Transform miner)
    {
        return GetNearest(miner, crystals);
    }

    public Transform GetNearestBase(Transform miner)
    {
        return GetNearest(miner, bases);
    }

    private Transform GetNearest(Transform referencePoint, HashSet<Transform> targets)
    {
        Transform nearest = null;
        float minDistance = float.MaxValue;

        foreach (Transform target in targets)
        {
            float distance = Vector3.Distance(referencePoint.position, target.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = target;
            }
        }

        return nearest;
    }
}
