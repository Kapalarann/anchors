using UnityEngine;

public class Aim : MonoBehaviour
{
    Rigidbody rigidB;
    [SerializeField]
    Camera cam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidB = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>();
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        AimLogic();
    }
    
    void AimLogic()
    {
        float _leftRightValue = Input.GetAxisRaw("Mouse X");
        float _upDownValue = Input.GetAxisRaw("Mouse Y");

        Vector3 _rotationX = new Vector3(_upDownValue, 0, 0);
        Vector3 _rotationY = new Vector3(0, _leftRightValue, 0);

        rigidB.MoveRotation(rigidB.rotation * Quaternion.Euler(_rotationY));
        cam.transform.Rotate(_rotationX / 3);
    }
}
