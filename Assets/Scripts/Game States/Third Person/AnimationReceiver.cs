using System;
using UnityEngine;

public class AnimationReceiver : MonoBehaviour
{
    public event Action<AnimationEvent> AttackEnd;
    public event Action<AnimationEvent> BowRelease;

    // This function is called at the end of the attack animation via Animation Event
    void OnAttackEnd(AnimationEvent animationEvent)
    {
        AttackEnd?.Invoke(animationEvent);
    }

    void OnRelease(AnimationEvent animationEvent)
    {
        BowRelease?.Invoke(animationEvent);
    }
}
