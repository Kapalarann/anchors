using UnityEngine;

public class SelectionSystem : MonoBehaviour
{
    public static SelectionSystem Instance;

    [Header("Selection Settings")]
    public LayerMask unitLayerMask;
    private Camera mainCamera;

    private SelectableUnit currentlySelectedUnit;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        HandleSelectionInput();
    }

    void HandleSelectionInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, unitLayerMask))
            {
                SelectableUnit unit = hit.collider.GetComponent<SelectableUnit>();
                if (unit != null)
                {
                    SelectUnit(unit);
                }
            }
            else
            {
                DeselectUnit();
            }
        }
    }

    void SelectUnit(SelectableUnit unit)
    {
        if (currentlySelectedUnit != null)
        {
            currentlySelectedUnit.OnDeselect();
        }

        // Select the new unit
        currentlySelectedUnit = unit;
        currentlySelectedUnit.OnSelect();
    }

    void DeselectUnit()
    {
        if (currentlySelectedUnit != null)
        {
            currentlySelectedUnit.OnDeselect();
            currentlySelectedUnit = null;
        }
    }
}
