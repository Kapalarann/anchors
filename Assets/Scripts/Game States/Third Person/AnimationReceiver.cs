using System;
using UnityEngine;

public class AnimationReceiver : MonoBehaviour
{
    private PlayerAttack _playerAttack;
    public event Action<AnimationEvent> AttackEnd;

    private void Awake()
    {
        _playerAttack = GetComponent<PlayerAttack>();
    }

    // This function is called at the end of the attack animation via Animation Event
    void OnAttackEnd(AnimationEvent animationEvent)
    {

        AttackEnd?.Invoke(animationEvent);
    }
}
