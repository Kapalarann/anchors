using UnityEngine;
using UnityEngine.AI;

public enum UnitBehavior
{
    Gatherer,
    Melee,
    Range,
    Mage
}

public class UnitStateManager : MonoBehaviour
{
    private UnitState _currentState;
    [Header("Behavior Type")]
    [SerializeField] public UnitBehavior _unitBehavior;

    [HideInInspector] public UnitIdleState idleState = new UnitIdleState();
    [HideInInspector] public UnitMoveState moveState = new UnitMoveState();
    [HideInInspector] public UnitAttackState attackState = new UnitAttackState();

    [Header("Movement")]
    [SerializeField] public float _movementSpeed;
    public Vector3 _targetPosition;

    [HideInInspector] public UnitStats _unitStat;
    [HideInInspector] public UnitStats _target;

    [HideInInspector] public NavMeshAgent _navMeshAgent;

    private void Start()
    {
        _unitStat = GetComponent<UnitStats>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        SetState(new UnitIdleState());
    }

    public void SetState(UnitState newState)
    {
        if (_currentState != null)
        {
            _currentState.Exit(this);
        }

        _currentState = newState;
        _currentState.Enter(this);
    }

    public void Update()
    {
        _currentState?.Update(this);
    }

    public void MoveTo(Vector3 position)
    {
        _targetPosition = position;
        SetState(moveState);
    }
}
