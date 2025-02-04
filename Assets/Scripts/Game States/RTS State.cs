using System;
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
        HandleMouseClick(0, hit =>
        {
            SelectableUnit unit = hit.collider.GetComponent<SelectableUnit>();
            if (unit != null) SelectUnit(unit);
        }, LayerMask.NameToLayer("Unit"), DeselectUnit);

        HandleMouseClick(1, hit =>
        {
            UnitStateManager unitBehavior = manager.selectedUnit.GetComponent<UnitStateManager>();
            if (unitBehavior != null) unitBehavior.MoveTo(hit.point);
        }, LayerMask.NameToLayer("Ground"), DoNothing);

        if (Input.GetKeyDown(KeyCode.E))
        {
            manager.RequestStateChange(manager.selectedUnit.GetComponent<UnitStats>().unitType); //request state change based on selected unit's unitType
        }
    }

    void HandleMouseClick(int button, System.Action<RaycastHit> onHit, LayerMask layer, System.Action onMiss = null)
    {
        if (Input.GetMouseButtonDown(button))
        {
            Ray ray = manager.cameraInstances[stateData.state].GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                onHit?.Invoke(hit);
            }
            else
            {
                onMiss?.Invoke();
            }
        }

        if (onHit is null)
        {
            throw new ArgumentNullException(nameof(onHit));
        }

        if (onMiss is null)
        {
            throw new ArgumentNullException(nameof(onMiss));
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

    void DoNothing()
    {

    }
}
