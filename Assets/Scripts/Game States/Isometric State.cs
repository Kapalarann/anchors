using UnityEngine;

public class IsometricState : GameState
{
    private StateData stateData;
    private Camera isometricCameraInstance;
    public IsometricState(GameStateManager manager, StateData stateData) : base(manager)
    {
        this.stateData = stateData;
    }

    public override void Enter()
    {
        if (stateData == null || manager.selectedUnit == null)
        {
            Debug.LogWarning("No StateData or selectedUnit provided for IsometricState!");
            manager.RequestStateChange(StateType.RTS); // Fallback if invalid
            return;
        }

        if (manager.cameraInstances.TryGetValue(stateData.state, out GameObject cameraObj))
        {
            isometricCameraInstance = cameraObj.GetComponent<Camera>();

            if (isometricCameraInstance != null)
            {
                isometricCameraInstance.transform.position = new Vector3(manager.selectedUnit.transform.position.x, isometricCameraInstance.transform.position.y, manager.selectedUnit.transform.position.z - 15f);
            }
            else
            {
                Debug.LogError("FPS Camera prefab does not have a Camera component!");
            }
        }

        Debug.Log("Entered Isometric Mode");
    }

    public override void Exit()
    {
        Debug.Log("Exited Isometric Mode");
    }

    public override void Update()
    {
        // Exit to RTS mode
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            manager.RequestStateChange(StateType.RTS);
        }
    }
}
