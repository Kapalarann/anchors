using UnityEngine;
using Spells;


namespace Spells
{
    public class HomingSpell : Spell
    {
        public float damage = 0f;
        public float speed = 5f;
        public float turnSpeed = 5f;
        public float lifetime = 5f;
        private Rigidbody rb;

        public override void Cast(Vector3 targetPosition)
        {
            
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
            tar.y += 1.2f;
            Vector3 desiredDirection = (tar - transform.position).normalized;

            // Gradually adjust velocity direction
            Vector3 newVelocity = Vector3.Slerp(rb.linearVelocity.normalized, desiredDirection, turnSpeed * Time.deltaTime) * speed;
            rb.linearVelocity = newVelocity;
        }
    }
}
