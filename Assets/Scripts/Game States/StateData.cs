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

    [Header("Movement")]
    public MonoBehaviour movementScript;
}
