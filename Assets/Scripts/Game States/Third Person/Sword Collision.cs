 using UnityEngine;

public class SwordCollision : MonoBehaviour
{
    public bool isTransfer = false;
    private Collider coll;
    public Animator _animator;
    public float damage;

    private void Start()
    {
        coll = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!coll.enabled) return;

        HealthAndStamina targetHP = other.GetComponent<HealthAndStamina>();
        if (targetHP == null) return;

        if (isTransfer && GameStateManager.Instance.currentUnit.transform == this.transform.root && targetHP.isStunned)
        {
            if (GameStateManager.Instance.TransferToTarget(transform.root, other.transform)) return;
        }

        targetHP.TakeDamage(damage);
    }
}
