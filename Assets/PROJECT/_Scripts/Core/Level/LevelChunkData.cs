using System;
using System.Collections.Generic;
using UnityEngine;

public struct ChunkCoord
{
    public int X;
    public int Y;
    public int Z;

    public override int GetHashCode() => (X, Y, Z).GetHashCode();
}

[Serializable]
public class LevelChunkData
{
    public string ChunkType;
    public Dictionary<string, Vector3> ItemPositions = new();
    public Dictionary<string, bool> OpenedDoors = new();
}
