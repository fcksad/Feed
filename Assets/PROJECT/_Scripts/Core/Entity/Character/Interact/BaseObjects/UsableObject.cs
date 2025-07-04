using System.Collections.Generic;
using UnityEngine;

public class UsableObject : GrabbableObject, IUsable
{
    public Transform Transform => transform;
    public Vector3 LocalPosition => _handOffset;
    public Quaternion LocalRotation => _handRotation;

    [Header("In Hand Offset")]
    [SerializeField] private Vector3 _handOffset = Vector3.zero;
    [SerializeField] private Quaternion _handRotation = Quaternion.identity;

    [SerializeField] private AnimatorOverrideController _overrideController;
    [SerializeField] protected List<Collider> _colliders;

    protected Camera _camera;
    private HandAnimationController _handAnimator;

    public Rigidbody Rigidbody => _rb;
    public List<Collider> GetColliders() => _colliders;

    public virtual void Initialize(HandAnimationController handAnimator, Camera camera)
    {
        _handAnimator = handAnimator;
        _handAnimator.SetOverride(_overrideController);
        _camera = camera;
    }

    public virtual void OnEquip(Transform handPoint, LayerMask handMask)
    {
        Transform.SetParent(handPoint);
        Transform.localPosition = LocalPosition;
        Transform.localRotation = LocalRotation;

        SetPhysics(false);
        SetLayerRecursively(gameObject, LayerMaskToLayer(handMask));
    }

    public virtual void OnUnequip(LayerMask defaultMask)
    {
        Transform.SetParent(null);
        SetPhysics(true);
        SetLayerRecursively(gameObject, LayerMaskToLayer(defaultMask));
    }

    public virtual void Use()
    {
        _handAnimator.PlayAttack();
    }

    protected void SetPhysics(bool enable)
    {
        foreach (var col in _colliders)
            col.enabled = enable;

        _rb.isKinematic = !enable;
        _rb.interpolation = enable ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
        _rb.collisionDetectionMode = enable ? CollisionDetectionMode.Discrete : CollisionDetectionMode.Continuous;
    }

    protected void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    protected int LayerMaskToLayer(LayerMask mask)
    {
        int value = mask.value;
        for (int i = 0; i < 32; i++)
        {
            if ((value & (1 << i)) != 0)
                return i;
        }
        return 0;
    }

}
