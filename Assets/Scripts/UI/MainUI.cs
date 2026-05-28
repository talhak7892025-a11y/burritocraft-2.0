using UnityEngine;
using UnityEngine.UI;

namespace Burritocraft.UI
{
    public class MainUI : MonoBehaviour
    {
        [SerializeField] private Text fpsText;
        [SerializeField] private Text coordinatesText;
        [SerializeField] private Text modeText;

        private float frameRate = 0f;
        private Transform playerTransform;

        private void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }

        private void Update()
        {
            UpdateFPS();
            UpdateCoordinates();
        }

        private void UpdateFPS()
        {
            frameRate = 1f / Time.deltaTime;
            if (fpsText != null)
                fpsText.text = $"FPS: {frameRate:F1}";
        }

        private void UpdateCoordinates()
        {
            if (playerTransform != null && coordinatesText != null)
            {
                Vector3 pos = playerTransform.position;
                coordinatesText.text = $"X: {pos.x:F1} Y: {pos.y:F1} Z: {pos.z:F1}";
            }
        }
    }
}
