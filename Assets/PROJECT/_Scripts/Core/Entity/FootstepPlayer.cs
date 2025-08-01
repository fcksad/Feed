using Service;
using System.Collections.Generic;
using UnityEngine;

public class FootstepPlayer
{
    private readonly IAudioService _audioService;
    private readonly ISurfaceAudioService _surfaceAudioService;
    private readonly SourceType _sourceType = SourceType.Footstep;
    private readonly LayerMask _mask;
    private readonly float _rayLength;
    private readonly Transform _rayPoint;

    private static readonly List<Material> _materialBuffer = new();

    public FootstepPlayer(IAudioService audioService, ISurfaceAudioService surfaceAudioService, LayerMask surfaceMask, Transform rayCastPos, float rayLength = 1f)
    {
        _audioService = audioService;
        _surfaceAudioService = surfaceAudioService;
        _mask = surfaceMask;
        _rayPoint = rayCastPos;
        _rayLength = rayLength;
    }

    public void TryPlayFootstep(Vector3 soundPoint)
    {
        if (!Physics.Raycast(_rayPoint.position, Vector3.down, out RaycastHit hit, _rayLength, _mask, QueryTriggerInteraction.Ignore))
        {
        }

        _materialBuffer.Clear();

        if (hit.collider != null)
        {
            var renderer = hit.collider?.GetComponent<Renderer>();
            if (renderer != null)
                _materialBuffer.AddRange(renderer.sharedMaterials);
        }

        var config = _surfaceAudioService.GetConfigByMaterial(_materialBuffer, _sourceType);
        if (config != null)
            _audioService.Play(config, position: soundPoint);
    }
}