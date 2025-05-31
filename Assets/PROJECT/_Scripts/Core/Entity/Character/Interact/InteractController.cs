using UnityEngine;

public class InteractController : MonoBehaviour
{
    private Camera _camera;
    [SerializeField] private float _interactDistance = 3f;
    [SerializeField] private LayerMask _interactableMask;

    private IInteractable _currentInteractable;

    private void Awake()
    {
        _camera = Camera.main;
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

                    _currentInteractable.Interact();
                }

                return;
            }
        }

        ClearCurrent();
    }

    private void ClearCurrent()
    {
        if (_currentInteractable != null && _currentInteractable.Outline != null)
        {
            _currentInteractable.Outline.OutlineMode = Outline.Mode.Hidden;
        }

        _currentInteractable = null;
    }
}
