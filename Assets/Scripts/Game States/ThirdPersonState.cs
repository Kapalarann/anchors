using Unity.Cinemachine;
using UnityEngine;

public class ThirdPersonState : GameState 
{
    public Camera TPCameraInstance;
    public CinemachineCamera CinemachineInstance;

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
            CinemachineInstance = cameraObj.GetComponentsInChildren<CinemachineCamera>()[0];

            if (CinemachineInstance != null)
            {
                CinemachineInstance.Target.TrackingTarget = manager.selectedUnit.transform;
            }
            else
            {
                Debug.LogError("TP Camera prefab does not have a cinemachine component!");
            }
        }

        Debug.Log("Entered TP Mode");

    }

    public override void Exit()
    {
        // Unparent the TP camera
        if (CinemachineInstance != null)
        {
            CinemachineInstance.Target.TrackingTarget = null;
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
