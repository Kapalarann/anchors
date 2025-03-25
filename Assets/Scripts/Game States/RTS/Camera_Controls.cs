using UnityEngine;

public class CameraControls : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float panSpeed = 20f; // Speed of camera panning
    [SerializeField] private float panBorderThickness = 10f; // Thickness of the screen edge for mouse pan
    [SerializeField] private Vector2 panLimit = new Vector2(50f, 50f); // Limits for camera movement

    [Header("Movement Controls")]
    [SerializeField] private KeyCode _up;
    [SerializeField] private KeyCode _down;
    [SerializeField] private KeyCode _left;
    [SerializeField] private KeyCode _right;

    [Header("Zoom Settings")]
    [SerializeField] private float scrollSpeed = 10f; // Speed of zoom
    [SerializeField] private float minZoom = 5f; // Minimum zoom level
    [SerializeField] private float maxZoom = 50f; // Maximum zoom level

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 50f; // Speed of camera rotation
    [SerializeField] private float pivotDistance = 20f; // Distance from the camera to the pivot

    private Camera _camera;
    private float currentZoom;
    private Vector3 pivotPoint; // The pivot point for rotation

    private void Start()
    {
        _camera = GetComponent<Camera>();
        //currentZoom = _camera.orthographicSize;
        currentZoom = _camera.fieldOfView;
        pivotPoint = transform.position + transform.forward * pivotDistance;
    }

    private void Update()
    {
        HandleMovement();
        HandleZoom();
        HandleRotation();
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

        // Mouse Input (Screen Edge Pan)
        if (Input.mousePosition.x >= Screen.width - panBorderThickness)
            moveValue += transform.right;
        if (Input.mousePosition.x <= panBorderThickness)
            moveValue -= transform.right;
        if (Input.mousePosition.y >= Screen.height - panBorderThickness)
            moveValue += transform.up;
        if (Input.mousePosition.y <= panBorderThickness)
            moveValue -= transform.up;

        transform.position += moveValue * panSpeed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scroll * scrollSpeed * Time.deltaTime * 100f;

        _camera.fieldOfView = Mathf.Clamp(currentZoom, minZoom, maxZoom);
    }

    private void HandleRotation()
    {
        if (Input.GetMouseButton(2)) // Middle Mouse Button
        {
            float horizontal = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float vertical = -Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            // Rotate around the pivot point
            transform.RotateAround(pivotPoint, Vector3.up, horizontal);
            transform.RotateAround(pivotPoint, transform.right, vertical);

            // Recalculate the pivot distance
            pivotDistance = Vector3.Distance(transform.position, pivotPoint);
        }
    }
}
