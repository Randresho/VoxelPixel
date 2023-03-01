using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public Material material;
    public BlockType[] blockTypes;

    Chunk[,] chunks = new Chunk[VoxelData.worldSizeInChunks, VoxelData.worldSizeInChunks];

    private void Awake()
    {
        GenerateWorld();
    }

    void GenerateWorld()
    {
        for (int x = 0; x < VoxelData.worldSizeInChunks; x++)
        {
            for (int z = 0; z < VoxelData.worldSizeInChunks; z++)
            {
                CreateNewChunk(x, z);
            }
        }
    }

    public byte GetVoxel(Vector3 pos)
    {
        if (!IsVoxelInWorld(pos))
            return 0;
        if (pos.y < 1)
            return 1;
        else if (pos.y == VoxelData.ChunkHeight - 1)
            return 3;
        else
            return 2;
    }

    void CreateNewChunk(int x, int z)
    {
        chunks[x, z] = new Chunk(new ChunkCoord(x, z), this);
    }

    bool IsChunkInWorld(ChunkCoord coord)
    {
        if (coord.x > 0 && coord.x < VoxelData.worldSizeInChunks - 1 && coord.z < VoxelData.worldSizeInChunks - 1)
            return true;
        else
            return false;
    }

    bool IsVoxelInWorld(Vector3 pos)
    {
        if (pos.x >= 0 && pos.x < VoxelData.worldSizeInVoxels && pos.y >= 0 && pos.y < VoxelData.ChunkHeight && pos.z >= 0 && pos.z < VoxelData.worldSizeInVoxels)
            return true;
        else
            return false;
    }
}

[System.Serializable]
public class BlockType
{
    public string blockName;
    public bool isSolid;

    [Header("Texture Values")]
    public int backFaceTexture;
    public int frontFaceTexture;
    public int topFaceTexture;
    public int bottomFaceTexture;
    public int leftFaceTexture;
    public int rightFaceTexture;

    //Back, Front, Top, Bottom, Left, Right
    public int getTextureID(int faceIndex) 
    {
        switch (faceIndex)
        {
            case 0: //Back
                return backFaceTexture;
            case 1: //Front
                return frontFaceTexture;
            case 2: //Top
                return topFaceTexture;
            case 3: //Bottom
                return bottomFaceTexture;
            case 4: //Left
                return leftFaceTexture;
            case 5: //Right
                return rightFaceTexture;
            default:
                Debug.Log("Error in GetTextureID; invalid faces index");
                return 0;
        }
    }
}
