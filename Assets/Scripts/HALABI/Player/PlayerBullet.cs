using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float damage = 0f;
    public float headshotMult = 1f;
    public bool isTransfer;
    public bool sticksToTarget;
    public Transform shooterTransform;

    private Rigidbody rb;
    private Collider col;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = rb.GetComponent<Collider>();
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(rb.linearVelocity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Boundary"))
        {
            Destroy(this.gameObject);
            return;
        }

        Transform parent = other.transform.root;

        if (shooterTransform == parent) return;

        HealthAndStamina hp = parent.gameObject.GetComponent<HealthAndStamina>();
        if (hp == null) return;

        if (isTransfer && GameStateManager.Instance.currentUnit.transform == shooterTransform && hp.isStunned)
        {
            if (GameStateManager.Instance.TransferToTarget(shooterTransform, parent))
            {
                Destroy(gameObject);
                return;
            }
        }

        if (other.gameObject.CompareTag("Body") || other.gameObject.CompareTag("Head"))
        {
            float damageMult = 1f;
            if (other.gameObject.CompareTag("Head")) 
            { 
                damageMult = headshotMult;
            }

            hp.TakeDamage(damage * damageMult);

            if (!sticksToTarget)
            {
                Destroy(gameObject);
                return;
            }

            if(rb != null)rb.isKinematic = true;
            TrailRenderer trail = GetComponent<TrailRenderer>();
            if(trail != null) trail.enabled = false;
            col.enabled = false;
            this.enabled = false;

            transform.SetParent(other.transform, true);
        }
    }
}
