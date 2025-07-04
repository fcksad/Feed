using UnityEngine;

public class GrabController : MonoBehaviour
{
    [SerializeField] private Transform _holdPoint;

    private IGrabbable _holdObject;

    public void TryGrab(IGrabbable grabbable)
    {
        if (_holdObject != null)
            return;

        _holdObject = grabbable;
        _holdObject.OnGrab(_holdPoint);
    }
    public void ReleaseGrab()
    {
        if (_holdObject == null) return;
        _holdObject.OnDrop();
        _holdObject = null;
    }

    public IGrabbable GetGrab() => _holdObject;
}
