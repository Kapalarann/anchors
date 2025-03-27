using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float damage = 0f;

    private Rigidbody rb;
    private Collider col;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = rb.GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Body") || other.gameObject.CompareTag("Head"))
        {
            Transform parent = other.transform.root;
            Health hp = parent.gameObject.GetComponent<Health>();
            if (hp == null) return;

            float damageMult = 1f;
            if (other.gameObject.CompareTag("Head")) damageMult = 2f;

            hp.TakeDamage(damage * damageMult);
            rb.isKinematic = true;
            GetComponent<TrailRenderer>().enabled = false;
            col.enabled = false;
            this.enabled = false;
            transform.SetParent(parent, true);
        }
    }

    void CreateBulletImpactEffect(Collision objectWeHit)
    {
        ContactPoint contact = objectWeHit.contacts[0];
        GameObject hole = Instantiate(
            GlobalReferences.Instance.bulletImpactEffectPrefab,
            contact.point,
            Quaternion.LookRotation(contact.normal)

        );
        hole.transform.SetParent(objectWeHit.gameObject.transform);
    }
}
