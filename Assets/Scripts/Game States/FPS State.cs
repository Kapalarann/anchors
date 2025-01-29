using UnityEngine;

public class FPSState : GameState
{
    private StateData stateData;
    private Camera fpsCameraInstance;
    private FPSMovement fpsMovement;
    public FPSState(GameStateManager manager, StateData stateData) : base(manager)
    {
        this.stateData = stateData;
    }
    public override void Enter()
    {
        if (stateData == null || manager.selectedUnit == null)
        {
            Debug.LogWarning("No StateData or selectedUnit provided for FPSState!");
            manager.RequestStateChange(StateType.RTS); // Fallback if invalid
            return;
        }

        // Get the FPS camera from GameStateManager
        if (manager.cameraInstances.TryGetValue(stateData.state, out GameObject cameraObj))
        {
            fpsCameraInstance = cameraObj.GetComponent<Camera>();

            if (fpsCameraInstance != null)
            {
                // Parent the FPS camera to the selected unit
                fpsCameraInstance.transform.SetParent(manager.selectedUnit.transform);
                fpsCameraInstance.transform.localPosition = Vector3.up * manager.cameraY; // Adjust if needed
                fpsCameraInstance.transform.localRotation = Quaternion.identity;
            }
            else
            {
                Debug.LogError("FPS Camera prefab does not have a Camera component!");
            }
        }

        // Enable FPSMovement
        fpsMovement = manager.selectedUnit.GetComponent<FPSMovement>();
        if (fpsMovement != null) fpsMovement.enabled = true;

        Debug.Log("Entered FPS Mode");
    }

    public override void Exit()
    {
        // Unparent the FPS camera
        if (fpsCameraInstance != null)
        {
            fpsCameraInstance.transform.SetParent(null);
        }

        Debug.Log("Exited FPS Mode");
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
