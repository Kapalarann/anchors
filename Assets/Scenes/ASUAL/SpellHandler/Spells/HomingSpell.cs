using UnityEngine;
using Spells;


namespace Spells
{
    public class HomingSpell : Spell
    {
        public float damage = 0f;
        public float speed = 10f;
        public float lifetime = 5f;
        private Transform target;
        private bool isFired = false;
        private Rigidbody rb;

        public override void Cast(Vector3 targetPosition)
        {
            
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            isFired = true;
        }

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("Rigidbody missing from HomingSpell!");
                return;
            }

            Destroy(gameObject, lifetime);
        }

        private void FixedUpdate()
        {
            if (target == null || !isFired) return;

            Vector3 tar = target.position;
            tar.y += 1f;
            Vector3 direction = (tar - transform.position).normalized;
            rb.linearVelocity = direction * speed;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Body"))
            {
                other.transform.root.GetComponent<HealthAndStamina>()?.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
