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
        [SerializeField] private Transform targetPosition;
        [SerializeField] private float distanceFromCamera = 2f;
        [SerializeField] private Vector3 offset = new Vector3(0, 0.5f, 0);
        
        [Header("Scaling")]
        [SerializeField] private float fixedSize = 0.002f;
        [SerializeField] private float maxDistance = 50f;
        
        [Header("Smoothing")]
        [SerializeField] private float positionSmoothSpeed = 5f;
        [SerializeField] private float rotationSmoothSpeed = 8f;
        
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
            
            if (targetPosition != null)
            {
                targetPos = targetPosition.position + offset;
                distance = Vector3.Distance(mainCamera.transform.position, targetPos);
            }
            else
            {
                targetPos = mainCamera.transform.position + 
                           mainCamera.transform.forward * distanceFromCamera + 
                           offset;
                distance = distanceFromCamera;
            }
            
            bool isVisible = distance < maxDistance;
            
            if (targetPosition != null)
            {
                Vector3 directionToTarget = targetPos - mainCamera.transform.position;
                if (Vector3.Dot(directionToTarget, mainCamera.transform.forward) < 0)
                {
                    isVisible = false;
                }
            }
            
            if (canvas != null)
            {
                canvas.enabled = isVisible;
            }
            
            if (!isVisible) return;
            
            // Smoothly interpolate position and rotation instead of snapping
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * positionSmoothSpeed);
            
            Quaternion targetRotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmoothSpeed);
            
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
    }
}
