using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class ChunkWorldHandler : MonoBehaviour
{
    [SerializeField] private List<LevelChunkConfig> _chunkConfigs;
    [SerializeField] private int _loadRadius = 2;
    [SerializeField] private Transform _player;

    private Dictionary<Vector3Int, LevelChunk> _loadedChunks = new();
    private Dictionary<Vector3Int, LevelChunkData> _savedChunks = new();

    private Vector3Int _currentChunkCoord;

/*    [Inject]
    public void Construct(Character character)
    {
        _player = character.transform;
    }
*/
    private void Update()
    {
        var coord = GetChunkCoordFromPosition(_player.position);
        if (coord != _currentChunkCoord)
        {
            _currentChunkCoord = coord;
            UpdateChunksAround(coord);
        }
    }

    private void UpdateChunksAround(Vector3Int center)
    {
        HashSet<Vector3Int> needed = new();
        for (int x = -_loadRadius; x <= _loadRadius; x++)
            for (int y = -_loadRadius; y <= _loadRadius; y++)
                for (int z = -_loadRadius; z <= _loadRadius; z++)
                    needed.Add(center + new Vector3Int(x, y, z));

        // Удаляем ненужные
        foreach (var kvp in _loadedChunks.ToList())
        {
            if (!needed.Contains(kvp.Key))
            {
                SaveAndUnload(kvp.Key);
            }
        }

        // Загружаем недостающие
        foreach (var pos in needed)
        {
            if (!_loadedChunks.ContainsKey(pos))
            {
                LoadChunkAsync(pos);
            }
        }
    }

    private void SaveAndUnload(Vector3Int coord)
    {
        var chunk = _loadedChunks[coord];
        //_savedChunks[coord] = chunk.GetComponent<FloorChunk>().GetSaveData();
        Destroy(chunk);
        _loadedChunks.Remove(coord);
    }

    private async void LoadChunkAsync(Vector3Int coord)
    {
        string type = GetChunkType(coord);
        var levelInfo = GetLevelInfoByType(type);
        if (levelInfo == null) return;

        var go = Instantiate(levelInfo.Prefab, GetWorldPosition(coord, levelInfo), Quaternion.identity);
        go.Initialize(coord, _savedChunks.TryGetValue(coord, out var saved) ? saved : null);
        _loadedChunks[coord] = go;
    }

    private LevelChunkConfig.LevelInfo GetLevelInfoByType(string type)
    {
        foreach (var config in _chunkConfigs)
        {
            foreach (var level in config.Levels)
            {
                if (level.Key == type)
                    return level;
            }
        }

        Debug.LogError($"Chunk type not found: {type}");
        return null;
    }

    private Vector3 GetWorldPosition(Vector3Int coord, LevelChunkConfig.LevelInfo info)
    {
        return new Vector3(
            coord.x * info.Size.x,
            coord.y * info.Size.y,
            coord.z * info.Size.z
        ) + info.Offset;
    }

    private Vector3Int GetChunkCoordFromPosition(Vector3 pos)
    {
        return new Vector3Int(Mathf.FloorToInt(pos.x / 50f), Mathf.FloorToInt(pos.y / 4f), Mathf.FloorToInt(pos.z / 50f));
    }
    private string GetChunkType(Vector3Int coord)
    {
        var seed = coord.x * 73856093 ^ coord.y * 19349663 ^ coord.z * 83492791;
        var rnd = new System.Random(seed);
        var allTypes = _chunkConfigs.SelectMany(c => c.Levels).Select(l => l.Key).ToList();
        return allTypes[rnd.Next(allTypes.Count)];
    }
}
