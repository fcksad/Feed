using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ChunkWorldHandler : MonoBehaviour
{
    private enum Direction { Up, Down }

    [SerializeField] private LevelChunkConfig _chunkConfig;
    [SerializeField] private Transform _player;
    [SerializeField] private int _loadRange = 3;

    private SortedDictionary<int, LevelChunk> _activeChunks = new();
    private Dictionary<int, LevelChunkData> _chunks = new();

    private float _chunkHeight;
    private int _currentChunkIndex;

    private float PlayerBottomY => _player.position.y - (1.65f / 2);

    private IInstantiateFactoryService _instantiateFactoryService;

    [Inject]
    public void Construct(IInstantiateFactoryService instantiateFactoryService)
    {
        _instantiateFactoryService = instantiateFactoryService;
    }
    private void Awake()
    {
        //load

        //if save null
        InitializeGeneration();
    }

    private void Update()
    {
        int playerLevel = Mathf.FloorToInt(PlayerBottomY / _chunkHeight);

        if (playerLevel != _currentChunkIndex)
        {
            _currentChunkIndex = playerLevel;
            CreateChunks(Direction.Up, _loadRange);
            CreateChunks(Direction.Down, _loadRange);
            UnloadDistantChunks();
        }
    }

    private void InitializeGeneration()
    {
        int centerIndex = Mathf.FloorToInt(PlayerBottomY / _chunkHeight);
        _currentChunkIndex = centerIndex;

        var info = GetRandomConfig();
        _chunkHeight = info.Height;

        for (int i = -_loadRange; i <= _loadRange; i++)
        {
            int index = centerIndex + i;
            SpawnOrLoadChunk(index);
        }
    }

    private void CreateChunks(Direction direction, int count)
    {
        for (int i = 1; i <= count; i++)
        {
            int index = direction == Direction.Up
                ? _currentChunkIndex + i
                : _currentChunkIndex - i;

            if (!_activeChunks.ContainsKey(index))
                SpawnOrLoadChunk(index);
        }
    }


    private void SpawnOrLoadChunk(int index)
    {
        LevelInfo info;

        if (_chunks.TryGetValue(index, out var data))
        {
            info = data.Info;
        }
        else
        {
            info = GetRandomConfig();
            _chunks[index] = new LevelChunkData
            {
                ChunkType = info.Key,
                Info = info,
                Index = index,
                Position = new Position
                {
                    Bottom = index * _chunkHeight,
                    Top = (index + 1) * _chunkHeight
                }
            };
        }

        Vector3 pos = new Vector3(0, index * _chunkHeight, 0) + info.Offset;
        var chunk = _instantiateFactoryService.Create(info.Prefab, transform, pos, Quaternion.identity, customName: index.ToString(), key: info.Key);
        chunk.Initialize(info);

        _activeChunks[index] = chunk;
    }
    private void UnloadDistantChunks()
    {
        List<int> toRemove = new();

        foreach (var kvp in _activeChunks)
        {
            int delta = kvp.Key - _currentChunkIndex;
            if (delta > _loadRange || delta < -_loadRange)
            {
                Destroy(kvp.Value.gameObject);
                toRemove.Add(kvp.Key);
            }
        }

        foreach (int index in toRemove)
            _activeChunks.Remove(index);
    }

    private LevelInfo GetRandomConfig()
    {
        return _chunkConfig.Levels[Random.Range(0, _chunkConfig.Levels.Count)];
    }
}
