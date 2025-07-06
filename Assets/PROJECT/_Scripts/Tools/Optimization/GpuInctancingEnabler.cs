using UnityEngine;

public class GpuInctancingEnabler : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    private void Awake()
    {
        MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        _meshRenderer.SetPropertyBlock(materialPropertyBlock);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_meshRenderer == null)
            _meshRenderer = GetComponent<MeshRenderer>();

        if (_meshRenderer == null)
        {
            Debug.LogError(gameObject.name + " Mesh in not valid");
            return;
        }


        var mats = _meshRenderer.sharedMaterials;
        bool anyChanged = false;

        for (int i = 0; i < mats.Length; i++)
        {
            var mat = mats[i];
            if (mat != null && !mat.enableInstancing)
            {
                mat.enableInstancing = true;
                anyChanged = true;
            }
        }

        if (anyChanged)
            _meshRenderer.sharedMaterials = mats;
    }
#endif
}
