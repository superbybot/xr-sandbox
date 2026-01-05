using UnityEngine;

namespace App.Demos.DialogueDemo.Scripts
{
    /// <summary>
    /// Keeps text in world space, facing the camera and properly scaled.
    /// Based on NorthStar's FloatingText implementation, simplified for user-facing prompts.
    /// </summary>
    public class FloatingText : MonoBehaviour
    {
        [Header("Positioning")]
        [SerializeField] private Transform targetPosition; // Optional: attach to character (null = follow user)
        [SerializeField] private float distanceFromCamera = 2f; // Distance in front of camera when user-facing
        [SerializeField] private Vector3 offset = new Vector3(0, 0.5f, 0); // Offset from target/camera
        
        [Header("Scaling")]
        [SerializeField] private float fixedSize = 0.002f; // Size multiplier based on distance
        [SerializeField] private float maxDistance = 50f; // Hide if further than this
        
        [Header("References")]
        [SerializeField] private Canvas canvas;
        
        private Camera mainCamera;
        private bool isInitialized = false;

        private void Awake()
        {
            if (canvas == null)
                canvas = GetComponent<Canvas>();
        }

        private void Start()
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("FloatingText: No main camera found!");
                enabled = false;
                return;
            }
            isInitialized = true;
        }

        private void LateUpdate()
        {
            if (!isInitialized || mainCamera == null) return;
            
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            Vector3 targetPos;
            float distance;
            
            // Determine target position
            if (targetPosition != null)
            {
                // Character-facing mode (when enabled)
                targetPos = targetPosition.position + offset;
                distance = Vector3.Distance(mainCamera.transform.position, targetPos);
            }
            else
            {
                // User-facing mode (default)
                // Position in front of camera
                targetPos = mainCamera.transform.position + 
                           mainCamera.transform.forward * distanceFromCamera + 
                           offset;
                distance = distanceFromCamera;
            }
            
            // Check if within visible range
            bool isVisible = distance < maxDistance;
            
            // Check if behind camera (only relevant for character-facing mode)
            if (targetPosition != null)
            {
                Vector3 directionToTarget = targetPos - mainCamera.transform.position;
                if (Vector3.Dot(directionToTarget, mainCamera.transform.forward) < 0)
                {
                    isVisible = false;
                }
            }
            
            // Update visibility
            if (canvas != null)
            {
                canvas.enabled = isVisible;
            }
            
            if (!isVisible) return;
            
            // Update position
            transform.position = targetPos;
            
            // Face camera
            transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
            
            // Scale based on distance
            float scale = distance * fixedSize * mainCamera.fieldOfView;
            transform.localScale = Vector3.one * scale;
        }

        /// <summary>
        /// Immediately sync position without waiting for LateUpdate
        /// </summary>
        public void SyncPosition()
        {
            if (isInitialized)
            {
                UpdatePosition();
            }
        }

        /// <summary>
        /// Set a target to follow (for character attachment)
        /// </summary>
        public void SetTarget(Transform target)
        {
            targetPosition = target;
        }

        /// <summary>
        /// Clear target to return to user-facing mode
        /// </summary>
        public void ClearTarget()
        {
            targetPosition = null;
        }

        private void OnDrawGizmosSelected()
        {
            if (mainCamera == null) mainCamera = Camera.main;
            if (mainCamera == null) return;
            
            // Draw target position
            Vector3 targetPos = targetPosition != null 
                ? targetPosition.position + offset
                : mainCamera.transform.position + mainCamera.transform.forward * distanceFromCamera + offset;
            
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(targetPos, 0.1f);
            
            // Draw line from camera to target
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(mainCamera.transform.position, targetPos);
        }
    }
}
