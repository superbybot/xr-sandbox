using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

namespace FloatingMenuDemo
{
    public class FloatingMenuManager : MonoBehaviour
    {
        [Header("UI Settings")]
        [SerializeField] private CanvasGroup menuCanvasGroup;
        [SerializeField] private float fadeDuration = 0.3f;
        [SerializeField] private float distanceFromHead = 1.5f;

        [Header("Follow Settings")]
        [SerializeField] private float positionSmoothSpeed = 5f;
        [SerializeField] private float rotationSmoothSpeed = 8f;
        [SerializeField] private Vector3 offset = new Vector3(0, -0.2f, 0); // Slight offset down so it's not blocking view directly

        [Header("Input")]
        [SerializeField] private InputActionReference menuToggleAction;

        private bool isMenuVisible = false;
        private Coroutine fadeCoroutine;
        private Transform mainCameraTransform;

        private void Awake()
        {
            Debug.Log("[FloatingMenuManager] Awake called.");
            
            if (Camera.main != null)
            {
                mainCameraTransform = Camera.main.transform;
                Debug.Log("[FloatingMenuManager] Main camera found: " + mainCameraTransform.name);
            }
            else
            {
                Debug.LogWarning("[FloatingMenuManager] Main camera not found!");
            }
            
            // Initialize closed
            if (menuCanvasGroup != null)
            {
                Debug.Log("[FloatingMenuManager] Menu Canvas Group assigned. Initializing hidden.");
                menuCanvasGroup.alpha = 0f;
                menuCanvasGroup.interactable = false;
                menuCanvasGroup.blocksRaycasts = false;
                // DO NOT disable the gameObject, or this script won't run/receive input!
            }
            else
            {
                Debug.LogError("[FloatingMenuManager] Menu Canvas Group NOT assigned!");
            }
        }

        private void OnEnable()
        {
            if (menuToggleAction != null && menuToggleAction.action != null)
            {
                Debug.Log("[FloatingMenuManager] Enabling menu toggle action: " + menuToggleAction.action.name);
                menuToggleAction.action.Enable();
                menuToggleAction.action.performed += OnMenuTogglePerformed;
            }
            else
            {
                Debug.LogWarning("[FloatingMenuManager] Menu toggle action reference is null or missing action!");
            }
        }

        private void OnDisable()
        {
            if (menuToggleAction != null && menuToggleAction.action != null)
            {
                menuToggleAction.action.performed -= OnMenuTogglePerformed;
            }
        }

        private void OnMenuTogglePerformed(InputAction.CallbackContext context)
        {
            Debug.Log("[FloatingMenuManager] Menu button performed! Context: " + context.phase);
            ToggleMenu();
        }

        private void LateUpdate()
        {
            if (isMenuVisible && mainCameraTransform != null)
            {
                UpdatePosition();
            }
        }

        private void UpdatePosition()
        {
            Vector3 targetPos = mainCameraTransform.position + (mainCameraTransform.forward * distanceFromHead) + offset;
            
            // Smoothly interpolate position
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * positionSmoothSpeed);
            
            // Smoothly interpolate rotation to face user
            // We want the UI to look at the camera, but usually UI looks 'back' so we check direction
            Quaternion targetRotation = Quaternion.LookRotation(transform.position - mainCameraTransform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmoothSpeed);
        }

        public void ToggleMenu()
        {
            if (isMenuVisible)
                HideMenu();
            else
                ShowMenu();
        }

        public void ShowMenu()
        {
            Debug.Log("[FloatingMenuManager] ShowMenu called. Current visibility: " + isMenuVisible);
            if (menuCanvasGroup == null) return;
            
            isMenuVisible = true;
            menuCanvasGroup.interactable = true;
            menuCanvasGroup.blocksRaycasts = true;
            
            // Position in front of player immediately on open, then follow
            if (mainCameraTransform != null)
            {
                Vector3 targetPos = mainCameraTransform.position + (mainCameraTransform.forward * distanceFromHead) + offset;
                transform.position = targetPos;
                transform.rotation = Quaternion.LookRotation(transform.position - mainCameraTransform.position);
            }

            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeCanvas(0f, 1f));
        }

        public void HideMenu()
        {
            Debug.Log("[FloatingMenuManager] HideMenu called. Current visibility: " + isMenuVisible);
            if (menuCanvasGroup == null) return;
            
            isMenuVisible = false;
            menuCanvasGroup.interactable = false;
            menuCanvasGroup.blocksRaycasts = false;

            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeCanvas(1f, 0f));
        }

        private IEnumerator FadeCanvas(float startAlpha, float endAlpha)
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                menuCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
                yield return null;
            }
            menuCanvasGroup.alpha = endAlpha;
        }
    }
}
