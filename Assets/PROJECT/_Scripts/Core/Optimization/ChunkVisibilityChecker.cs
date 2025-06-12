using System.Collections.Generic;
using UnityEngine;

public class ChunkVisibilityChecker : MonoBehaviour
{
    private Camera _camera;
    [SerializeField] private Plane[] _plane;
    [SerializeField] private Collider _collider;
    [SerializeField] private List<GameObject> _object;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void FixedUpdate()
    {
        var bounds = _collider.bounds;
        _plane = GeometryUtility.CalculateFrustumPlanes(_camera);
        var toggle = GeometryUtility.TestPlanesAABB(_plane, bounds);
        foreach (var obj in _object)
        {
            obj.SetActive(toggle);
        }
    }
}
