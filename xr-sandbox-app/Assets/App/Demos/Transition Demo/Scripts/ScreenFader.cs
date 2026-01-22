using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace App.Demos.TransitionDemo.Scripts
{
    public class ScreenFader : MonoBehaviour
    {
        [Header("Volume Reference")]
        [SerializeField] private Volume fadeVolume;
        
        [Header("Fade Settings")]
        [SerializeField] private float defaultFadeDuration = 1.0f;
        [SerializeField] private float fadeToBlackValue = -10f;
        [SerializeField] private float fadeToWhiteValue = 10f;
        
        public float ManualFadeValue { get; private set; }
        public float TeleportFadeValue { get; private set; }
        public float TimedFadeValue { get; private set; }
        
        private ColorAdjustments colorAdjustments;

        private void Awake()
        {
            Debug.Log($"[ScreenFader] Awake - fadeVolume: {fadeVolume}");
            
            if (fadeVolume == null)
            {
                Debug.LogError("[ScreenFader] fadeVolume is NULL! Assign it in Inspector.");
                return;
            }
            
            if (fadeVolume.profile == null)
            {
                Debug.LogError("[ScreenFader] fadeVolume.profile is NULL! Assign a Volume Profile.");
                return;
            }
            
            if (fadeVolume.profile.TryGet(out ColorAdjustments ca))
            {
                colorAdjustments = ca;
                Debug.Log($"[ScreenFader] ColorAdjustments found. postExposure override: {ca.postExposure.overrideState}");
            }
            else
            {
                Debug.LogError("[ScreenFader] Volume Profile must have ColorAdjustments override!");
            }
        }

        private void Update()
        {
            if (fadeVolume == null || colorAdjustments == null) return;
            
            float totalFade = ManualFadeValue + TeleportFadeValue + TimedFadeValue;
            float newWeight = Mathf.Abs(totalFade) > 0.01f ? 1f : 0f;
            
            if (Mathf.Abs(fadeVolume.weight - newWeight) > 0.01f || Mathf.Abs(colorAdjustments.postExposure.value - totalFade) > 0.1f)
            {
                Debug.Log($"[ScreenFader] Update - totalFade: {totalFade:F2}, weight: {newWeight}, postExposure: {totalFade:F2}");
            }
            
            fadeVolume.weight = newWeight;
            colorAdjustments.postExposure.value = totalFade;
        }

        #region Async Methods

        public async UniTask FadeToBlackAsync(float duration = -1f)
        {
            if (duration < 0) duration = defaultFadeDuration;
            Debug.Log($"[ScreenFader] FadeToBlackAsync started - duration: {duration}s, target: {fadeToBlackValue}");
            await FadeManualAsync(fadeToBlackValue, duration);
            Debug.Log("[ScreenFader] FadeToBlackAsync completed");
        }

        public async UniTask FadeToClearAsync(float duration = -1f)
        {
            if (duration < 0) duration = defaultFadeDuration;
            Debug.Log($"[ScreenFader] FadeToClearAsync started - duration: {duration}s, target: 0");
            await FadeManualAsync(0f, duration);
            Debug.Log("[ScreenFader] FadeToClearAsync completed");
        }

        public async UniTask FadeToWhiteAsync(float duration = -1f)
        {
            if (duration < 0) duration = defaultFadeDuration;
            await FadeManualAsync(fadeToWhiteValue, duration);
        }

        public async UniTask FadeOutAndInAsync(float fadeOutDuration = -1f, float holdDuration = 0.1f, float fadeInDuration = -1f)
        {
            if (fadeOutDuration < 0) fadeOutDuration = defaultFadeDuration;
            if (fadeInDuration < 0) fadeInDuration = defaultFadeDuration;
            
            await FadeToBlackAsync(fadeOutDuration);
            
            if (holdDuration > 0)
                await UniTask.WaitForSeconds(holdDuration);
            
            await FadeToClearAsync(fadeInDuration);
        }

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

        public void FadeToBlack(float duration = -1f)
        {
            FadeToBlackAsync(duration).Forget();
        }

        public void FadeToClear(float duration = -1f)
        {
            FadeToClearAsync(duration).Forget();
        }

        public void FadeToWhite(float duration = -1f)
        {
            FadeToWhiteAsync(duration).Forget();
        }

        public void FadeOutAndIn(float fadeOutDuration = -1f, float holdDuration = 0.1f, float fadeInDuration = -1f)
        {
            FadeOutAndInAsync(fadeOutDuration, holdDuration, fadeInDuration).Forget();
        }

        #endregion

        #region Instant Methods

        public void SetFadeImmediate(float fadeValue)
        {
            Debug.Log($"[ScreenFader] SetFadeImmediate - value: {fadeValue}");
            ManualFadeValue = fadeValue;
        }

        public void ClearFadeImmediate()
        {
            ManualFadeValue = 0f;
            TeleportFadeValue = 0f;
            TimedFadeValue = 0f;
        }

        #endregion

        #region Teleport Fade Methods

        public async UniTask TeleportFadeAsync(float fadeOutDuration = 0.3f, float holdDuration = 0.1f, float fadeInDuration = 0.3f)
        {
            float startValue = TeleportFadeValue;
            float elapsed = 0f;
            
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / fadeOutDuration);
                TeleportFadeValue = Mathf.Lerp(startValue, fadeToBlackValue, t);
                await UniTask.Yield();
            }
            
            TeleportFadeValue = fadeToBlackValue;
            
            if (holdDuration > 0)
                await UniTask.WaitForSeconds(holdDuration);
            
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

        #region Context Menu

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
