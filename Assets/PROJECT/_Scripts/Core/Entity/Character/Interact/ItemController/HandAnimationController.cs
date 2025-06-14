using UnityEngine;

public class HandAnimationController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private RuntimeAnimatorController _defaultController;

    public void SetOverride(AnimatorOverrideController overrideController)
    {
        _animator.runtimeAnimatorController = overrideController != null ? overrideController : _defaultController;
    }

    public void ResetToDefault()
    {
        _animator.runtimeAnimatorController = _defaultController;
    }

    public void PlayAttack()
    {
        _animator.SetTrigger("Attack");
    }
}
