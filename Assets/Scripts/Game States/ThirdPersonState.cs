using UnityEngine;

public class ThirdPersonState : GameState 
{
    public Camera TPCameraInstance;
    private Transform crystal;
    private Transform basePoint;

    private StateData stateData;

    public ThirdPersonState(GameStateManager manager, StateData stateData) : base(manager)
    {
        this.stateData = stateData;
    }

    public override void Enter()
    {
        if (stateData == null || manager.selectedUnit == null)
        {
            Debug.LogWarning("No StateData or selectedUnit provided for ThirdPersonState!");
            manager.RequestStateChange(StateType.RTS); // Switch back to RTS if invalid
            return;
        }

        if (manager.cameraInstances.TryGetValue(stateData.state, out GameObject cameraObj))
        {
            TPCameraInstance = cameraObj.GetComponentInChildren<Camera>();

            if (TPCameraInstance != null)
            {
                // Parent the camera to the miner unit
                TPCameraInstance.transform.SetParent(manager.selectedUnit.transform);
                TPCameraInstance.transform.localPosition = Vector3.zero;
                TPCameraInstance.transform.LookAt(manager.selectedUnit.transform.position + Vector3.forward);
            }
            else
            {
                Debug.LogError("TP Camera prefab does not have a Camera component!");
            }
        }

        var meleeMovement = manager.selectedUnit.GetComponent<MeleeMovement>();
        if (meleeMovement != null)
        {
            meleeMovement.enabled = true;
        }

        Debug.Log("Entered TP Mode");

    }

    public override void Exit()
    {
        // Unparent the TP camera
        if (TPCameraInstance != null)
        {
            TPCameraInstance.transform.SetParent(null);
        }

        Debug.Log("Exited Third Person Mode");
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // Example: Exit to RTS Mode
        {
            manager.RequestStateChange(StateType.RTS);
        }
    }
}
