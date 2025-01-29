using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    public Canvas canvas;

    [Header("RTS settings")]
    [SerializeField] public LayerMask unitLayerMask;
    
    [Header("FPS settings")]
    [SerializeField] public float cameraY;

    [Header("Miner settings")]
    [SerializeField] public float cameraDistance = 10f;

    [Header("State Data")]
    public List<StateData> states; // List of all states in the game
    public Dictionary<StateType, StateData> stateDictionary; // Lookup table for states
    public Dictionary<StateType, GameObject> cameraInstances = new Dictionary<StateType, GameObject>(); // Stores camera instances
    public Dictionary<StateType, GameObject> uiInstances = new Dictionary<StateType, GameObject>(); // Stores UI instances

    [HideInInspector] public GameState currentState;
    [HideInInspector] public SelectableUnit selectedUnit;

    private MonoBehaviour activeMovementScript;
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LookForCanvas();

        // Create a lookup dictionary for states
        stateDictionary = new Dictionary<StateType, StateData>();
        foreach (var state in states) stateDictionary[state.state] = state;

        // Pre-instantiate cameras and UI but keep them disabled
        foreach (var state in states)
        {
            if (state.cameraPrefab != null)
            {
                GameObject camInstance;
                Transform existingCam = GameObject.Find(state.cameraPrefab.name)?.transform; // finds already existing copy of the camera prefab in heirarchy

                if (existingCam == null) camInstance = Instantiate(state.cameraPrefab); // if there is none, instantiate one
                else camInstance = existingCam.gameObject;

                camInstance.SetActive(false);
                cameraInstances[state.state] = camInstance;
            }

            if (state.uiPrefab != null)
            {
                GameObject uiInstance;
                Transform existingUI = canvas.transform.Find(state.uiPrefab.name); // finds already existing copy of the ui prefab in canvas

                if (existingUI == null) uiInstance = Instantiate(state.uiPrefab, canvas.transform); // if there is none, instantiate one
                else uiInstance = existingUI.gameObject;

                uiInstance.SetActive(false);
                uiInstances[state.state] = uiInstance;
            }
        }
    }

    private void Start()
    {
        if (states.Count > 0) SwitchState(states[0]); // Default to the first state
        else Debug.LogError("No states available in GameStateManager!");
    }

    private void Update()
    {
        currentState?.Update();
    }

    public void SwitchState(StateData stateData)
    {
        if (stateData == null)
        {
            Debug.LogError("Attempted to switch to a null state!");
            return;
        }

        currentState?.Exit(); // Exit current state

        // Disable old camera, UI, and movement
        foreach (var cam in cameraInstances.Values) cam.SetActive(false);
        foreach (var ui in uiInstances.Values) ui.SetActive(false);

        // Disable current movement script if it's not from the selected unit
        if (activeMovementScript != null && activeMovementScript.gameObject == selectedUnit?.gameObject)
        {
            activeMovementScript.enabled = false;
            activeMovementScript = null;
        }

        // Activate the new state's camera, UI, and movement
        if (cameraInstances.ContainsKey(stateData.state)) cameraInstances[stateData.state].SetActive(true);
        if (uiInstances.ContainsKey(stateData.state)) uiInstances[stateData.state].SetActive(true);

        // Enable movement for the selected unit
        if (stateData.hasMovement && selectedUnit != null && !string.IsNullOrEmpty(stateData.movementScriptType))
        {
            Type movementType = Type.GetType(stateData.movementScriptType);

            if (movementType != null)
            {
                MonoBehaviour newMovementScript = selectedUnit.GetComponent(movementType) as MonoBehaviour;
                if (newMovementScript != null)
                {
                    activeMovementScript = newMovementScript;
                    activeMovementScript.enabled = true;
                }
            }
            else
            {
                Debug.LogError($"Invalid movement script type: {stateData.movementScriptType}");
            }
        }

        switch (stateData.state)
        {
            case StateType.RTS:
                currentState = new RTSState(this, stateData);
                break;
            case StateType.FPS:
                currentState = new FPSState(this, stateData);
                break;
            case StateType.Platformer:
                currentState = new MinerState(this, stateData);
                break;
            default:
                Debug.LogError($"Unknown state type: {stateData.state}");
                return;
        }

        currentState.Enter(); // Enter new state
    }

    private void LookForCanvas()
    {
        // Find the Canvas in the scene
        canvas = FindFirstObjectByType<Canvas>();

        // Check if we found the Canvas
        if (canvas != null) Debug.Log("Canvas found and assigned!");
        else Debug.LogError("No Canvas found in the scene.");
    }

    // Public function for states to request a transition
    public void RequestStateChange(StateType newStateType)
    {
        if (stateDictionary.TryGetValue(newStateType, out StateData newStateData)) SwitchState(newStateData);
        else Debug.LogError($"No StateData found for {newStateType}");
    }
}
