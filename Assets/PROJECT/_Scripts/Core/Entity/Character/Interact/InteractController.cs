using Service;
using System;
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
                }

                return;
            }
        }


        _currentInteractable = null;
        _screenPointIndicator.SetDefault();
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
        }
        else
        {
            _currentInteractable.Interact();
        }

        _audioService.Play(_select);


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
}
