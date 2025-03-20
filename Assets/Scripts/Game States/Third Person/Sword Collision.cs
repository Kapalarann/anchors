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

        if (TransferToTarget(other)) return;

        Health targetHP = other.GetComponent<Health>();
        if (targetHP == null) return;
        
        float damage = _animator.GetCurrentAnimatorStateInfo(0).IsTag("HeavyAttack") ? hDamage : lDamage;
        targetHP.TakeDamage(damage);
        Debug.Log($"Dealt {damage} damage to {targetHP.name}.");
    }

    public bool TransferToTarget(Collider target)
    {
        if (target.gameObject == _animator.gameObject) return false;

        SelectableUnit unit = target.gameObject.GetComponent<SelectableUnit>();
        UnitStats stats = target.gameObject.GetComponent<UnitStats>();

        if (unit == null || stats == null) 
        {
            Debug.Log($"Unable to transfer to {unit.name}, not a unit");
            return false; 
        }

        GameStateManager.Instance.selectedUnit = unit;
        GameStateManager.Instance.RequestStateChange(stats.unitType);
        Debug.Log($"Transfered to {unit.name}");

        return true;
    }
}
