using UnityEngine;
using System.Collections.Generic;
using Burritocraft.Core;

namespace Burritocraft.Voxel
{
    public class WorldGenerator : MonoBehaviour
    {
        [SerializeField] private int renderDistance = 8;
        [SerializeField] private Material chunkMaterial;
        
        private Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();
        private Perlin perlinGenerator;
        private Transform playerTransform;
        private Vector3Int lastPlayerChunkPos = Vector3Int.zero;

        private void Start()
        {
            if (chunkMaterial == null)
            {
                // Create a default material if none is assigned
                chunkMaterial = new Material(Shader.Find("Standard"));
            }

            perlinGenerator = new Perlin(GameManager.Instance.WorldSeed);
        }

        private void Update()
        {
            if (playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                    playerTransform = player.transform;
                return;
            }

            UpdateChunksAroundPlayer();
        }

        private void UpdateChunksAroundPlayer()
        {
            Vector3Int playerChunkPos = GetChunkPosition(playerTransform.position);

            if (playerChunkPos == lastPlayerChunkPos)
                return;

            lastPlayerChunkPos = playerChunkPos;

            // Load chunks around player
            for (int x = -renderDistance; x <= renderDistance; x++)
            {
                for (int z = -renderDistance; z <= renderDistance; z++)
                {
                    Vector3Int chunkPos = playerChunkPos + new Vector3Int(x, 0, z);
                    
                    if (!chunks.ContainsKey(chunkPos))
                    {
                        LoadChunk(chunkPos);
                    }
                }
            }

            // Unload distant chunks
            List<Vector3Int> chunksToUnload = new List<Vector3Int>();
            foreach (var chunkPos in chunks.Keys)
            {
                float distance = Vector3.Distance(chunkPos * Chunk.SIZE, playerTransform.position);
                if (distance > renderDistance * Chunk.SIZE * 2)
                {
                    chunksToUnload.Add(chunkPos);
                }
            }

            foreach (var chunkPos in chunksToUnload)
            {
                UnloadChunk(chunkPos);
            }
        }

        private void LoadChunk(Vector3Int chunkPos)
        {
            GameObject chunkObj = new GameObject();
            chunkObj.transform.parent = transform;
            
            Chunk chunk = chunkObj.AddComponent<Chunk>();
            chunk.Initialize(chunkPos);

            // Generate terrain
            GenerateChunkTerrain(chunk, chunkPos);

            // Render mesh
            chunk.GenerateMesh();

            MeshRenderer renderer = chunkObj.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = chunkMaterial;
            }

            chunks[chunkPos] = chunk;
        }

        private void GenerateChunkTerrain(Chunk chunk, Vector3Int chunkPos)
        {
            int chunkWorldX = chunkPos.x * Chunk.SIZE;
            int chunkWorldY = chunkPos.y * Chunk.SIZE;
            int chunkWorldZ = chunkPos.z * Chunk.SIZE;

            for (int x = 0; x < Chunk.SIZE; x++)
            {
                for (int z = 0; z < Chunk.SIZE; z++)
                {
                    float worldX = chunkWorldX + x;
                    float worldZ = chunkWorldZ + z;

                    // Simple noise-based terrain generation
                    float noiseValue = Mathf.PerlinNoise(worldX * 0.05f, worldZ * 0.05f);
                    int height = Mathf.FloorToInt(noiseValue * 16) + 8; // Height between 8 and 24

                    for (int y = 0; y < Chunk.SIZE; y++)
                    {
                        int worldY = chunkWorldY + y;

                        byte voxelId = 0;
                        if (worldY < height - 3)
                        {
                            voxelId = 1; // Stone
                        }
                        else if (worldY < height - 1)
                        {
                            voxelId = 2; // Dirt
                        }
                        else if (worldY == height - 1)
                        {
                            voxelId = 3; // Grass
                        }
                        else if (worldY < 0)
                        {
                            voxelId = 9; // Bedrock at bottom
                        }

                        chunk.SetVoxel(x, y, z, voxelId);
                    }
                }
            }
        }

        private void UnloadChunk(Vector3Int chunkPos)
        {
            if (chunks.TryGetValue(chunkPos, out Chunk chunk))
            {
                Destroy(chunk.gameObject);
                chunks.Remove(chunkPos);
            }
        }

        private Vector3Int GetChunkPosition(Vector3 worldPos)
        {
            return new Vector3Int(
                Mathf.FloorToInt(worldPos.x / Chunk.SIZE),
                0,
                Mathf.FloorToInt(worldPos.z / Chunk.SIZE)
            );
        }

        public Chunk GetChunkAtWorldPosition(Vector3 worldPos)
        {
            Vector3Int chunkPos = GetChunkPosition(worldPos);
            if (chunks.TryGetValue(chunkPos, out Chunk chunk))
            {
                return chunk;
            }
            return null;
        }
    }

    public class Perlin
    {
        private int seed;

        public Perlin(int seed)
        {
            this.seed = seed;
            Random.InitState(seed);
        }

        public float GetValue(float x, float y)
        {
            return Mathf.PerlinNoise(x, y);
        }
    }
}
