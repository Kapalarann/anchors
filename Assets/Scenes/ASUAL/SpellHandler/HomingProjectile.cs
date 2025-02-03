using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 5f;
    private Transform target;
    private bool targetSet = false;
    private Rigidbody rb;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        targetSet = true;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody missing from HomingProjectile!");
            return;
        }

        Destroy(gameObject, lifetime); 
    }

    void FixedUpdate()
    {
        if (target == null || !targetSet)
        {
            return; 
        }

       
        Vector3 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = direction * speed;

       
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Projectile hit enemy: " + other.gameObject.name); 
            Destroy(gameObject); 
        }
    }
}
