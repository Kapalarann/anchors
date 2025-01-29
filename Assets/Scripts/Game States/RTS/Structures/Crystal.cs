using System.Resources;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    private void Start()
    {
        ResourceManager.Instance.RegisterCrystal(transform);
    }

    private void OnDisable()
    {
        ResourceManager.Instance.UnregisterCrystal(transform);
    }
}
