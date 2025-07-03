using System.Collections.Generic;
using UnityEngine;

public class FootstepPlayer
{
    private readonly IAudioService _audioService;
    private readonly FootstepConfig _footstepConfig;
    private readonly LayerMask _mask;
    private readonly float _rayLength;
    private readonly Transform _rayPoint;

    private static readonly List<Material> _materialBuffer = new();

    public FootstepPlayer(IAudioService audioService, FootstepConfig config, LayerMask surfaceMask, Transform rayCastPos ,float rayLength = 1f)
    {
        _audioService = audioService;
        _footstepConfig = config;
        _mask = surfaceMask;
        _rayPoint = rayCastPos;
        _rayLength = rayLength;
    }

    public void TryPlayFootstep(Vector3 soundPoint)
    {
        if (!Physics.Raycast(_rayPoint.position, Vector3.down, out RaycastHit hit, _rayLength, _mask, QueryTriggerInteraction.Ignore))
            return;

        _materialBuffer.Clear();

        var renderer = hit.collider.GetComponent<Renderer>();
        if (renderer != null)
            _materialBuffer.AddRange(renderer.sharedMaterials);

        var config = _footstepConfig.GetConfigByMaterial(_materialBuffer);
        if (config != null)
            _audioService.Play(config, position: soundPoint);
    }
}
