using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum SurfaceType { Default, Wood, }

[Serializable]
public class FootstepSurface
{
    [field: SerializeField] public SurfaceType Type { get; private set; } = SurfaceType.Default;
    [field: SerializeField] public List<Material> Materials { get; private set; }
    [field: SerializeField] public AudioConfig Audio { get; private set; }
}

[CreateAssetMenu(fileName = "FootstepConfig", menuName = "Configs/Character/Audio/Footstep")]
public class FootstepConfig : ScriptableObject
{
    [field: SerializeField] public List<FootstepSurface> Surfaces { get; private set; }

    private Dictionary<Material, AudioConfig> _materialToAudio;

    private void InitializeDictionary()
    {
        _materialToAudio = new Dictionary<Material, AudioConfig>();

        foreach (var surface in Surfaces)
        {
            foreach (var mat in surface.Materials)
            {
                if (!_materialToAudio.ContainsKey(mat))
                {
                    _materialToAudio.Add(mat, surface.Audio);
                }
            }
        }
    }

    public AudioConfig GetConfigByMaterial(List<Material> materials)
    {
        if (_materialToAudio == null)
            InitializeDictionary();

        foreach (var material in materials)
        {
            if (material != null && _materialToAudio.TryGetValue(material, out var audio))
            {
                return audio;
            }
        }

        var defaultSurface = Surfaces.FirstOrDefault(s => s.Type == SurfaceType.Default);
        return defaultSurface?.Audio;
    }

}
