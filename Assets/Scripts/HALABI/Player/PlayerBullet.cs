using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float damage = 0f;
    public float headshotMult = 1f;
    public bool isTransfer;
    public Transform shooterTransform;

    private Rigidbody rb;
    private Collider col;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = rb.GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform parent = other.transform.root;

        if (isTransfer)
        {
            if (GameStateManager.Instance.TransferToTarget(shooterTransform, other)) return;
        }

        if (other.gameObject.CompareTag("Body") || other.gameObject.CompareTag("Head"))
        {
            Health hp = parent.gameObject.GetComponent<Health>();
            if (hp == null) return;

            float damageMult = 1f;
            if (other.gameObject.CompareTag("Head")) 
            { 
                damageMult = headshotMult;
            }

            hp.TakeDamage(damage * damageMult);
            rb.isKinematic = true;
            GetComponent<TrailRenderer>().enabled = false;
            col.enabled = false;
            this.enabled = false;

            transform.SetParent(other.transform, true);
        }
    }
}
