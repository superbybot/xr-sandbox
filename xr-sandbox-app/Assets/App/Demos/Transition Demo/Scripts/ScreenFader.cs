using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace App.Demos.TransitionDemo.Scripts
{
    /// <summary>
    /// Controls screen fade transitions using Post-Processing Volume.
    /// Inspired by NorthStar's ScreenFader, adapted to use UniTask and ColorAdjustments.
    /// </summary>
    public class ScreenFader : MonoBehaviour
    {
        [Header("Volume Reference")]
        [SerializeField] private Volume fadeVolume;
        
        [Header("Fade Settings")]
        [SerializeField] private float defaultFadeDuration = 1.0f;
        [SerializeField] private float fadeToBlackValue = -10f;
        [SerializeField] private float fadeToWhiteValue = 10f;
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = false;
        
        // Multiple fade values that combine (similar to NorthStar)
        public float ManualFadeValue { get; private set; }
        public float TeleportFadeValue { get; private set; }
        public float TimedFadeValue { get; private set; }
        
        private ColorAdjustments colorAdjustments;
        
        // Singleton pattern
        public static ScreenFader Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("Multiple ScreenFader instances found. Destroying duplicate.");
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            
            // Get ColorAdjustments from the volume profile
            if (fadeVolume != null && fadeVolume.profile.TryGet(out ColorAdjustments ca))
            {
                colorAdjustments = ca;
            }
            else
            {
                Debug.LogError("ScreenFader: Volume Profile must have ColorAdjustments override!");
            }
        }

        private void Update()
        {
            if (fadeVolume == null || colorAdjustments == null) return;
            
            // Combine all fade values (similar to NorthStar)
            float totalFade = ManualFadeValue + TeleportFadeValue + TimedFadeValue;
            
            // Set the volume weight based on fade intensity
            // Weight 0 = disabled (performance optimization)
            // Weight 1 = fully active
            fadeVolume.weight = Mathf.Abs(totalFade) > 0.01f ? 1f : 0f;
            
            // Set the postExposure value
            colorAdjustments.postExposure.value = totalFade;
        }

        #region Async Methods

        /// <summary>
        /// Fade to black (async)
        /// </summary>
        public async UniTask FadeToBlackAsync(float duration = -1f)
        {
            if (duration < 0) duration = defaultFadeDuration;
            
            if (enableDebugLogs)
                Debug.Log($"ScreenFader: Fading to black over {duration}s");
            
            await FadeManualAsync(fadeToBlackValue, duration);
        }

        /// <summary>
        /// Fade to clear/normal (async)
        /// </summary>
        public async UniTask FadeToClearAsync(float duration = -1f)
        {
            if (duration < 0) duration = defaultFadeDuration;
            
            if (enableDebugLogs)
                Debug.Log($"ScreenFader: Fading to clear over {duration}s");
            
            await FadeManualAsync(0f, duration);
        }

        /// <summary>
        /// Fade to white (async)
        /// </summary>
        public async UniTask FadeToWhiteAsync(float duration = -1f)
        {
            if (duration < 0) duration = defaultFadeDuration;
            
            if (enableDebugLogs)
                Debug.Log($"ScreenFader: Fading to white over {duration}s");
            
            await FadeManualAsync(fadeToWhiteValue, duration);
        }

        /// <summary>
        /// Fade out, hold, then fade in (async)
        /// Useful for teleports and scene transitions
        /// </summary>
        public async UniTask FadeOutAndInAsync(float fadeOutDuration = -1f, float holdDuration = 0.1f, float fadeInDuration = -1f)
        {
            if (fadeOutDuration < 0) fadeOutDuration = defaultFadeDuration;
            if (fadeInDuration < 0) fadeInDuration = defaultFadeDuration;
            
            if (enableDebugLogs)
                Debug.Log($"ScreenFader: Fade out/in sequence - Out:{fadeOutDuration}s Hold:{holdDuration}s In:{fadeInDuration}s");
            
            // Fade to black
            await FadeToBlackAsync(fadeOutDuration);
            
            // Hold
            if (holdDuration > 0)
                await UniTask.WaitForSeconds(holdDuration);
            
            // Fade back to clear
            await FadeToClearAsync(fadeInDuration);
        }

        /// <summary>
        /// Fade manual value to target over duration
        /// </summary>
        private async UniTask FadeManualAsync(float targetValue, float duration)
        {
            float startValue = ManualFadeValue;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                ManualFadeValue = Mathf.Lerp(startValue, targetValue, t);
                await UniTask.Yield();
            }
            
            ManualFadeValue = targetValue;
        }

        #endregion

        #region Fire-and-Forget Methods

        /// <summary>
        /// Fade to black (fire and forget)
        /// </summary>
        public void FadeToBlack(float duration = -1f)
        {
            FadeToBlackAsync(duration).Forget();
        }

        /// <summary>
        /// Fade to clear (fire and forget)
        /// </summary>
        public void FadeToClear(float duration = -1f)
        {
            FadeToClearAsync(duration).Forget();
        }

        /// <summary>
        /// Fade to white (fire and forget)
        /// </summary>
        public void FadeToWhite(float duration = -1f)
        {
            FadeToWhiteAsync(duration).Forget();
        }

        /// <summary>
        /// Fade out and in (fire and forget)
        /// </summary>
        public void FadeOutAndIn(float fadeOutDuration = -1f, float holdDuration = 0.1f, float fadeInDuration = -1f)
        {
            FadeOutAndInAsync(fadeOutDuration, holdDuration, fadeInDuration).Forget();
        }

        #endregion

        #region Instant Methods

        /// <summary>
        /// Set fade value immediately without animation
        /// </summary>
        public void SetFadeImmediate(float fadeValue)
        {
            ManualFadeValue = fadeValue;
            
            if (enableDebugLogs)
                Debug.Log($"ScreenFader: Set fade immediate to {fadeValue}");
        }

        /// <summary>
        /// Clear all fade immediately
        /// </summary>
        public void ClearFadeImmediate()
        {
            ManualFadeValue = 0f;
            TeleportFadeValue = 0f;
            TimedFadeValue = 0f;
            
            if (enableDebugLogs)
                Debug.Log("ScreenFader: Cleared all fades immediately");
        }

        #endregion

        #region Teleport Fade Methods (for integration with teleport systems)

        /// <summary>
        /// Fade for teleport (uses separate fade value)
        /// </summary>
        public async UniTask TeleportFadeAsync(float fadeOutDuration = 0.3f, float holdDuration = 0.1f, float fadeInDuration = 0.3f)
        {
            float startValue = TeleportFadeValue;
            float elapsed = 0f;
            
            // Fade out
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / fadeOutDuration);
                TeleportFadeValue = Mathf.Lerp(startValue, fadeToBlackValue, t);
                await UniTask.Yield();
            }
            
            TeleportFadeValue = fadeToBlackValue;
            
            // Hold
            if (holdDuration > 0)
                await UniTask.WaitForSeconds(holdDuration);
            
            // Fade in
            elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / fadeInDuration);
                TeleportFadeValue = Mathf.Lerp(fadeToBlackValue, 0f, t);
                await UniTask.Yield();
            }
            
            TeleportFadeValue = 0f;
        }

        #endregion

        #region Context Menu (for testing in editor)

        [ContextMenu("Test: Fade to Black")]
        private void TestFadeToBlack()
        {
            FadeToBlack(1f);
        }

        [ContextMenu("Test: Fade to Clear")]
        private void TestFadeToClear()
        {
            FadeToClear(1f);
        }

        [ContextMenu("Test: Fade Out and In")]
        private void TestFadeOutAndIn()
        {
            FadeOutAndIn(0.5f, 0.2f, 0.5f);
        }

        [ContextMenu("Test: Fade to White")]
        private void TestFadeToWhite()
        {
            FadeToWhite(1f);
        }

        #endregion
    }
}
