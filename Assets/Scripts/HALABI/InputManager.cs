using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public PlayerInput playerInput;
    public InputActionMap onFoot;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        onFoot = playerInput.currentActionMap;
    }
}
