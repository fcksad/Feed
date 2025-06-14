using UnityEngine;

public class ItemObject : InteractableObject, IUsable
{
    [SerializeField] private AnimatorOverrideController _overrideController;
    protected Camera _camera;
    private HandAnimationController _handAnimator;


    public virtual void Initialize(HandAnimationController handAnimator, Camera camera)
    {
        _handAnimator = handAnimator;
        _handAnimator.SetOverride(_overrideController);
        _camera = camera;
    }

    public virtual void Use()
    {
        _handAnimator.PlayAttack();
    }
}
