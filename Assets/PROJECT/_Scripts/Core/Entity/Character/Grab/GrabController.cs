using Service;
using System;
using UnityEngine;
using Zenject;

public class GrabController : MonoBehaviour, IInitializable, IDisposable
{
    [SerializeField] private Transform _holdPoint;
    [SerializeField] private float _throwForce = 5f;
    [SerializeField] private float _rotateSpeed = 100f;

    private IInputService _inputService;

    private IGrabbable _holdObject;
    private Coroutine _rotateRoutine;

    private Action _onRotateLeft;
    private Action _onRotateRight;
    private Action _onRotateForward;
    private Action _onRotateBackward;

    [Inject]
    public void Construct(IInputService inputService)
    {
        _inputService = inputService;
    }

    public void Initialize()
    {
        _inputService.AddActionListener(CharacterAction.Attack, onStarted: Throw);
        _inputService.AddActionListener(CharacterAction.Attack1, onStarted: Drop);
        _inputService.AddActionListener(CharacterAction.RotateLeft, onStarted: _onRotateLeft = () => StartRotate(Vector3.right), onCanceled: StopRotate);
        _inputService.AddActionListener(CharacterAction.RotateRight, onStarted: _onRotateRight = () => StartRotate(Vector3.left), onCanceled: StopRotate);
        _inputService.AddActionListener(CharacterAction.RotateForward, onStarted: _onRotateForward = () => StartRotate(Vector3.forward), onCanceled: StopRotate);
        _inputService.AddActionListener(CharacterAction.RotateBackward, onStarted: _onRotateBackward = () => StartRotate(Vector3.back), onCanceled: StopRotate);
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
            obj.localRotation = Quaternion.identity;
            _holdObject.Rigidbody.linearVelocity = Vector3.zero;
            _holdObject.Rigidbody.angularVelocity = Vector3.zero;
        }

        if (_holdObject.ToggleCollider == true)
        {
            foreach (var collider in _holdObject.GetColliders())
            {
                collider.enabled = false;
                _holdObject.Rigidbody.isKinematic = true;
                _holdObject.Rigidbody.interpolation = RigidbodyInterpolation.None;
                _holdObject.Rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }
        }
    }

    private void Throw()
    {
        if (_holdObject == null)
            return;

        var obj = _holdObject;
        Drop();
        obj.Rigidbody.linearVelocity = Camera.main.transform.forward * _throwForce;
    }

    private void Drop()
    {
        if (_holdObject == null)
            return;

        var obj = _holdObject.Transform;
        obj.SetParent(null);

        if (_holdObject.ToggleCollider == true)
        {
            foreach (var collider in _holdObject.GetColliders())
            {
                collider.enabled = true;
                _holdObject.Rigidbody.isKinematic = false;
                _holdObject.Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
                _holdObject.Rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            }
        }

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
}
