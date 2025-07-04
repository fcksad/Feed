using Localization;
using Service;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class InteractController : MonoBehaviour
{
    [SerializeField] private float _interactDistance = 3f;
    [SerializeField] private LayerMask _includeLayerMask;
    [SerializeField] private AudioConfig _select;
    [SerializeField] private AudioConfig _denyselect;

    private Camera _camera;
    private ScreenPointIndicator _screenPointIndicator;
    private IInteractable _currentInteractable;

    private GrabController _grabController;
    private ItemController _itemController;

    private IInputService _inputService;
    private IHintService _hintService;
    private IAudioService _audioService;

    [Header("Hint")]
    private readonly Dictionary<string, List<CharacterAction>> _activeHints = new();
    private readonly List<CharacterAction> _interactHints = new List<CharacterAction>{ CharacterAction.Interact };
    [SerializeField] private LocalizedName _interactLocalized;
    [SerializeField] private LocalizedName _useLocalized;

    private readonly List<CharacterAction> _grabHints = new List<CharacterAction> { CharacterAction.Attack1 };
    [SerializeField] private LocalizedName _grabLocalized;

    [Inject]
    public void Construct(IInputService inputService, IHintService hintService, IAudioService audioService, ScreenPointIndicator screenPointIndicator)
    {
        _inputService = inputService;
        _hintService = hintService;
        _audioService = audioService;
        _screenPointIndicator = screenPointIndicator;
    }

    public void Initialize(GrabController grabController, ItemController itemController, Camera camera)
    {
        _interactLocalized.Init();
        _useLocalized.Init();
        _grabLocalized.Init();

        _grabController = grabController;
        _itemController = itemController;
        _camera = camera;

        _inputService.AddActionListener(CharacterAction.Interact, onPerformed: OnInteract);      
        _inputService.AddActionListener(CharacterAction.Attack1, onStarted: OnGrabStarted);        
        _inputService.AddActionListener(CharacterAction.Attack1, onCanceled: OnGrabCanceled);


    }

    public void Dispose()
    {
        _inputService.RemoveActionListener(CharacterAction.Interact, onPerformed: OnInteract);
        _inputService.RemoveActionListener(CharacterAction.Attack1, onStarted: OnGrabStarted);
        _inputService.RemoveActionListener(CharacterAction.Attack1, onCanceled: OnGrabCanceled);

        _interactLocalized.Dispose();
        _useLocalized.Dispose();
        _grabLocalized.Dispose();
    }

    private void FixedUpdate()
    {
        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, _interactDistance, _includeLayerMask, QueryTriggerInteraction.Ignore))
        {
            var interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                if (_currentInteractable != interactable)
                {
                    _currentInteractable = interactable;
                    _screenPointIndicator.SetTargeted(_currentInteractable.Name);

                    UpdateHint(_currentInteractable);
                }

                return;
            }
        }


        _currentInteractable = null;
        _screenPointIndicator.SetDefault();
        HideHint();
    }

    private void OnInteract()
    {
        if (_currentInteractable == null)
        {
            _audioService.Play(_denyselect);
            return;
        }

        if (_currentInteractable is IUsable usable)
        {
            _itemController.Equip(usable);
            _audioService.Play(_select);
        }
        else
        {
            _currentInteractable.Interact();
        }
    }

    private void OnGrabStarted()
    {
        if (_currentInteractable is IGrabbable grabbable)
        {
            _grabController.TryGrab(grabbable);
            _audioService.Play(_select);
        }
    }

    private void OnGrabCanceled()
    {
        if (_grabController.GetGrab() != null)
            _grabController.ReleaseGrab();
    }

    public IInteractable GetInteractableObject() => _currentInteractable;

    private void UpdateHint(IInteractable interactable)
    {
        var newHints = new Dictionary<string, List<CharacterAction>>();

        if (interactable is IUsable)
        {
            TryAddHint(_useLocalized, _interactHints, newHints);
        }
        else
        {
            TryAddHint(_interactLocalized, _interactHints, newHints);
        }

        if (interactable is IGrabbable)
        {
            TryAddHint(_grabLocalized, _grabHints, newHints);
        }



        foreach (var kvp in newHints)
        {
            if (!_activeHints.ContainsKey(kvp.Key))
                _hintService.ShowHint(kvp.Key, kvp.Value);
        }

        foreach (var kvp in _activeHints)
        {
            if (!newHints.ContainsKey(kvp.Key))
                _hintService.HideHint(kvp.Key);
        }

        _activeHints.Clear();
        foreach (var kvp in newHints)
            _activeHints[kvp.Key] = kvp.Value;
    }

    private void HideHint()
    {
        foreach (var kvp in _activeHints)
        {
            _hintService.HideHint(kvp.Key);
        }
        _activeHints.Clear();
    }

    private void TryAddHint(LocalizedName localized, List<CharacterAction> actions, Dictionary<string, List<CharacterAction>> result)
    {
        if (localized == null || string.IsNullOrEmpty(localized.Name))
            return;

        result[localized.Name] = actions;
    }
}
