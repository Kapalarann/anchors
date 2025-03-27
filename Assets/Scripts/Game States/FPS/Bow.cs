using UnityEngine;

public class Bow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public Animator bowAnimator;
    [SerializeField] public GameObject arrowPrefab;
    [SerializeField] public Transform arrowSpawnPoint;
    public Camera playerCamera;

    [Header("Bow Settings")]
    public float maxDrawTime = 1.5f;
    public float minShootForce = 10f;
    public float maxShootForce = 50f;

    private bool isDrawing = false;
    private float drawStartTime;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Start drawing bow
        {
            isDrawing = true;
            drawStartTime = Time.time;
            bowAnimator.SetBool("isHeld", true);
        }

        if (Input.GetMouseButtonUp(0) && isDrawing) // Release to shoot
        {
            isDrawing = false;
            bowAnimator.SetBool("isHeld", false);
            bowAnimator.SetTrigger("onRelease");
            ShootArrow();
        }
    }

    void ShootArrow()
    {
        float drawDuration = Mathf.Clamp(Time.time - drawStartTime, 0, maxDrawTime);
        float shootForce = Mathf.Lerp(minShootForce, maxShootForce, drawDuration / maxDrawTime);

        GameObject newArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
        Rigidbody arrowRb = newArrow.GetComponent<Rigidbody>();
        if (arrowRb != null)
        {
            arrowRb.linearVelocity = playerCamera.transform.forward * shootForce;
        }
    }
}
