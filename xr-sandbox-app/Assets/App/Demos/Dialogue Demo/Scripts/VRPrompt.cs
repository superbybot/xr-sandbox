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
        
        private bool isShowing = false;
        private float timeLastDisplayed;

        private void Awake()
        {
            if (promptText == null)
                promptText = GetComponentInChildren<TextMeshProUGUI>();
            
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
            
            if (floatingText == null)
                floatingText = GetComponent<FloatingText>();
            
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0;
            }
        }

        private void Update()
        {
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
            
            if (promptText != null)
            {
                promptText.text = message;
            }
            
            if (floatingText != null)
            {
                floatingText.SyncPosition();
            }
            
            isShowing = true;
            timeLastDisplayed = Time.time;
            
            if (canvasGroup != null)
            {
                await FadeToAsync(1f, fadeDuration);
            }
            
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
    }
}
