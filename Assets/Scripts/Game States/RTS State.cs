using UnityEngine;

public class RTSState : GameState
{
    private StateData stateData;
    public RTSState(GameStateManager manager, StateData stateData) : base(manager)
    {
        this.stateData = stateData;
    }

    public override void Enter()
    {
        Debug.Log("Entered RTS Mode");
    }

    public override void Exit()
    {
        Debug.Log("Exited RTS Mode");
    }

    public override void Update()
    {
        // Handle unit selection
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = manager.cameraInstances[stateData.state].GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, manager.unitLayerMask))
            {
                SelectableUnit unit = hit.collider.GetComponent<SelectableUnit>();
                if (unit != null)
                {
                    SelectUnit(unit);
                }
            }
            else DeselectUnit();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            manager.RequestStateChange(manager.selectedUnit.GetComponent<UnitStats>().unitType); //request state change based on selected unit's unitType
        }
    }

    void SelectUnit(SelectableUnit unit)
    {
        if (manager.selectedUnit != null) manager.selectedUnit.OnDeselect();

        manager.selectedUnit = unit;
        manager.selectedUnit.OnSelect();
    }

    void DeselectUnit()
    {
        if (manager.selectedUnit != null)
        {
            manager.selectedUnit.OnDeselect();
            manager.selectedUnit = null;
        }
    }
}
