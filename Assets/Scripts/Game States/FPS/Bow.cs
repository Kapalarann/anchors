using UnityEngine;

public class Bow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public SkinnedMeshRenderer[] bowMesh;
    [SerializeField] public MeshRenderer[] objectMesh;
    [SerializeField] public SkinnedMeshRenderer[] characterMesh;
    [SerializeField] public Animator bowAnimator;
    [SerializeField] public GameObject arrowPrefab;
    [SerializeField] public Transform arrowSpawnPoint;
    [SerializeField] public Transform crosshair;
    public Camera playerCamera;

    [Header("Bow Settings")]
    [SerializeField] public float maxDrawTime = 1.5f;
    [SerializeField] public float minShootForce = 10f;
    [SerializeField] public float maxShootForce = 50f;
    [SerializeField] public float damage = 0f;
    [SerializeField] public float headshotMultiplier = 1f;

    public bool isTransferArrow = false;
    private bool isDrawing = false;
    private float drawStartTime;

    private void OnEnable()
    {
        foreach(SkinnedMeshRenderer mesh in bowMesh)
        {
            mesh.enabled = true;

        }
        foreach (MeshRenderer mesh in objectMesh)
        {
            mesh.enabled = false;
        }
        foreach(SkinnedMeshRenderer mesh in characterMesh)
        {
            mesh.enabled = false;
        }

        crosshair = UIManager.Instance.GetComponentInChildren<FPSCrosshair>().transform;
    }

    private void OnDisable()
    {
        foreach (SkinnedMeshRenderer mesh in bowMesh)
        {
            mesh.enabled = false;
        }
        foreach (MeshRenderer mesh in objectMesh)
        {
            mesh.enabled = true;
        }
        foreach (SkinnedMeshRenderer mesh in characterMesh)
        {
            mesh.enabled = true;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Start drawing bow
        {
            isDrawing = true;
            drawStartTime = Time.time;
            bowAnimator.SetBool("isHeld", true);
        }

        if (isDrawing && crosshair != null)
        {
            crosshair.localScale = Vector3.one * (1f - 0.8f * (Mathf.Clamp(Time.time - drawStartTime, 0, maxDrawTime) / maxDrawTime));
        }

        if (Input.GetMouseButtonUp(0) && isDrawing) // Release to shoot
        {
            isDrawing = false;
            if(crosshair != null) crosshair.localScale = Vector3.one;
            bowAnimator.SetBool("isHeld", false);
            bowAnimator.SetTrigger("onRelease");
            ShootArrow();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (isTransferArrow)
            {
                isTransferArrow = false;
                return;
            }
            else isTransferArrow = true;
        }
    }

    void ShootArrow()
    {
        float drawDuration = Mathf.Clamp(Time.time - drawStartTime, 0, maxDrawTime);
        float shootForce = Mathf.Lerp(minShootForce, maxShootForce, drawDuration / maxDrawTime);

        GameObject newArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);
        PlayerBullet arrow = newArrow.GetComponent<PlayerBullet>();

        arrow.shooterTransform = transform;
        arrow.isTransfer = isTransferArrow;
        arrow.damage = damage;
        arrow.headshotMult = headshotMultiplier;

        if (isTransferArrow) arrow.GetComponent<TrailRenderer>().colorGradient = setTrailGradient(Color.black);

        Rigidbody arrowRb = newArrow.GetComponent<Rigidbody>();
        if (arrowRb != null)
        {
            arrowRb.linearVelocity = playerCamera.transform.forward * shootForce;
        }
    }

    private Gradient setTrailGradient(Color color)
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color, 0f), new GradientColorKey(color, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(color.a, 0f), new GradientAlphaKey(color.a, 1f) }
        );

        return gradient;
    }
}
