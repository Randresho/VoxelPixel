using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public ChunkCoord coord;

    //GameObjects
    private GameObject chunkObject;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    [Space]
    private int triangleIdx;
    private int vertexIdx;
    private List<Vector3> vertices;
    private List<Vector2> uvs;
    private List<int> triangles;
    private Mesh mesh;
    private byte[,,] voxelMap = new byte[VoxelData.ChunkWidth,VoxelData.ChunkHeight,VoxelData.ChunkFoward];

    private World world;

    public Chunk(ChunkCoord _coord, World _world)
    {
        coord = _coord;
        world = _world;

        //Add gameobjects to the object
        chunkObject = new GameObject("Chunk " + coord.x + ", " + coord.z);
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();

        meshRenderer.material = world.material;
        chunkObject.transform.SetParent(world.transform);
        chunkObject.transform.position = new Vector3(coord.x * VoxelData.ChunkWidth, 0f, coord.z * VoxelData.ChunkWidth);

        PopulateVoxelMap();
        CreateMeshData();
        CreateMesh();
    }

    private void PopulateVoxelMap()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkFoward; z++)
                {
                    voxelMap[x,y,z] = world.GetVoxel(new Vector3(x, y, z) + position);
                }
            }
        }
    }

    private void CreateMeshData()
    {
        vertexIdx = 0;
        vertices = new List<Vector3>();
        triangles = new List<int>();
        uvs = new List<Vector2>();

        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkFoward; z++)
                {
                    if (world.blockTypes[voxelMap[x, y, z]].isSolid)
                        AddVoxelDataToChunk(new Vector3(x, y, z));
                }
            }
        }
    }

    public bool IsActiveChunk
    {
        get { return chunkObject.activeSelf; }
        set { chunkObject.SetActive(value); }
    }

    public Vector3 position
    {
        get { return chunkObject.transform.position; } 
    }

    bool IsVoxelInChunk(int x, int y, int z)
    {
        if (x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 || y > VoxelData.ChunkHeight - 1 || z < 0 || z > VoxelData.ChunkFoward - 1)
            return false;
        else
            return true;
    }

    bool CheckVoxel(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (!IsVoxelInChunk(x,y,z))
            return world.blockTypes[world.GetVoxel(pos + position)].isSolid;

        return world.blockTypes[voxelMap[x, y, z]].isSolid;
    }

    private void AddVoxelDataToChunk(Vector3 pos)
    {
        for (int p = 0; p < 6; p++)
        {
            if (!CheckVoxel(pos + VoxelData.faceChecks[p]))
            {
                byte blockID = voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];

                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]]);
                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]]);
                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]]);
                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]]);


                AddTexture(world.blockTypes[blockID].getTextureID(p));

                triangles.Add(vertexIdx);
                triangles.Add(vertexIdx + 1);
                triangles.Add(vertexIdx + 2);
                triangles.Add(vertexIdx + 2);
                triangles.Add(vertexIdx + 1);
                triangles.Add(vertexIdx + 3);

                vertexIdx += 4;
            }
        }
    }

    private void CreateMesh()
    {
        mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    private void AddTexture(int textureID)
    {
        float y = textureID / VoxelData.textureAtlasSizeInBlocks;
        float x = textureID - (y * VoxelData.textureAtlasSizeInBlocks);

        x *= VoxelData.NormalizedBlockTextureSize;
        y *= VoxelData.NormalizedBlockTextureSize;

        //Only if you want to start on de top
        y = 1f - y - VoxelData.NormalizedBlockTextureSize;

        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + VoxelData.NormalizedBlockTextureSize));
        uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y));
        uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y + VoxelData.NormalizedBlockTextureSize));
    }
}

public class ChunkCoord
{
    public int x;
    public int z;

    public ChunkCoord (int _x, int _z)
    {
        x = _x;
        z = _z;
    }

    public bool Equals(ChunkCoord other)
    {
        if (other == null)
            return false;
        else if (other.x == x && other.z == z) 
            return true;
        else
            return false;
    }
}
