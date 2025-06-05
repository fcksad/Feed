using Service;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using Zenject;

public class GrabController : MonoBehaviour, IInitializable, IDisposable
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _holdPoint;
    [SerializeField] private float _throwForce = 5f;
    [SerializeField] private float _rotateSpeed = 100f;

    [SerializeField] private LocalizedString _localizedString;
    private string _actionName;
    private readonly List<CharacterAction> _grabHints = new()
    {
         CharacterAction.Attack, CharacterAction.Attack1,
         CharacterAction.RotateLeft, CharacterAction.RotateRight,
         CharacterAction.RotateForward, CharacterAction.RotateBackward
    };

    private IInputService _inputService;
    private IHintService _hintService;

    private IGrabbable _holdObject;
    private Coroutine _rotateRoutine;

    private Action _onRotateLeft;
    private Action _onRotateRight;
    private Action _onRotateForward;
    private Action _onRotateBackward;

    [Inject]
    public void Construct(IInputService inputService, IHintService hintService)
    {
        _inputService = inputService;
        _hintService = hintService;
    }

    public void Initialize()
    {
        _inputService.AddActionListener(CharacterAction.Attack, onStarted: Throw);
        _inputService.AddActionListener(CharacterAction.Attack1, onStarted: Drop);
        _inputService.AddActionListener(CharacterAction.RotateLeft, onStarted: _onRotateLeft = () => StartRotate(Vector3.right), onCanceled: StopRotate);
        _inputService.AddActionListener(CharacterAction.RotateRight, onStarted: _onRotateRight = () => StartRotate(Vector3.left), onCanceled: StopRotate);
        _inputService.AddActionListener(CharacterAction.RotateForward, onStarted: _onRotateForward = () => StartRotate(Vector3.forward), onCanceled: StopRotate);
        _inputService.AddActionListener(CharacterAction.RotateBackward, onStarted: _onRotateBackward = () => StartRotate(Vector3.back), onCanceled: StopRotate);

        UpdateLocal();
    }

    private void UpdateLocal()
    {
        _localizedString.StringChanged += name =>
        {
            _actionName = name;
        };

        _localizedString.RefreshString();
    }

    public void Dispose()
    {
        _inputService.RemoveActionListener(CharacterAction.Attack, onStarted: Throw);
        _inputService.RemoveActionListener(CharacterAction.Attack1, onStarted: Drop);
        _inputService.RemoveActionListener(CharacterAction.RotateLeft, onStarted: _onRotateLeft, onCanceled: StopRotate);
        _inputService.RemoveActionListener(CharacterAction.RotateRight, onStarted: _onRotateRight, onCanceled: StopRotate);
        _inputService.RemoveActionListener(CharacterAction.RotateForward, onStarted: _onRotateForward, onCanceled: StopRotate);
        _inputService.RemoveActionListener(CharacterAction.RotateBackward, onStarted: _onRotateBackward, onCanceled: StopRotate);
    }


    public IGrabbable GetGrab()
    {
        return _holdObject;
    }

    public void TryGrabOrInteract(IInteractable target)
    {
        if (_holdObject != null)
        {
            _holdObject.InteractWith(target);
        }
        else if (target is IGrabbable grabbable)
        {
            Grab(grabbable);
        }
        else
        {
            target.Interact();
        }
    }

    public void Grab(IGrabbable grabbable)
    {
        if (_holdObject != null)
            return;

        _holdObject = grabbable;
        var obj = _holdObject.Transform;

        obj.SetParent(_holdPoint);

        if (_holdObject.SetDefaultPos == true)
        {
            obj.localPosition = Vector3.zero;
            obj.localRotation = _holdObject.Rotate;

            _holdObject.Rigidbody.linearVelocity = Vector3.zero;
            _holdObject.Rigidbody.angularVelocity = Vector3.zero;
        }

        if (_holdObject.ToggleCollider)
            SetHeldObjectPhysics(false);

        _holdObject.OnGrab();
        _hintService.ShowHint(_actionName, _grabHints);

    }

    private void Throw()
    {
        if (_holdObject == null)
            return;

        var obj = _holdObject;
        Drop();
        obj.Rigidbody.linearVelocity = _camera.transform.forward * _throwForce;
    }

    private void Drop()
    {
        if (_holdObject == null)
            return;

        var obj = _holdObject.Transform;
        obj.SetParent(null);

        if (_holdObject.ToggleCollider)
            SetHeldObjectPhysics(true);

        _holdObject.OnDrop();
        _hintService.HideHint(_actionName);
        _holdObject = null;
    }

    private void StartRotate(Vector3 axis)
    {
        if (_holdObject == null || _rotateRoutine != null)
            return;

        _rotateRoutine = StartCoroutine(RotateRoutine(axis));
    }

    private void StopRotate()
    {
        if (_rotateRoutine != null)
        {
            StopCoroutine(_rotateRoutine);
            _rotateRoutine = null;
        }
    }

    private System.Collections.IEnumerator RotateRoutine(Vector3 axis)
    {
        while (_holdObject != null)
        {
            yield return new WaitForFixedUpdate();
            _holdObject.Transform.Rotate(axis * _rotateSpeed * Time.fixedDeltaTime);
        }
    }

    private void SetHeldObjectPhysics(bool enable)
    {
        foreach (var col in _holdObject.GetColliders())
            col.enabled = enable;

        _holdObject.Rigidbody.isKinematic = !enable;
        _holdObject.Rigidbody.interpolation = enable ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None;
        _holdObject.Rigidbody.collisionDetectionMode = enable ? CollisionDetectionMode.Discrete : CollisionDetectionMode.Continuous;
    }
}
