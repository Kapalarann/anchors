using UnityEngine;

public class MinerMovement : MonoBehaviour
{
    private CharacterController characterController;

    [Header("Settings")]
    [SerializeField] public float moveSpeed = 5f;

    [HideInInspector] public Transform cameraTransform;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        GameStateManager.Instance.RegisterMovement(StateType.Platformer, this);
    }

    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        // Get horizontal input (left/right)    
        float horizontal = -Input.GetAxis("Horizontal");

        // Calculate the direction perpendicular to the camera's forward direction
        Vector3 cameraForward = cameraTransform.transform.forward;
        cameraForward.y = 0; // Remove vertical component
        cameraForward.Normalize();

        // Perpendicular direction (to the right of the camera)
        Vector3 perpendicularDirection = Vector3.Cross(cameraForward, Vector3.up).normalized;

        // Calculate movement vector
        Vector3 moveDirection = perpendicularDirection * horizontal;

        // Move the character using the CharacterController
        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
    }
}
