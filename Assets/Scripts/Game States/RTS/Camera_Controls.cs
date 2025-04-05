using UnityEngine;

public class CameraControls : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float panSpeed = 20f;
    [SerializeField] private float panBorderThickness = 10f;

    [Header("Movement Controls")]
    [SerializeField] private KeyCode _up;
    [SerializeField] private KeyCode _down;
    [SerializeField] private KeyCode _left;
    [SerializeField] private KeyCode _right;

    [Header("Zoom Settings")]
    [SerializeField] private float scrollSpeed = 10f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 50f;

    [Header("Camera Movement Boundaries")]
    [SerializeField] private Vector2 minBoundary = new Vector2(-50, -50); // X and Z min
    [SerializeField] private Vector2 maxBoundary = new Vector2(50, 50);   // X and Z max

    private Camera _camera;
    private float currentZoom;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        currentZoom = _camera.fieldOfView;
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleZoom();
    }

    private void HandleMovement()
    {
        Vector3 moveValue = Vector3.zero;

        if (Input.GetKey(_up))
            moveValue += transform.up;
        if (Input.GetKey(_down))
            moveValue -= transform.up;
        if (Input.GetKey(_right))
            moveValue += transform.right;
        if (Input.GetKey(_left))
            moveValue -= transform.right;

        if (Input.mousePosition.x >= Screen.width - panBorderThickness)
            moveValue += Vector3.right;
        if (Input.mousePosition.x <= panBorderThickness)
            moveValue -= Vector3.right;
        if (Input.mousePosition.y >= Screen.height - panBorderThickness)
            moveValue += Vector3.forward;
        if (Input.mousePosition.y <= panBorderThickness)
            moveValue -= Vector3.forward;

        Vector3 newPosition = transform.position + moveValue * panSpeed * Time.deltaTime;

        newPosition.x = Mathf.Clamp(newPosition.x, minBoundary.x, maxBoundary.x);
        newPosition.z = Mathf.Clamp(newPosition.z, minBoundary.y, maxBoundary.y);

        transform.position = newPosition;
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scroll * scrollSpeed * Time.deltaTime * 100f;
        _camera.fieldOfView = Mathf.Clamp(currentZoom, minZoom, maxZoom);
    }
}
