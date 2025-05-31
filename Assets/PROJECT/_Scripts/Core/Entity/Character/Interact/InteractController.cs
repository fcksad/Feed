using Service;
using UnityEngine;
using Zenject;

public class InteractController : MonoBehaviour
{
    private Camera _camera;
    [SerializeField] private float _interactDistance = 3f;
    [SerializeField] private LayerMask _interactableMask;
    [SerializeField] private AudioConfig _select;
    [SerializeField] private AudioConfig _denyselect;

    private IInteractable _currentInteractable;

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

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Start()
    {
        _inputService.AddActionListener(CharacterAction.Interact, onPerformed: OnInteract);
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
            _currentInteractable.Interact();

            if (_select != null)
                _audioService.Play(_select);
        }
        else
        {
            if (_denyselect != null)
                _audioService.Play(_denyselect);
        }
    }

    private void OnDestroy()
    {
        _inputService.RemoveActionListener(CharacterAction.Interact, onPerformed: OnInteract);
    }
}
