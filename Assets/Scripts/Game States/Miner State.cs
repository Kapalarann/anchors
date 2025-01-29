using UnityEngine;

public class MinerState : GameState
{
    public Camera minerCameraInstance;
    private Transform crystal;
    private Transform basePoint;

    private StateData stateData;

    public MinerState(GameStateManager manager, StateData stateData) : base(manager)
    {
        this.stateData = stateData;
    }

    public override void Enter()
    {
        if (stateData == null || manager.selectedUnit == null)
        {
            Debug.LogWarning("No StateData or selectedUnit provided for MinerState!");
            manager.RequestStateChange(StateType.RTS); // Switch back to RTS if invalid
            return;
        }

        crystal = ResourceManager.Instance.GetNearestCrystal(manager.selectedUnit.transform);
        basePoint = ResourceManager.Instance.GetNearestBase(manager.selectedUnit.transform);

        // Get the Miner camera from GameStateManager
        if (manager.cameraInstances.TryGetValue(stateData.state, out GameObject cameraObj))
        {
            minerCameraInstance = cameraObj.GetComponent<Camera>();

            if (minerCameraInstance != null)
            {
                // Parent the camera to the miner unit
                minerCameraInstance.transform.SetParent(manager.selectedUnit.transform);
                minerCameraInstance.transform.localPosition = new Vector3(0, 2, -manager.cameraDistance); // Offset for better view
                minerCameraInstance.transform.LookAt(manager.selectedUnit.transform.position + Vector3.forward);
            }
            else
            {
                Debug.LogError("Miner Camera prefab does not have a Camera component!");
            }
        }

        var minerMovement = manager.selectedUnit.GetComponent<MinerMovement>();
        if (minerMovement != null)
        {
            minerMovement.enabled = true;
            minerMovement.cameraTransform = minerCameraInstance.transform;
        }

        Debug.Log("Entered Miner Mode");
        
    }

    public override void Exit()
    {
        // Unparent the miner camera
        if (minerCameraInstance != null)
        {
            minerCameraInstance.transform.SetParent(null);
        }

        Debug.Log("Exited Miner Mode");
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // Example: Exit to RTS Mode
        {
            manager.RequestStateChange(StateType.RTS);
        }
    }

}
