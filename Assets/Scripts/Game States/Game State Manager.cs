using System.Collections.Generic;
using Unity.VisualScripting;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.AI;

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
    private List<Component> activeComponents = new List<Component>();

    [HideInInspector] public GameState currentState;
    /*[HideInInspector]*/ public SelectableUnit selectedUnit;
    [HideInInspector] public PlayerInput currentPlayerInput;
    [HideInInspector] public UnitStateManager currentUSM;
    [HideInInspector] public NavMeshAgent currentAgent;

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

        DisableActiveComponents();

        if (cameraInstances.ContainsKey(stateData.state)) cameraInstances[stateData.state].SetActive(true);
        if (uiInstances.ContainsKey(stateData.state)) uiInstances[stateData.state].SetActive(true);

        ActivateStateComponents(stateData);
        HandlePlayerInput(stateData.hasPlayerInput);
        HandleUSM();

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
            case StateType.ThirdPerson:
                currentState = new ThirdPersonState(this, stateData);
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
        if (stateDictionary.TryGetValue(newStateType, out StateData newStateData))
        {
            SwitchState(newStateData);
        }
        else
        {
            Debug.LogError($"No StateData found for {newStateType}");
        }
    }

    private void DisableActiveComponents()
    {
        foreach (var component in activeComponents)
        {
            if (component is MonoBehaviour monoBehaviour) monoBehaviour.enabled = false;
            MethodInfo deactivateMethod = component.GetType().GetMethod("DeactivateInput");
            deactivateMethod?.Invoke(component, null);
        }
        activeComponents.Clear();
    }

    private void ActivateStateComponents(StateData sData)
    {
        if (selectedUnit == null || sData == null || sData.ScriptType.Length == 0) return;

        foreach (var scriptName in sData.ScriptType)
        {
            Type scriptType = Type.GetType(scriptName);
            if (scriptType != null)
            {
                Component componentInstance = selectedUnit.GetComponent(scriptType);
                if (componentInstance != null)
                {
                    if (componentInstance is MonoBehaviour monoBehaviour)
                    {
                        monoBehaviour.enabled = true;
                    }
                    else
                    {
                        MethodInfo activateMethod = scriptType.GetMethod("ActivateInput");
                        activateMethod?.Invoke(componentInstance, null);
                    }
                    activeComponents.Add(componentInstance);
                }
            }
            else
            {
                Debug.LogError($"Invalid script type: {scriptName}");
            }
        }
    }

    private void HandlePlayerInput(bool hasPlayerInput)
    {
        if(currentPlayerInput != null)
        {
            currentPlayerInput.enabled = false;
            currentPlayerInput = null;
        }
        if(hasPlayerInput)
        {
            PlayerInput pInput = selectedUnit.GetComponent<PlayerInput>();
            if (selectedUnit != null && pInput != null)
            {
                currentPlayerInput = pInput;
                pInput.enabled = true;
            }
        }
    }

    private void HandleUSM()
    {
        if(currentUSM != null)
        {
            currentUSM.enabled = true;
            currentAgent.enabled = true;
        }
        if (selectedUnit != null)
        {
            currentUSM = selectedUnit.GetComponent<UnitStateManager>();
            if (currentUSM != null) currentUSM.enabled = false;
                
            currentAgent = selectedUnit.GetComponent<NavMeshAgent>();
            if(currentAgent != null) currentAgent.enabled = false;
        }
    }
}
