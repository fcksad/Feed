using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class ChunkWorldHandler : MonoBehaviour
{
    [SerializeField] private LevelChunkConfig _chunkConfig;
    [SerializeField] private Transform _player;
    [SerializeField] private int _loadRange = 2;

    private SortedDictionary<float, LevelChunk> _loadedChunks = new();
    private float _minY = 0f;
    private float _maxY = 0f;
    private float _threshold;


    /* private void Start()
     {
         var initialChunk = GetRandomConfig();
         var chunk = Instantiate(initialChunk.Prefab, Vector3.zero + initialChunk.Offset, Quaternion.identity);
         chunk.Initialize(initialChunk);
         _loadedChunks[0f] = chunk;

         _minY = 0f;
         _maxY = initialChunk.Height;

         _threshold = initialChunk.Height;
     }*/

    private void Start()
    {
        // 🟢 Центральный чанк на позиции 0
        var initialInfo = GetRandomConfig();
        var chunk = Instantiate(initialInfo.Prefab, Vector3.zero + initialInfo.Offset, Quaternion.identity, transform);
        chunk.Initialize(initialInfo);

        _loadedChunks[0f] = chunk;
        _minY = 0f;
        _maxY = initialInfo.Height;
        _threshold = initialInfo.Height;

        // 🔽 Генерация вниз
        float currentY = 0f;
        for (int i = 0; i < _loadRange; i++)
        {
            var info = GetRandomConfig();
            currentY -= info.Height;
            var pos = new Vector3(0, currentY, 0) + info.Offset;
            var ch = Instantiate(info.Prefab, pos, Quaternion.identity, transform);
            ch.Initialize(info);
            _loadedChunks[currentY] = ch;
            _minY = currentY;
        }

        // 🔼 Генерация вверх
        currentY = initialInfo.Height;
        for (int i = 0; i < _loadRange; i++)
        {
            var info = GetRandomConfig();
            var pos = new Vector3(0, currentY, 0) + info.Offset;
            var ch = Instantiate(info.Prefab, pos, Quaternion.identity, transform);
            ch.Initialize(info);
            _loadedChunks[currentY] = ch;
            currentY += info.Height;
            _maxY = currentY;
        }
    }

    private void Update()
    {
        float playerY = _player.position.y;

        if (playerY > _maxY - _threshold)
            CreateChunkUp();

        if (playerY < _minY + _threshold)
            CreateChunkDown();
    }

    private void CreateChunkUp()
    {
        var info = GetRandomConfig();
        var position = new Vector3(0, _maxY, 0) + info.Offset;

        var chunk = Instantiate(info.Prefab, position, Quaternion.identity, transform);
        chunk.Initialize(info);
        _loadedChunks[_maxY] = chunk;

        _minY = GetBottomMostY(); 
        _maxY += info.Height;

        TryUnloadLowest();
    }

    private void CreateChunkDown()
    {
        var info = GetRandomConfig();
        float newMinY = _minY - info.Height;
        var position = new Vector3(0, newMinY, 0) + info.Offset;

        var chunk = Instantiate(info.Prefab, position, Quaternion.identity, transform);
        chunk.Initialize(info);
        _loadedChunks[newMinY] = chunk;

        _minY = newMinY;

        TryUnloadHighest();
    }


    private void TryUnloadLowest()
    {
        if (_loadedChunks.Count < 3) return;

        var bottomKey = GetBottomMostY();
        if (_player.position.y - bottomKey > _threshold * 2)
        {
            Destroy(_loadedChunks[bottomKey].gameObject);
            _loadedChunks.Remove(bottomKey);
            _minY = GetBottomMostY();
        }
    }

    private void TryUnloadHighest()
    {
        if (_loadedChunks.Count < 3) return;

        var topKey = GetTopMostY();
        if (topKey - _player.position.y > _threshold * 2)
        {
            Destroy(_loadedChunks[topKey].gameObject);
            _loadedChunks.Remove(topKey);
            _maxY = GetTopMostY() + _loadedChunks[_maxY - _chunkConfig.Levels[0].Height].Data.Height;
        }
    }

    private float GetBottomMostY()
    {
        foreach (var key in _loadedChunks.Keys)
            return key; 
        return 0f;
    }

    private float GetTopMostY()
    {
        float lastKey = 0f;
        foreach (var key in _loadedChunks.Keys)
            lastKey = key;
        return lastKey;
    }

    private LevelInfo GetRandomConfig()
    {
        int index = Random.Range(0, _chunkConfig.Levels.Count);
        return _chunkConfig.Levels[index];
    }
}
