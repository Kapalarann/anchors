using UnityEngine;
using Spells;


namespace Spells
{
    public class HomingSpell : Spell
    {
        public float speed = 10f;
        public float lifetime = 5f;
        private Transform target;
        private bool isFired = false;
        private Rigidbody rb;

        public override void Cast(Vector3 targetPosition)
        {
            GameObject enemy = FindClosestEnemy(targetPosition);
            if (enemy != null)
            {
                SetTarget(enemy.transform);
            }
            else
            {
                Debug.Log("No enemy found!");
                Destroy(gameObject);
            }
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

            Vector3 direction = (target.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }

        private GameObject FindClosestEnemy(Vector3 position)
        {
            Collider[] enemies = Physics.OverlapSphere(position, 10f, LayerMask.GetMask("Enemy"));
            if (enemies.Length > 0)
            {
                return enemies[0].gameObject;
            }
            return null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                Debug.Log($"Homing spell hit {other.name}!");
                other.GetComponent<EnemyStats>()?.TakeDamage(20);
                Destroy(gameObject);
            }
        }
    }
}
