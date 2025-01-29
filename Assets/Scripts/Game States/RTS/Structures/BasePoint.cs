using System.Resources;
using UnityEngine;

public class BasePoint : MonoBehaviour
{
    private void Start()
    {
        ResourceManager.Instance.RegisterBase(transform);
    }

    private void OnDisable()
    {
        ResourceManager.Instance.UnregisterBase(transform);
    }
}
