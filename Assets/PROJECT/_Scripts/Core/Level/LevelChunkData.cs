using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelChunkData
{
    public string ChunkType;
    public Dictionary<string, Vector3> ItemPositions = new();
    public Dictionary<string, bool> OpenedDoors = new();
    public LevelInfo Info;
    public int Index;

    public float Height => Info.Height;
}
