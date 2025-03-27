using UnityEngine;

public class SwordCollision : MonoBehaviour
{
    private Collider coll;
    public Animator _animator;
    public float lDamage, hDamage;

    private void Start()
    {
        coll = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!coll.enabled) return;

        if (GameStateManager.Instance.TransferToTarget(transform.root, other)) return;

        Health targetHP = other.GetComponent<Health>();
        if (targetHP == null) return;
        
        float damage = _animator.GetCurrentAnimatorStateInfo(0).IsTag("HeavyAttack") ? hDamage : lDamage;
        targetHP.TakeDamage(damage);
        Debug.Log($"Dealt {damage} damage to {targetHP.name}.");
    }
}
