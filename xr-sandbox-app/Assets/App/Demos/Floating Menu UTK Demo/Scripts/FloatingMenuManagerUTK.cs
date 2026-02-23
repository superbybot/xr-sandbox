using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

namespace FloatingMenuDemo
{
    public class FloatingMenuManagerUTK : MonoBehaviour
    {
        [Header("UI Settings")]
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private float fadeDuration = 0.3f;
        [SerializeField] private float distanceFromHead = 1.5f;

        [Header("Follow Settings")]
        [SerializeField] private float positionSmoothSpeed = 5f;
        [SerializeField] private float rotationSmoothSpeed = 8f;
        [SerializeField] private Vector3 offset = new Vector3(0, -0.2f, 0);

        [Header("Input")]
        [SerializeField] private UnityEngine.InputSystem.InputActionProperty menuToggleAction;

        [Header("Countdown")]
        [SerializeField] private CountdownControllerUTK countdownController;

        private bool isMenuVisible = false;
        private Coroutine fadeCoroutine;
        private Transform mainCameraTransform;
        private VisualElement root;
        private Button startCountdownButton;

        private void Awake()
        {
            Debug.Log("[FloatingMenuManagerUTK] Awake called.");
            
            if (Camera.main != null)
            {
                mainCameraTransform = Camera.main.transform;
            }
            
            if (uiDocument != null)
            {
                root = uiDocument.rootVisualElement;
                if (root != null)
                {
                    root.style.opacity = 0f;
                    root.style.display = DisplayStyle.None;

                    startCountdownButton = root.Q<Button>("start-countdown-button");
                    if (startCountdownButton != null)
                    {
                        startCountdownButton.clicked += HandleStartCountdown;
                    }
                }
            }
            else
            {
                Debug.LogError("[FloatingMenuManagerUTK] UIDocument NOT assigned!");
            }
        }

        private void HandleStartCountdown()
        {
            if (countdownController != null)
            {
                HideMenu();
                countdownController.StartCountdown();
            }
            else
            {
                Debug.LogWarning("[FloatingMenuManagerUTK] CountdownController NOT assigned!");
            }
        }

        private void OnEnable()
        {
            if (menuToggleAction.action != null)
                menuToggleAction.action.Enable();
        }

        private void OnDisable()
        {
            if (menuToggleAction.action != null)
                menuToggleAction.action.Disable();
        }

        private void OnDestroy()
        {
            if (startCountdownButton != null)
            {
                startCountdownButton.clicked -= HandleStartCountdown;
            }
        }

        private void Update()
        {
            if (menuToggleAction.action != null && menuToggleAction.action.WasPerformedThisFrame())
            {
                ToggleMenu();
            }

            if (isMenuVisible && mainCameraTransform != null)
            {
                UpdatePosition();
            }
        }

        private void UpdatePosition()
        {
            Vector3 targetPos = mainCameraTransform.position + (mainCameraTransform.forward * distanceFromHead) + offset;
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * positionSmoothSpeed);
            
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
            if (root == null) return;
            
            isMenuVisible = true;
            root.style.display = DisplayStyle.Flex;
            
            if (mainCameraTransform != null)
            {
                Vector3 targetPos = mainCameraTransform.position + (mainCameraTransform.forward * distanceFromHead) + offset;
                transform.position = targetPos;
                transform.rotation = Quaternion.LookRotation(transform.position - mainCameraTransform.position);
            }

            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeUI(0f, 1f));
        }

        public void HideMenu()
        {
            if (root == null) return;
            
            isMenuVisible = false;

            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeUI(1f, 0f, () => root.style.display = DisplayStyle.None));
        }

        private IEnumerator FadeUI(float startAlpha, float endAlpha, System.Action onComplete = null)
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                root.style.opacity = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
                yield return null;
            }
            root.style.opacity = endAlpha;
            onComplete?.Invoke();
        }
    }
}
