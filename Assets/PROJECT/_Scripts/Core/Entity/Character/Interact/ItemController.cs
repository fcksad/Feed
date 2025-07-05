using Localization;
using Service;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ItemController : MonoBehaviour
{
    [SerializeField] private Transform _handPoint;
    [SerializeField] private LayerMask _defaultMask;
    [SerializeField] private LayerMask _handMask;

    private Camera _camera;
    private HandAnimationController _handAnimator;

    private readonly List<CharacterAction> _dropHints = new List<CharacterAction> { CharacterAction.Drop };
    [SerializeField] private LocalizedName _dropLocalized;

    private IUsable _usable;

    private IInputService _inputService;
    private IHintService _hintService;

    [Inject]
    public void Construct(IInputService inputService, IHintService hintService)
    {
        _inputService = inputService;
        _hintService = hintService;
    }

    public void Initialize(Camera camera , HandAnimationController handAnimationController)
    {
        _dropLocalized.Init();

        _camera = camera;
        _handAnimator = handAnimationController;

        _inputService.AddActionListener(CharacterAction.Drop, onStarted: Drop);
        _inputService.AddActionListener(CharacterAction.Attack, onStarted: Use);
    }

    public void Dispose()
    {
        _inputService.RemoveActionListener(CharacterAction.Drop, onStarted: Drop);
        _inputService.RemoveActionListener(CharacterAction.Attack, onStarted: Use);

        _dropLocalized.Dispose();
    }

    public void Equip(IUsable item)
    {
        if (_usable != null) return;

        _usable = item;
        if (_usable is UsableObject usableObject)
        {
            usableObject.Initialize(_handAnimator, _camera);
            usableObject.OnEquip(_handPoint, _handMask);
            _hintService.ShowHint(_dropLocalized.Name, _dropHints);
        }
    }

    public void Use()
    {
        _usable?.Use();
    }

    public void Drop()
    {
        if (_usable is UsableObject usableObject)
        {
            usableObject.OnUnequip(_defaultMask);
        }

        _usable = null;
        _handAnimator.ResetToDefault();
        _hintService.HideHint(_dropLocalized.Name);
    }
}
