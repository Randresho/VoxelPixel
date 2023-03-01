using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelData 
{
    //Axis X
    public static readonly int ChunkWidth = 16;
    //Axis Y
    public static readonly int ChunkHeight = 7;
    //Axis z
    public static readonly int ChunkFoward = 16;
    //
    public static readonly int worldSizeInChunks = 100;

    public static int worldSizeInVoxels
    {
        get { return worldSizeInChunks * ChunkWidth; }
    }

    public static readonly int viewDistanceInChunks = 5;

    //Texture
    public static readonly int textureAtlasSizeInBlocks = 4;
    public static float NormalizedBlockTextureSize
    {
        get { return 1f / (float)textureAtlasSizeInBlocks; }
    }

    //Set the points of a cube
    public static readonly Vector3[] voxelVerts = new Vector3[8]
    {
        new Vector3(-0.5f, -0.5f, -0.5f), //1
        new Vector3(0.5f, -0.5f, -0.5f), //2
        new Vector3(0.5f, 0.5f, -0.5f), //3
        new Vector3(-0.5f, 0.5f, -0.5f), //4
        new Vector3(-0.5f, -0.5f, 0.5f), //5
        new Vector3(0.5f, -0.5f, 0.5f), //6
        new Vector3(0.5f, 0.5f, 0.5f), //7
        new Vector3(-0.5f, 0.5f, 0.5f), //8
        /*new Vector3(0f, 0f, 0f), // 1
        new Vector3(1f, 0f, 0f), // 2
        new Vector3(1f, 1f, 0f), // 3
        new Vector3(0f, 1f, 0f), // 4
        new Vector3(0f, 0f, 1f), // 5
        new Vector3(1f, 0f, 1f), // 6
        new Vector3(1f, 1f, 1f), // 7
        new Vector3(0f, 1f, 1f), // 8*/
    };

    public static readonly Vector3[] faceChecks = new Vector3[6]
    {
        new Vector3(0f, 0f, -1f),
        new Vector3(0f, 0f, 1f),
        new Vector3(0f, 1f, 0f),
        new Vector3(0f, -1f, 0f),
        new Vector3(-1f, 0f, 0f),
        new Vector3(1f, 0f, 0f),
    };

    //Set the faces of a cube
    public static readonly int[,] voxelTris = new int[6, 4]
    {
        {0,3,1,2 }, //Back Face
        //{0,3,1,1,3,2 }, //Back Face

        {5,6,4,7 }, //Front Face
        //{5,6,4,4,6,7 }, //Front Face

        {3,7,2,6 }, //Top Face
        //{3,7,2,2,7,6 }, //Top Face

        {1,5,0,4 }, //Bottom Face
        //{1,5,0,0,5,4 }, //Bottom Face

        {4,7,0,3 }, //Left Face
        //{4,7,0,0,7,3 }, //Left Face

        {1,2,5,6 }, //Right Face
        //{1,2,5,5,2,6 }, //Right Face
    };

    //Set de direccion of de Uv of a cube
    public static readonly Vector2[] voxelUvs = new Vector2[4]
    {
        new Vector2(0f, 0f),
        new Vector2(0f, 1f),
        new Vector2(1f, 0f),
        new Vector2(1f, 1f),
    };
}
