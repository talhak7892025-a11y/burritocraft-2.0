using UnityEngine;
using System.Collections.Generic;

namespace Burritocraft.Voxel
{
    public class Chunk : MonoBehaviour
    {
        public static int SIZE = 16; // 16x16x16 voxel chunk
        
        private byte[] voxels;
        private Vector3Int position;
        private Mesh mesh;
        private MeshCollider meshCollider;
        private bool isDirty = true;
        private bool isGenerated = false;

        public Vector3Int Position => position;
        public bool IsGenerated => isGenerated;
        public bool IsDirty => isDirty;

        private void Awake()
        {
            voxels = new byte[SIZE * SIZE * SIZE];
            mesh = new Mesh();
            mesh.name = "Chunk Mesh";
            
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            if (meshFilter == null)
                meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer == null)
                meshRenderer = gameObject.AddComponent<MeshRenderer>();

            meshCollider = GetComponent<MeshCollider>();
            if (meshCollider == null)
                meshCollider = gameObject.AddComponent<MeshCollider>();
        }

        public void Initialize(Vector3Int chunkPos)
        {
            position = chunkPos;
            gameObject.name = $"Chunk_{chunkPos.x}_{chunkPos.y}_{chunkPos.z}";
            transform.position = new Vector3(chunkPos.x * SIZE, chunkPos.y * SIZE, chunkPos.z * SIZE);
        }

        public void SetVoxel(int x, int y, int z, byte voxelId)
        {
            if (IsInBounds(x, y, z))
            {
                int index = x + y * SIZE + z * SIZE * SIZE;
                if (voxels[index] != voxelId)
                {
                    voxels[index] = voxelId;
                    isDirty = true;
                }
            }
        }

        public byte GetVoxel(int x, int y, int z)
        {
            if (IsInBounds(x, y, z))
            {
                int index = x + y * SIZE + z * SIZE * SIZE;
                return voxels[index];
            }
            return 0; // Air
        }

        public void GenerateMesh()
        {
            if (!isDirty && isGenerated)
                return;

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Color> colors = new List<Color>();

            for (int x = 0; x < SIZE; x++)
            {
                for (int y = 0; y < SIZE; y++)
                {
                    for (int z = 0; z < SIZE; z++)
                    {
                        byte voxelId = GetVoxel(x, y, z);
                        if (voxelId != 0) // Not air
                        {
                            AddVoxelMesh(x, y, z, voxelId, vertices, triangles, colors);
                        }
                    }
                }
            }

            mesh.Clear();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.colors = colors.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            meshCollider.convex = false;
            meshCollider.mesh = null;
            meshCollider.mesh = mesh;

            isDirty = false;
            isGenerated = true;
        }

        private void AddVoxelMesh(int x, int y, int z, byte voxelId, List<Vector3> vertices, List<int> triangles, List<Color> colors)
        {
            Color voxelColor = GetVoxelColor(voxelId);

            // Check each face and add if exposed
            if (!HasVoxelAt(x, y + 1, z)) AddTopFace(x, y, z, vertices, triangles, colors, voxelColor);
            if (!HasVoxelAt(x, y - 1, z)) AddBottomFace(x, y, z, vertices, triangles, colors, voxelColor);
            if (!HasVoxelAt(x + 1, y, z)) AddRightFace(x, y, z, vertices, triangles, colors, voxelColor);
            if (!HasVoxelAt(x - 1, y, z)) AddLeftFace(x, y, z, vertices, triangles, colors, voxelColor);
            if (!HasVoxelAt(x, y, z + 1)) AddFrontFace(x, y, z, vertices, triangles, colors, voxelColor);
            if (!HasVoxelAt(x, y, z - 1)) AddBackFace(x, y, z, vertices, triangles, colors, voxelColor);
        }

        private bool HasVoxelAt(int x, int y, int z)
        {
            return IsInBounds(x, y, z) && GetVoxel(x, y, z) != 0;
        }

        private bool IsInBounds(int x, int y, int z)
        {
            return x >= 0 && x < SIZE && y >= 0 && y < SIZE && z >= 0 && z < SIZE;
        }

        private Color GetVoxelColor(byte voxelId)
        {
            return voxelId switch
            {
                1 => new Color(0.5f, 0.5f, 0.5f), // Stone - Gray
                2 => new Color(0.4f, 0.3f, 0.2f), // Dirt - Brown
                3 => new Color(0.2f, 0.8f, 0.2f), // Grass - Green
                4 => new Color(0.9f, 0.8f, 0.6f), // Sand - Tan
                5 => new Color(0.2f, 0.5f, 0.8f), // Water - Blue
                6 => new Color(1f, 0.5f, 0f), // Lava - Orange
                7 => new Color(0.6f, 0.4f, 0.2f), // Wood - Dark Brown
                8 => new Color(0.3f, 0.6f, 0.2f), // Leaves - Dark Green
                9 => new Color(0.1f, 0.1f, 0.1f), // Bedrock - Dark Gray
                _ => Color.white
            };
        }

        // Face addition methods (simplified cube faces)
        private void AddTopFace(int x, int y, int z, List<Vector3> vertices, List<int> triangles, List<Color> colors, Color color)
        {
            int startIndex = vertices.Count;
            vertices.Add(new Vector3(x, y + 1, z));
            vertices.Add(new Vector3(x + 1, y + 1, z));
            vertices.Add(new Vector3(x + 1, y + 1, z + 1));
            vertices.Add(new Vector3(x, y + 1, z + 1));

            for (int i = 0; i < 4; i++) colors.Add(color);

            triangles.Add(startIndex);
            triangles.Add(startIndex + 1);
            triangles.Add(startIndex + 2);
            triangles.Add(startIndex);
            triangles.Add(startIndex + 2);
            triangles.Add(startIndex + 3);
        }

        private void AddBottomFace(int x, int y, int z, List<Vector3> vertices, List<int> triangles, List<Color> colors, Color color)
        {
            int startIndex = vertices.Count;
            vertices.Add(new Vector3(x, y, z));
            vertices.Add(new Vector3(x, y, z + 1));
            vertices.Add(new Vector3(x + 1, y, z + 1));
            vertices.Add(new Vector3(x + 1, y, z));

            for (int i = 0; i < 4; i++) colors.Add(color);

            triangles.Add(startIndex);
            triangles.Add(startIndex + 1);
            triangles.Add(startIndex + 2);
            triangles.Add(startIndex);
            triangles.Add(startIndex + 2);
            triangles.Add(startIndex + 3);
        }

        private void AddRightFace(int x, int y, int z, List<Vector3> vertices, List<int> triangles, List<Color> colors, Color color)
        {
            int startIndex = vertices.Count;
            vertices.Add(new Vector3(x + 1, y, z));
            vertices.Add(new Vector3(x + 1, y + 1, z));
            vertices.Add(new Vector3(x + 1, y + 1, z + 1));
            vertices.Add(new Vector3(x + 1, y, z + 1));

            for (int i = 0; i < 4; i++) colors.Add(color);

            triangles.Add(startIndex);
            triangles.Add(startIndex + 1);
            triangles.Add(startIndex + 2);
            triangles.Add(startIndex);
            triangles.Add(startIndex + 2);
            triangles.Add(startIndex + 3);
        }

        private void AddLeftFace(int x, int y, int z, List<Vector3> vertices, List<int> triangles, List<Color> colors, Color color)
        {
            int startIndex = vertices.Count;
            vertices.Add(new Vector3(x, y, z));
            vertices.Add(new Vector3(x, y, z + 1));
            vertices.Add(new Vector3(x, y + 1, z + 1));
            vertices.Add(new Vector3(x, y + 1, z));

            for (int i = 0; i < 4; i++) colors.Add(color);

            triangles.Add(startIndex);
            triangles.Add(startIndex + 1);
            triangles.Add(startIndex + 2);
            triangles.Add(startIndex);
            triangles.Add(startIndex + 2);
            triangles.Add(startIndex + 3);
        }

        private void AddFrontFace(int x, int y, int z, List<Vector3> vertices, List<int> triangles, List<Color> colors, Color color)
        {
            int startIndex = vertices.Count;
            vertices.Add(new Vector3(x, y, z + 1));
            vertices.Add(new Vector3(x + 1, y, z + 1));
            vertices.Add(new Vector3(x + 1, y + 1, z + 1));
            vertices.Add(new Vector3(x, y + 1, z + 1));

            for (int i = 0; i < 4; i++) colors.Add(color);

            triangles.Add(startIndex);
            triangles.Add(startIndex + 1);
            triangles.Add(startIndex + 2);
            triangles.Add(startIndex);
            triangles.Add(startIndex + 2);
            triangles.Add(startIndex + 3);
        }

        private void AddBackFace(int x, int y, int z, List<Vector3> vertices, List<int> triangles, List<Color> colors, Color color)
        {
            int startIndex = vertices.Count;
            vertices.Add(new Vector3(x, y, z));
            vertices.Add(new Vector3(x, y + 1, z));
            vertices.Add(new Vector3(x + 1, y + 1, z));
            vertices.Add(new Vector3(x + 1, y, z));

            for (int i = 0; i < 4; i++) colors.Add(color);

            triangles.Add(startIndex);
            triangles.Add(startIndex + 1);
            triangles.Add(startIndex + 2);
            triangles.Add(startIndex);
            triangles.Add(startIndex + 2);
            triangles.Add(startIndex + 3);
        }
    }
}
