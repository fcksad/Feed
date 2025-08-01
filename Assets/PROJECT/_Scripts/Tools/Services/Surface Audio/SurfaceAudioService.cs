using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Service
{
    public class SurfaceAudioService : ISurfaceAudioService, IInitializable, IDisposable
    {
        private List<SurfaceAudioConfig> _configs = new List<SurfaceAudioConfig>();
        private Dictionary<SourceType, Dictionary<Material, AudioConfig>> _surfaceMaterials = new();
        private Dictionary<SourceType, AudioConfig> _defaultConfigs = new();

        public void Initialize()
        {
            _configs = ResourceLoader.GetAll<SurfaceAudioConfig>();
            Setup();
        }

        public void Dispose()
        {
            _configs = null;
            _surfaceMaterials = null;
        }

        private void Setup()
        {
            foreach (var config in _configs)
            {
                if (!_surfaceMaterials.ContainsKey(config.SourceType))
                    _surfaceMaterials[config.SourceType] = new Dictionary<Material, AudioConfig>();

                var materialMap = _surfaceMaterials[config.SourceType];

                foreach (var surface in config.Surfaces)
                {
                    foreach (var material in surface.Materials)
                    {
                        if (material == null) continue;

                        if (!materialMap.ContainsKey(material))
                            materialMap.Add(material, surface.Audio);
                    }

                    if (surface.Type == SurfaceType.Default && !_defaultConfigs.ContainsKey(config.SourceType))
                    {
                        _defaultConfigs[config.SourceType] = surface.Audio;
                    }
                }
            }
        }

        public AudioConfig GetConfigByMaterial(List<Material> materials, SourceType sourceType)
        {
            if (_surfaceMaterials == null)
                Setup();

            if (_surfaceMaterials.TryGetValue(sourceType, out var materialMap))
            {
                foreach (var material in materials)
                {
                    if (material != null && materialMap.TryGetValue(material, out var audio))
                    {
                        return audio;
                    }
                }
            }

            if (_defaultConfigs.TryGetValue(sourceType, out var defaultAudio))
            {
                return defaultAudio;
            }

            Debug.LogWarning($"[SurfaceAudioService] No AudioConfig found for SourceType {sourceType} (materials: [{string.Join(", ", materials.Where(m => m != null).Select(m => m.name))}])");
            return null;
        }
    }

}
