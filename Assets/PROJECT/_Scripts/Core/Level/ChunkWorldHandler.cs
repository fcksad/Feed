using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class ChunkWorldHandler : MonoBehaviour
{
    private enum Direction { Up, Down }

    [SerializeField] private LevelChunkConfig _chunkConfig;
    [SerializeField] private Transform _player;
    [SerializeField] private int _loadRange = 3;

    private Dictionary<int, (float y, LevelChunkData data)> _chunkDataMap = new();
    private Dictionary<int, (float y, LevelChunk instance)> _activeChunks = new();

    private List<LevelInfo> _chunks;
    private int _chunkListIndex = 0;

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

    private void InitializeGeneration()
    {
        int centerIndex = Mathf.FloorToInt(PlayerBottomY);
        _currentChunkIndex = centerIndex;

        Zero();

        for (int i = 1; i <= _loadRange; i++)
        {
            SpawnOrLoadChunk(centerIndex + i, Direction.Up);
            SpawnOrLoadChunk(centerIndex - i, Direction.Down);
        }
    }

    private void Update()
    {
        int playerChunk = GetPlayerChunkIndex();

        if (playerChunk != _currentChunkIndex)
        {
            _currentChunkIndex = playerChunk;
            CreateChunks(Direction.Up, _loadRange);
            CreateChunks(Direction.Down, _loadRange);
            UnloadDistantChunks();
        }
    }

    private int GetPlayerChunkIndex()
    {
        float y = PlayerBottomY;

        foreach (var pair in _chunkDataMap)
        {
            float chunkY = pair.Value.y;
            float chunkHeight = pair.Value.data.Info.Height;

            if (y >= chunkY && y < chunkY + chunkHeight)
            {
                return pair.Key;
            }
        }

        return _currentChunkIndex; // fallback: не найден — останется тот же
    }


    private void CreateChunks(Direction direction, int count)
    {
        for (int i = 1; i <= count; i++)
        {
            int index = direction == Direction.Up
                ? _currentChunkIndex + i
                : _currentChunkIndex - i;

            if (!_activeChunks.ContainsKey(index))
                SpawnOrLoadChunk(index, direction);
        }
    }


    private void Zero()
    {
        LevelInfo info;

        if (_chunkDataMap.TryGetValue(0, out var existingData))
        {
            info = existingData.data.Info;
        }
        else
        {
            info = _chunkConfig.Levels.FirstOrDefault();
        }

        Vector3 pos = new Vector3(0, 0, 0) + info.Offset;
        var chunk = _instantiateFactoryService.Create(info.Prefab, transform, pos, Quaternion.identity, customName: $"{info.Type} - [0]", key: info.Type);
        chunk.Initialize(info);
        if (!_chunkDataMap.ContainsKey(0))
        {
            _chunkDataMap[0] = (0, new LevelChunkData
            {
                ChunkType = info.Type,
                Info = info,
                Index = 0,
            });
        }

        _activeChunks[0] = (0, chunk);
    }

    private void SpawnOrLoadChunk(int index, Direction direction)
    {
        LevelInfo info;

        if (_chunkDataMap.TryGetValue(index, out var existingData))
        {
            info = existingData.data.Info;
        }
        else
        {
            info = GetConfig();
        }

        float prevY = GetPreviousYPosition(index, direction);
        float newY = direction == Direction.Up
            ? prevY
            : prevY - info.Height;

        Vector3 pos = new Vector3(0, newY, 0) + info.Offset;

        var chunk = _instantiateFactoryService.Create(info.Prefab, transform, pos, Quaternion.identity, customName: $"{info.Type} - [{index}]", key: info.Type);
        chunk.Initialize(info);

        if (!_chunkDataMap.ContainsKey(index))
        {
            _chunkDataMap[index] = (newY, new LevelChunkData
            {
                ChunkType = info.Type,
                Info = info,
                Index = index,
            });
        }

        _activeChunks[index] = (newY, chunk);
    }

    private float GetPreviousYPosition(int index, Direction direction)
    {
        if (index == 0)
            return 0f;

        int previousIndex = direction == Direction.Up ? index - 1 : index + 1;

        if (_chunkDataMap.TryGetValue(previousIndex, out var previousData))
        {
            return previousData.y += direction == Direction.Up ? previousData.data.Height : 0;
        }

        return 0;
    }


    private void UnloadDistantChunks()
    {
        List<int> toRemove = new();

        foreach (var chunk in _activeChunks)
        {
            int delta = chunk.Key - _currentChunkIndex;
            if (delta > _loadRange || delta < -_loadRange)
            {
                _instantiateFactoryService.Release(chunk.Value.instance, chunk.Value.instance.ChunkType);
                toRemove.Add(chunk.Key);
            }
        }

        foreach (int index in toRemove)
            _activeChunks.Remove(index);
    }

    private LevelInfo GetConfig()
    {
        if (_chunks == null || _chunks.Count == 0)
        {
            var weights = new Dictionary<LevelInfo, int>();

            foreach (var level in _chunkConfig.Levels)
            {
                if (level != null && level.Prefab != null && level.Weight > 0)
                {
                    weights[level] = level.Weight;
                }
            }
            int totalCount = weights.Values.Sum();
            _chunks = WeightedDropSystem<LevelInfo>.Get(weights, totalCount);
        }

        if (_chunkListIndex >= _chunks.Count)
        {
            _chunks = WeightedDropSystem<LevelInfo>.Shuffle(_chunks);
            _chunkListIndex = 0;
        }

        var LevelInfo = _chunks[_chunkListIndex];
        _chunkListIndex++;

        return LevelInfo;
    }

}
