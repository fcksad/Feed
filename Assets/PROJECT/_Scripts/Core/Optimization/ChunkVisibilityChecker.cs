using UnityEngine;

public class ChunkVisibilityChecker : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    [SerializeField] private Renderer _renderer;
    [SerializeField] private Plane[] _plane;
    [SerializeField] private Collider _collider;

    private void Awake()
    {
        _camera = Camera.main;
        _renderer = GetComponent<Renderer>();
        _collider = GetComponent<Collider>();
    }

    private void Update()
    {
        var bounds = _collider.bounds;
        _plane = GeometryUtility.CalculateFrustumPlanes(_camera);
        if (GeometryUtility.TestPlanesAABB(_plane, bounds))
        {
            _renderer.enabled = true;
        }
        else
        {
            _renderer.enabled = false;
        }
    }
}
