using Cysharp.Threading.Tasks;
using UnityEngine;

namespace App.Demos.TransitionDemo.Scripts
{
    /// <summary>
    /// Demo controller showing programmatic usage of the ScreenFader system.
    /// Demonstrates various fade transition scenarios and use cases.
    /// </summary>
    public class TransitionDemoController : MonoBehaviour
    {
        [Header("Welcome Sequence")]
        [SerializeField] private bool playWelcomeSequence = true;
        [SerializeField] private float welcomeDelay = 0.5f;
        
        [Header("Auto Demo Sequence")]
        [SerializeField] private bool autoStartDemoSequence = false;
        [SerializeField] private float demoSequenceDelay = 5f;
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;

        private void Start()
        {
            if (playWelcomeSequence)
            {
                PlayWelcomeSequenceAsync().Forget();
            }
            
            if (autoStartDemoSequence)
            {
                StartDemoSequenceAsync().Forget();
            }
        }

        /// <summary>
        /// Welcome sequence: Fade in from black on scene start
        /// </summary>
        private async UniTask PlayWelcomeSequenceAsync()
        {
            if (ScreenFader.Instance == null)
            {
                Debug.LogError("TransitionDemoController: ScreenFader.Instance is null!");
                return;
            }
            
            // Start with screen black
            ScreenFader.Instance.SetFadeImmediate(ScreenFader.Instance.ManualFadeValue - 10f);
            
            // Wait a moment
            await UniTask.WaitForSeconds(welcomeDelay);
            
            // Fade in to clear
            await ScreenFader.Instance.FadeToClearAsync(2f);
            
            if (enableDebugLogs)
                Debug.Log("TransitionDemoController: Welcome sequence complete");
        }

        /// <summary>
        /// Automated demo sequence showing different fade types
        /// </summary>
        private async UniTask StartDemoSequenceAsync()
        {
            await UniTask.WaitForSeconds(demoSequenceDelay);
            
            if (enableDebugLogs)
                Debug.Log("TransitionDemoController: Starting demo sequence");
            
            // 1. Fade to black
            if (enableDebugLogs)
                Debug.Log("Demo: Fade to black");
            await ScreenFader.Instance.FadeToBlackAsync(1f);
            await UniTask.WaitForSeconds(1f);
            
            // 2. Fade to clear
            if (enableDebugLogs)
                Debug.Log("Demo: Fade to clear");
            await ScreenFader.Instance.FadeToClearAsync(1f);
            await UniTask.WaitForSeconds(2f);
            
            // 3. Fade to white (flashbang effect)
            if (enableDebugLogs)
                Debug.Log("Demo: Fade to white");
            await ScreenFader.Instance.FadeToWhiteAsync(0.5f);
            await UniTask.WaitForSeconds(1f);
            
            // 4. Fade back to clear
            if (enableDebugLogs)
                Debug.Log("Demo: Fade back to clear");
            await ScreenFader.Instance.FadeToClearAsync(1f);
            await UniTask.WaitForSeconds(2f);
            
            // 5. Fade out and in (teleport simulation)
            if (enableDebugLogs)
                Debug.Log("Demo: Fade out and in (teleport)");
            await ScreenFader.Instance.FadeOutAndInAsync(0.5f, 0.2f, 0.5f);
            
            if (enableDebugLogs)
                Debug.Log("TransitionDemoController: Demo sequence complete");
        }

        #region Example Use Cases (can be called from UI or other scripts)

        /// <summary>
        /// Example: Simulate a teleport with fade transition
        /// </summary>
        public async UniTask SimulateTeleportAsync(Vector3 targetPosition)
        {
            if (enableDebugLogs)
                Debug.Log($"Simulating teleport to {targetPosition}");
            
            // Fade out
            await ScreenFader.Instance.FadeToBlackAsync(0.3f);
            
            // Move player (in real scenario, this would be actual teleport logic)
            // transform.position = targetPosition;
            
            // Short hold
            await UniTask.WaitForSeconds(0.1f);
            
            // Fade in
            await ScreenFader.Instance.FadeToClearAsync(0.3f);
        }

        /// <summary>
        /// Example: Simulate a scene transition with extended fade
        /// </summary>
        public async UniTask SimulateSceneTransitionAsync(string sceneName)
        {
            if (enableDebugLogs)
                Debug.Log($"Simulating scene transition to {sceneName}");
            
            // Fade to black
            await ScreenFader.Instance.FadeToBlackAsync(1.5f);
            
            // Load scene (in real scenario)
            // SceneManager.LoadScene(sceneName);
            await UniTask.WaitForSeconds(1f);
            
            // Fade in
            await ScreenFader.Instance.FadeToClearAsync(1.5f);
        }

        /// <summary>
        /// Example: Death/respawn effect
        /// </summary>
        public async UniTask SimulateDeathRespawnAsync()
        {
            if (enableDebugLogs)
                Debug.Log("Simulating death/respawn");
            
            // Quick fade to black
            await ScreenFader.Instance.FadeToBlackAsync(0.5f);
            
            // Hold in black
            await UniTask.WaitForSeconds(1f);
            
            // Respawn logic here
            
            // Fade back in
            await ScreenFader.Instance.FadeToClearAsync(1f);
        }

        /// <summary>
        /// Example: Flashbang/stun effect
        /// </summary>
        public async UniTask SimulateFlashbangAsync()
        {
            if (enableDebugLogs)
                Debug.Log("Simulating flashbang effect");
            
            // Instant white
            ScreenFader.Instance.SetFadeImmediate(10f);
            
            // Fade back to normal
            await ScreenFader.Instance.FadeToClearAsync(2f);
        }

        #endregion

        #region Context Menu (for testing in editor)

        [ContextMenu("Play Welcome Sequence")]
        private void TestWelcomeSequence()
        {
            PlayWelcomeSequenceAsync().Forget();
        }

        [ContextMenu("Play Demo Sequence")]
        private void TestDemoSequence()
        {
            StartDemoSequenceAsync().Forget();
        }

        [ContextMenu("Simulate Teleport")]
        private void TestTeleport()
        {
            SimulateTeleportAsync(Vector3.zero).Forget();
        }

        [ContextMenu("Simulate Flashbang")]
        private void TestFlashbang()
        {
            SimulateFlashbangAsync().Forget();
        }

        #endregion
    }
}
