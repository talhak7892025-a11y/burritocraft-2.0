using UnityEngine;
using Unity.Netcode;
using Burritocraft.Voxel;

namespace Burritocraft.Player
{
    public class PlayerController : NetworkBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float sprintSpeed = 10f;
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float mouseSensitivity = 2f;
        [SerializeField] private float groundDrag = 5f;
        [SerializeField] private float airDrag = 2f;
        [SerializeField] private float groundDist = 0.2f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float blockReachDistance = 5f;
        [SerializeField] private float blockBreakSpeed = 0.5f;

        private Rigidbody rb;
        private Camera playerCamera;
        private Transform cameraTransform;
        private float xRotation = 0f;
        private bool isGrounded;
        private bool isBreakingBlock = false;
        private float blockBreakProgress = 0f;
        private WorldGenerator worldGenerator;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            playerCamera = GetComponentInChildren<Camera>();
            cameraTransform = playerCamera.transform;
            gameObject.tag = "Player";
        }

        private void Start()
        {
            if (!IsOwner)
            {
                playerCamera.enabled = false;
                GetComponent<AudioListener>().enabled = false;
            }

            worldGenerator = FindObjectOfType<WorldGenerator>();
        }

        private void Update()
        {
            if (!IsOwner)
                return;

            // Ground check
            isGrounded = Physics.CheckSphere(transform.position + Vector3.down * groundDist, 0.3f, groundLayer);

            // Input handling
            HandleMovement();
            HandleCamera();
            HandleBlockInteraction();
        }

        private void HandleMovement()
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            bool isSprinting = Input.GetKey(KeyCode.LeftShift);
            bool isJumping = Input.GetKeyDown(KeyCode.Space);

            float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
            Vector3 moveDirection = transform.forward * z + transform.right * x;
            moveDirection.Normalize();

            rb.velocity = new Vector3(moveDirection.x * currentSpeed, rb.velocity.y, moveDirection.z * currentSpeed);

            if (isJumping && isGrounded)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }

            // Apply drag
            rb.drag = isGrounded ? groundDrag : airDrag;
        }

        private void HandleCamera()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0, 0);
            transform.Rotate(Vector3.up * mouseX);
        }

        private void HandleBlockInteraction()
        {
            // Raycast for block targeting
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, blockReachDistance))
            {
                Chunk chunk = hit.collider.GetComponent<Chunk>();
                if (chunk != null)
                {
                    if (Input.GetMouseButtonDown(0)) // Break block
                    {
                        BreakBlock(hit, chunk);
                    }
                    else if (Input.GetMouseButtonDown(1)) // Place block
                    {
                        PlaceBlock(hit, chunk);
                    }
                }
            }
            else
            {
                isBreakingBlock = false;
                blockBreakProgress = 0f;
            }
        }

        private void BreakBlock(RaycastHit hit, Chunk chunk)
        {
            Vector3 hitPoint = hit.point - hit.normal * 0.1f;
            Vector3 localHitPoint = chunk.transform.InverseTransformPoint(hitPoint);

            int x = Mathf.FloorToInt(localHitPoint.x);
            int y = Mathf.FloorToInt(localHitPoint.y);
            int z = Mathf.FloorToInt(localHitPoint.z);

            if (chunk.GetVoxel(x, y, z) != 0)
            {
                chunk.SetVoxel(x, y, z, 0); // Set to air
                chunk.GenerateMesh();

                if (IsOwner)
                {
                    BreakBlockServerRpc(chunk.Position, x, y, z);
                }
            }
        }

        private void PlaceBlock(RaycastHit hit, Chunk chunk)
        {
            Vector3 hitPoint = hit.point + hit.normal * 0.1f;
            Vector3 localHitPoint = chunk.transform.InverseTransformPoint(hitPoint);

            int x = Mathf.FloorToInt(localHitPoint.x);
            int y = Mathf.FloorToInt(localHitPoint.y);
            int z = Mathf.FloorToInt(localHitPoint.z);

            // Clamp to chunk bounds
            x = Mathf.Clamp(x, 0, Chunk.SIZE - 1);
            y = Mathf.Clamp(y, 0, Chunk.SIZE - 1);
            z = Mathf.Clamp(z, 0, Chunk.SIZE - 1);

            if (chunk.GetVoxel(x, y, z) == 0)
            {
                chunk.SetVoxel(x, y, z, 3); // Place grass block
                chunk.GenerateMesh();

                if (IsOwner)
                {
                    PlaceBlockServerRpc(chunk.Position, x, y, z, 3);
                }
            }
        }

        [ServerRpc]
        private void BreakBlockServerRpc(Vector3Int chunkPos, int x, int y, int z)
        {
            BreakBlockClientRpc(chunkPos, x, y, z);
        }

        [ClientRpc]
        private void BreakBlockClientRpc(Vector3Int chunkPos, int x, int y, int z)
        {
            // Handle block breaking for all clients
        }

        [ServerRpc]
        private void PlaceBlockServerRpc(Vector3Int chunkPos, int x, int y, int z, byte blockId)
        {
            PlaceBlockClientRpc(chunkPos, x, y, z, blockId);
        }

        [ClientRpc]
        private void PlaceBlockClientRpc(Vector3Int chunkPos, int x, int y, int z, byte blockId)
        {
            // Handle block placement for all clients
        }
    }
}
