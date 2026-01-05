using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace App.Demos.DialogueDemo.Scripts
{
    /// <summary>
    /// Displays text prompts in VR with fade in/out animations.
    /// Based on NorthStar's Subtitle system, simplified for user-facing prompts.
    /// </summary>
    public class VRPrompt : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI promptText;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private FloatingText floatingText;
        
        [Header("Animation Settings")]
        [SerializeField] private float displayDuration = 3f;
        [SerializeField] private float fadeDuration = 0.5f;
        
        // [Header("Character Attachment (Optional)")]
        // [Tooltip("Enable this to attach prompt to a character instead of showing to user")]
        // [SerializeField] private bool attachToCharacter = false;
        // [SerializeField] private Transform characterAttachPoint;
        
        private bool isShowing = false;
        private float timeLastDisplayed;

        private void Awake()
        {
            // Auto-find references if not set
            if (promptText == null)
                promptText = GetComponentInChildren<TextMeshProUGUI>();
            
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
            
            if (floatingText == null)
                floatingText = GetComponent<FloatingText>();
            
            // Start hidden
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0;
            }
        }

        private void Start()
        {
            // Uncomment to enable character attachment
            // if (attachToCharacter && characterAttachPoint != null && floatingText != null)
            // {
            //     floatingText.SetTarget(characterAttachPoint);
            // }
        }

        private void Update()
        {
            // Auto-hide after duration
            if (isShowing && Time.time - timeLastDisplayed > displayDuration)
            {
                HidePromptAsync().Forget();
            }
        }

        /// <summary>
        /// Show a prompt with fade in/out animation (async)
        /// </summary>
        public async UniTask ShowPromptAsync(string message, float? customDuration = null)
        {
            if (string.IsNullOrEmpty(message))
            {
                Debug.LogWarning("VRPrompt: Attempted to show empty message");
                return;
            }
            
            // Set text
            if (promptText != null)
            {
                promptText.text = message;
            }
            
            // Update position immediately
            if (floatingText != null)
            {
                floatingText.SyncPosition();
            }
            
            // Fade in
            isShowing = true;
            timeLastDisplayed = Time.time;
            
            if (canvasGroup != null)
            {
                await FadeToAsync(1f, fadeDuration);
            }
            
            // Wait for display duration if custom duration provided
            if (customDuration.HasValue)
            {
                await UniTask.WaitForSeconds(customDuration.Value);
                await HidePromptAsync();
            }
        }

        /// <summary>
        /// Show a prompt immediately without async (fire and forget)
        /// </summary>
        public void ShowPrompt(string message, float? customDuration = null)
        {
            ShowPromptAsync(message, customDuration).Forget();
        }

        /// <summary>
        /// Hide the prompt with fade out animation
        /// </summary>
        public async UniTask HidePromptAsync()
        {
            if (!isShowing) return;
            
            isShowing = false;
            
            if (canvasGroup != null)
            {
                await FadeToAsync(0f, fadeDuration);
            }
        }

        /// <summary>
        /// Hide the prompt immediately without animation
        /// </summary>
        public void HideImmediate()
        {
            isShowing = false;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0;
            }
        }

        /// <summary>
        /// Check if prompt is currently showing
        /// </summary>
        public bool IsShowing() => isShowing && canvasGroup != null && canvasGroup.alpha > 0;

        /// <summary>
        /// Fade canvas group to target alpha over duration
        /// </summary>
        private async UniTask FadeToAsync(float targetAlpha, float duration)
        {
            if (canvasGroup == null) return;
            
            float startAlpha = canvasGroup.alpha;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                await UniTask.Yield();
            }
            
            canvasGroup.alpha = targetAlpha;
        }

        // Uncomment to enable character attachment feature
        // /// <summary>
        // /// Attach this prompt to a character's position
        // /// </summary>
        // public void AttachToCharacter(Transform characterTransform)
        // {
        //     if (floatingText != null && characterTransform != null)
        //     {
        //         floatingText.SetTarget(characterTransform);
        //         attachToCharacter = true;
        //         characterAttachPoint = characterTransform;
        //     }
        // }
        
        // /// <summary>
        // /// Detach from character and return to user-facing mode
        // /// </summary>
        // public void DetachFromCharacter()
        // {
        //     if (floatingText != null)
        //     {
        //         floatingText.ClearTarget();
        //         attachToCharacter = false;
        //         characterAttachPoint = null;
        //     }
        // }

#if UNITY_EDITOR
        [ContextMenu("Test Show Prompt")]
        private void TestShowPrompt()
        {
            ShowPrompt("This is a test prompt!");
        }

        [ContextMenu("Test Hide Prompt")]
        private void TestHidePrompt()
        {
            HidePromptAsync().Forget();
        }
#endif
    }
}
