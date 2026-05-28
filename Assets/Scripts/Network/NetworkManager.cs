using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

namespace Burritocraft.Network
{
    public class BurritocraftNetworkManager : MonoBehaviour
    {
        [SerializeField] private bool isServer = false;
        [SerializeField] private string serverAddress = "127.0.0.1";
        [SerializeField] private int serverPort = 7777;

        private void Start()
        {
            if (isServer)
            {
                StartServer();
            }
            else
            {
                ConnectToServer();
            }
        }

        private void StartServer()
        {
            Debug.Log("[NetworkManager] Starting as Server");
            NetworkManager.Singleton.StartServer();
        }

        private void ConnectToServer()
        {
            Debug.Log($"[NetworkManager] Connecting to server at {serverAddress}:{serverPort}");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
                serverAddress,
                (ushort)serverPort
            );
            NetworkManager.Singleton.StartClient();
        }
    }
}
