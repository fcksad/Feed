using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GrabbableObject : InteractableObject, IGrabbable
{
    [SerializeField] protected Rigidbody _rb;
    [Header("Grab Offset")]
    [SerializeField] private Vector3 _offset = Vector3.zero;
    [SerializeField] private Quaternion _quaternion = Quaternion.identity;
    [SerializeField] private float _followSpeed = 10f;
    private Coroutine _followRoutine;


    public virtual void OnGrab(Transform pointToMove)
    {
        if (_followRoutine != null)
            StopCoroutine(_followRoutine);

        _rb.useGravity = false;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        _followRoutine = StartCoroutine(FollowTargetRoutine(pointToMove));
    }

    public virtual void OnDrop()
    {
        if (_followRoutine != null)
        {
            StopCoroutine(_followRoutine);
            _followRoutine = null;
            _rb.useGravity = true;
            _rb.interpolation = RigidbodyInterpolation.None;
        }
    }

    private IEnumerator FollowTargetRoutine(Transform target)
    {
        while (true)
        {
            Vector3 targetPos = target.position + target.rotation * _offset;
            Vector3 velocity = (targetPos - _rb.position) * _followSpeed;
            _rb.linearVelocity = velocity;

            yield return new WaitForFixedUpdate();
        }
    }
}
