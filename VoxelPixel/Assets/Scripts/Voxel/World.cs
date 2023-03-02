using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public int seed;
    public BiomeAttributes biomes;


    public Transform player;
    public Vector3 spawnPosition;

    public Material material;
    public BlockType[] blockTypes;

    Chunk[,] chunks = new Chunk[VoxelData.worldSizeInChunks, VoxelData.worldSizeInChunks];

    List<ChunkCoord> activeChunks = new List<ChunkCoord>();
    ChunkCoord playerChunkCoord;
    ChunkCoord playerLastChunkCoord;

    private void Awake()
    {
        Random.InitState(seed);

        spawnPosition = new Vector3((VoxelData.worldSizeInChunks * VoxelData.ChunkWidth) / 2f, VoxelData.ChunkHeight + 2f, (VoxelData.worldSizeInChunks * VoxelData.ChunkWidth) / 2f);
        GenerateWorld();
        playerLastChunkCoord = GetChunkFromVector3(player.position);
    }

    private void FixedUpdate()
    {
        playerChunkCoord = GetChunkFromVector3(player.position);

        if(!playerChunkCoord.Equals(playerLastChunkCoord))
            CheckViewDistance();
    }

    void GenerateWorld()
    {
        for (int x = (VoxelData.worldSizeInChunks / 2) - VoxelData.viewDistanceInChunks; x < (VoxelData.worldSizeInChunks / 2) + VoxelData.viewDistanceInChunks; x++)
        {
            for (int z = (VoxelData.worldSizeInChunks / 2) - VoxelData.viewDistanceInChunks; z < (VoxelData.worldSizeInChunks / 2) + VoxelData.viewDistanceInChunks; z++)
            {
                CreateNewChunk(x, z);
            }
        }

        player.position = spawnPosition;
    }

    ChunkCoord GetChunkFromVector3 (Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);
        return new ChunkCoord(x, z);
    }

    void CheckViewDistance()
    {
        ChunkCoord coord = GetChunkFromVector3(player.position);

        List<ChunkCoord> previouslyActiveChunks = new List<ChunkCoord>(activeChunks);

        for (int x = coord.x - VoxelData.viewDistanceInChunks; x < coord.x + VoxelData.viewDistanceInChunks; x++)
        {
            for (int z = coord.z - VoxelData.viewDistanceInChunks; z < coord.z + VoxelData.viewDistanceInChunks; z++)
            {
                if(IsChunkInWorld(new ChunkCoord(x, z)))
                {
                    if (chunks[x, z] == null)
                        CreateNewChunk(x, z);
                    else if(!chunks[x, z].IsActiveChunk) 
                    {
                        chunks[x, z].IsActiveChunk = true;
                        activeChunks.Add(new ChunkCoord(x, z));
                    }
                }

                for (int i = 0; i < previouslyActiveChunks.Count; i++)
                {
                    if (previouslyActiveChunks[i].Equals(new ChunkCoord(x, z)))
                        previouslyActiveChunks.RemoveAt(i);
                }
            }
        }

        foreach (ChunkCoord chunk in previouslyActiveChunks)
        {
            chunks[chunk.x, chunk.z].IsActiveChunk = false;
        }
    }

    public byte GetVoxel(Vector3 pos)
    {
        int yPos = Mathf.FloorToInt(pos.y);

        #region IMMUTUABLE PASS
        //If outside world, return air
        if (!IsVoxelInWorld(pos))
            return 0;

        //If bottom block of chunk, return bedrock
        if(yPos == 0)
            return 1;
        #endregion

        #region Basic Terrain Pass
        int terrainHeight = Mathf.FloorToInt(biomes.terrainHeight * Noise.Get2DPerlin(new Vector2(pos.x, pos.z), 0, biomes.terrainScale)) + biomes.solidGroundHeight;
        int voxelValue = 0;

        if (yPos == terrainHeight)
            voxelValue = 3;
        else if (yPos < terrainHeight && yPos > terrainHeight - 4)
            voxelValue = 5;
        else if (yPos > terrainHeight)
            return 0;
        else
            voxelValue = 2;
        #endregion

        #region Second Pass
        if (voxelValue == 2)
        {
            foreach (Lode lode in biomes.lodes)
            {
                if (yPos > lode.minHeight && yPos < lode.maxHeight)
                {
                    if (Noise.Get3DPerlin(pos, lode.noiseOffset, lode.scale, lode.threshold))
                        voxelValue = lode.blockID;
                }
            }
        }

        return (byte)voxelValue;
        #endregion
    }

    void CreateNewChunk(int x, int z)
    {
        chunks[x, z] = new Chunk(new ChunkCoord(x, z), this);
        activeChunks.Add(new ChunkCoord(x, z));
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
