using System.Collections.Generic;
using UnityEngine;

public class LevelChunk : MonoBehaviour
{
    [field: SerializeField] public LevelChunkData Data { get; private set; }

    [field: SerializeField] public string ChunkType { get; private set; }
    [field: SerializeField] public Dictionary<string, Vector3> ItemPositions { get; private set; }

    [field: SerializeField] public List<GameObject> Doors { get; private set; }
    [field: SerializeField] public Dictionary<string, bool> OpenedDoors { get; private set; }

    public void Initialize(Vector3Int coord, LevelChunkData savedData)
    {
        if (savedData != null)
        {
            Data = savedData;
            ChunkType = savedData.ChunkType;
            // восстановить двери
            foreach (var door in Doors)
            {
                if (Data.OpenedDoors.TryGetValue(door.name, out bool open))
                    door.SetActive(!open); // к примеру
            }

            // восстановить предметы
            foreach (var item in ItemPositions)
            {
                if (Data.ItemPositions.TryGetValue(item.Key, out Vector3 pos))
                    transform.Find(item.Key).position = pos;
            }
        }
        else
        {
            Data = new LevelChunkData
            {
                ChunkType = ChunkType
            };
        }
    }

    public LevelChunkData GetSaveData()
    {
        Data.ItemPositions = ItemPositions; // можно собрать вручную
        Data.OpenedDoors = OpenedDoors;
        return Data;
    }
}
