using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private Transform healthBarContainer;

    public Transform HealthBarContainer => healthBarContainer;

    private void Awake()
    {
        Instance = this;
    }
}
