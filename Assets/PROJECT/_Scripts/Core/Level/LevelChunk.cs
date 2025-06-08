using System.Collections.Generic;
using UnityEngine;

public class LevelChunk : MonoBehaviour
{
    [field: SerializeField] public LevelChunkData Data { get; private set; }

    [field: SerializeField] public string ChunkType { get; private set; }
    [field: SerializeField] public Dictionary<string, Vector3> ItemPositions { get; private set; }

    [field: SerializeField] public List<GameObject> Doors { get; private set; }
    [field: SerializeField] public Dictionary<string, bool> OpenedDoors { get; private set; }

    public void Initialize(LevelInfo info)
    {
        Data.Info = info;
    }

    public LevelChunkData GetSaveData()
    {
        Data.ItemPositions = ItemPositions; // можно собрать вручную
        Data.OpenedDoors = OpenedDoors;
        return Data;
    }
}
