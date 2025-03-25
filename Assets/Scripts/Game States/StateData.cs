using UnityEngine;

public enum StateType
{
    RTS,
    FPS,
    ThirdPerson,
    Isometric,
    Platformer
}

[CreateAssetMenu(fileName = "StateData", menuName = "Game/State Data")]
public class StateData : ScriptableObject
{
    public StateType state;

    [Header("Camera")]
    public GameObject cameraPrefab;

    [Header("UI")]
    public GameObject uiPrefab;

    [Header("Enabled/Disabled Scripts")]
    public string[] ScriptType;
    public bool hasPlayerInput;
}
