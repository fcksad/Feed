using Service;
using System;
using UnityEngine;
using Zenject;

public class InteractController : MonoBehaviour, IInitializable, IDisposable
{
    private Camera _camera;
    [SerializeField] private float _interactDistance = 3f;
    [SerializeField] private LayerMask _interactableMask;
    [SerializeField] private AudioConfig _select;
    [SerializeField] private AudioConfig _denyselect;

    private IInteractable _currentInteractable;

    [SerializeField] private GrabController _grabController;
    private IInputService _inputService;
    private IHintService _hintService;
    private IAudioService _audioService;

    [Inject]
    public void Construct(IInputService inputService, IHintService hintService, IAudioService audioService)
    {
        _inputService = inputService;
        _hintService = hintService;
        _audioService = audioService;
    }

    public void Initialize()
    {
        _camera = Camera.main;
        _inputService.AddActionListener(CharacterAction.Interact, onPerformed: OnInteract);
    }

    public void Dispose()
    {
        _inputService.RemoveActionListener(CharacterAction.Interact, onPerformed: OnInteract);
    }

    public IInteractable GetInteractableObject()
    {
        return _currentInteractable;
    }

    private void Update()
    {
        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, _interactDistance, _interactableMask))
        {
            var interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                if (_currentInteractable != interactable)
                {
                    ClearCurrent();

                    _currentInteractable = interactable;
                    if (_currentInteractable.Outline != null)
                        _currentInteractable.Outline.OutlineMode = Outline.Mode.Enabled;

       
                    _hintService.ShowHint(CharacterAction.Interact);
                }

                return;
            }
        }

        ClearCurrent();
        _hintService.HideHint(CharacterAction.Interact);
    }

    private void ClearCurrent()
    {
        if (_currentInteractable != null && _currentInteractable.Outline != null)
        {
            _currentInteractable.Outline.OutlineMode = Outline.Mode.Hidden;
        }

        _currentInteractable = null;
    }

    private void OnInteract()
    {
        if (_currentInteractable != null)
        {
            _grabController.TryGrabOrInteract(_currentInteractable);
            _audioService.Play(_select);
        }
        else
        {
            _audioService.Play(_denyselect);
        }
    }
}
