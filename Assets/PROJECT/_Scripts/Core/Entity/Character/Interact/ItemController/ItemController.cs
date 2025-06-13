using Service;
using UnityEngine;
using Zenject;

public class ItemController : MonoBehaviour
{
    [SerializeField] private Transform _handPoint;
    private IUsable _usable;

    [SerializeField] private LayerMask _defaultMask;
    [SerializeField] private LayerMask _handMask;


    private IInputService _inputService;
    private IHintService _hintService;

    [Inject]
    public void Construct(IInputService inputService, IHintService hintService)
    {
        _inputService = inputService;
        _hintService = hintService;
    }

    private void Start()
    {
        _inputService.AddActionListener(CharacterAction.Drop, onStarted: Drop);
        _inputService.AddActionListener(CharacterAction.Attack, onStarted: Use);
    }

    private void OnDestroy()
    {
        _inputService.RemoveActionListener(CharacterAction.Drop, onStarted: Drop);
        _inputService.RemoveActionListener(CharacterAction.Attack, onStarted: Use);
    }

    public void Equip(IUsable item)
    {
        if (_usable == null)
        {
            _usable = item;
            _usable.Transform.SetParent(_handPoint);
            SetHeldObjectPhysics(false);
            _usable.Transform.localPosition = _usable.LocalPosition;
            _usable.Transform.localRotation = _usable.LocalRotation;
            SetLayerRecursively(_usable.Transform.gameObject, LayerMaskToLayer(_handMask));
        }
    }

    public void Use()
    {
        _usable?.Use();
    }

    public void Drop()
    {
        if (_usable != null)
        {
            var t = _usable.Transform;
            _usable.Transform.SetParent(null);
            SetLayerRecursively(_usable.Transform.gameObject, LayerMaskToLayer(_defaultMask));
            SetHeldObjectPhysics(true);
            _usable = null;
        }
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    private int LayerMaskToLayer(LayerMask mask)
    {
        int value = mask.value;
        for (int i = 0; i < 32; i++)
        {
            if ((value & (1 << i)) != 0)
                return i;
        }
        return 0;
    }

    private void SetHeldObjectPhysics(bool enable)
    {
        foreach (var col in _usable.GetColliders())
            col.enabled = enable;

        _usable.Rigidbody.isKinematic = !enable;
        _usable.Rigidbody.interpolation = enable ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
        _usable.Rigidbody.collisionDetectionMode = enable ? CollisionDetectionMode.Discrete : CollisionDetectionMode.Continuous;
    }
}
