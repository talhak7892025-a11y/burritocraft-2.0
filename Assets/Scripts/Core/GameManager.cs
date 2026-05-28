using UnityEngine;
using Unity.Netcode;

namespace Burritocraft.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private bool isMultiplayer = false;
        [SerializeField] private int worldSeed = 12345;
        [SerializeField] private float chunkSize = 16f;

        public bool IsMultiplayer => isMultiplayer;
        public int WorldSeed => worldSeed;
        public float ChunkSize => chunkSize;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (isMultiplayer)
            {
                InitializeMultiplayer();
            }
            else
            {
                InitializeSinglePlayer();
            }
        }

        private void InitializeMultiplayer()
        {
            Debug.Log("[GameManager] Initializing Multiplayer mode");
            
            NetworkManager networkManager = GetComponent<NetworkManager>();
            if (networkManager != null)
            {
                // Will be configured based on host/client
            }
        }

        private void InitializeSinglePlayer()
        {
            Debug.Log("[GameManager] Initializing Single Player mode");
        }
    }
}
