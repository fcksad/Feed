using System.Collections.Generic;
using UnityEngine;

namespace Service
{
    public interface ISurfaceAudioService
    {
        public AudioConfig GetConfigByMaterial(List<Material> materials, SourceType sourceType);
    }
}
