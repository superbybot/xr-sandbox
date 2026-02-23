using Cysharp.Threading.Tasks;
using UnityEngine;

namespace App.Demos.TransitionDemo.Scripts
{
    public class ScreenFader : MonoBehaviour
    {
        [Header("Fade Settings")]
        [SerializeField] private float defaultFadeDuration = 1.0f;
        [SerializeField] private Color fadeColor = new Color(0.01f, 0.01f, 0.01f, 1.0f);
        
        public float ManualFadeValue { get; private set; }
        public float TeleportFadeValue { get; private set; }
        public float TimedFadeValue { get; private set; }
        
        private OVRScreenFade ovrScreenFade;

        private void Awake()
        {
            SetupOVRScreenFade();
        }

        private void SetupOVRScreenFade()
        {
            ovrScreenFade = FindFirstObjectByType<OVRScreenFade>();
            
            // Find the active camera to parent the fader to
            Camera mainCam = Camera.main;
            if (mainCam == null) mainCam = FindFirstObjectByType<Camera>();

            if (ovrScreenFade == null)
            {
                // Create it if it doesn't exist
                GameObject faderObj = new GameObject("OVRScreenFade_Runtime");
                ovrScreenFade = faderObj.AddComponent<OVRScreenFade>();
                Debug.Log("[ScreenFader] OVRScreenFade created at runtime.");
            }

            // FORCE PARENTING to the camera so it follows the head
            if (mainCam != null)
            {
                ovrScreenFade.transform.SetParent(mainCam.transform, false);
                ovrScreenFade.transform.localPosition = Vector3.zero;
                ovrScreenFade.transform.localRotation = Quaternion.identity;
                Debug.Log($"[ScreenFader] Fader parented to camera: {mainCam.name}");
            }
            else
            {
                Debug.LogWarning("[ScreenFader] No camera found! Fade will not follow the head.");
            }

            ovrScreenFade.fadeColor = fadeColor;
            ovrScreenFade.fadeTime = defaultFadeDuration;
        }

        private void Update()
        {
            if (ovrScreenFade == null) return;
            
            // OVRScreenFade takes the MAX of its internal alphas.
            // We combine our values and set them explicitly.
            float totalFade = Mathf.Clamp01(Mathf.Max(ManualFadeValue, TeleportFadeValue, TimedFadeValue));
            
            // Only update if value changed significantly
            if (Mathf.Abs(ovrScreenFade.currentAlpha - totalFade) > 0.001f)
            {
                ovrScreenFade.SetExplicitFade(totalFade);
            }
        }

        #region Async Methods

        public async UniTask FadeToBlackAsync(float duration = -1f)
        {
            if (duration < 0) duration = defaultFadeDuration;
            Debug.Log($"[ScreenFader] FadeToBlackAsync started - duration: {duration}s");
            await FadeManualAsync(1f, duration);
            Debug.Log("[ScreenFader] FadeToBlackAsync completed");
        }

        public async UniTask FadeToClearAsync(float duration = -1f)
        {
            if (duration < 0) duration = defaultFadeDuration;
            Debug.Log($"[ScreenFader] FadeToClearAsync started - duration: {duration}s");
            await FadeManualAsync(0f, duration);
            Debug.Log("[ScreenFader] FadeToClearAsync completed");
        }

        public async UniTask FadeToWhiteAsync(float duration = -1f)
        {
            // Note: OVRScreenFade default implementation is usually black-based.
            // To support white, we'd need to swap fadeColor temporarily.
            Color originalColor = ovrScreenFade.fadeColor;
            ovrScreenFade.fadeColor = Color.white;
            
            if (duration < 0) duration = defaultFadeDuration;
            await FadeManualAsync(1f, duration);
            
            // Keep white until we fade back to clear
            ovrScreenFade.fadeColor = originalColor;
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
            ManualFadeValue = Mathf.Clamp01(Mathf.Abs(fadeValue) > 0.1f ? 1f : 0f);
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
                TeleportFadeValue = Mathf.Lerp(startValue, 1f, t);
                await UniTask.Yield();
            }
            
            TeleportFadeValue = 1f;
            
            if (holdDuration > 0)
                await UniTask.WaitForSeconds(holdDuration);
            
            elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / fadeInDuration);
                TeleportFadeValue = Mathf.Lerp(1f, 0f, t);
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
