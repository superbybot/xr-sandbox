using Cysharp.Threading.Tasks;
using UnityEngine;

namespace App.Demos.TransitionDemo.Scripts
{
    public class TransitionDemoController : MonoBehaviour
    {
        [Header("Screen Fader Reference")]
        [SerializeField] private ScreenFader screenFader;
        
        [Header("Welcome Sequence")]
        [SerializeField] private bool playWelcomeSequence = true;
        [SerializeField] private float welcomeDelay = 0.5f;
        
        [Header("Auto Demo Sequence")]
        [SerializeField] private bool autoStartDemoSequence = false;
        [SerializeField] private float demoSequenceDelay = 5f;

        private void Start()
        {
            if (screenFader == null)
            {
                Debug.LogError("TransitionDemoController: ScreenFader reference is not assigned!");
                return;
            }
            
            if (playWelcomeSequence)
            {
                PlayWelcomeSequenceAsync().Forget();
            }
            
            if (autoStartDemoSequence)
            {
                StartDemoSequenceAsync().Forget();
            }
        }

        private async UniTask PlayWelcomeSequenceAsync()
        {
            screenFader.SetFadeImmediate(-10f);
            await UniTask.WaitForSeconds(welcomeDelay);
            await screenFader.FadeToClearAsync(2f);
        }

        private async UniTask StartDemoSequenceAsync()
        {
            await UniTask.WaitForSeconds(demoSequenceDelay);
            
            await screenFader.FadeToBlackAsync(1f);
            await UniTask.WaitForSeconds(1f);
            
            await screenFader.FadeToClearAsync(1f);
            await UniTask.WaitForSeconds(2f);
            
            await screenFader.FadeToWhiteAsync(0.5f);
            await UniTask.WaitForSeconds(1f);
            
            await screenFader.FadeToClearAsync(1f);
            await UniTask.WaitForSeconds(2f);
            
            await screenFader.FadeOutAndInAsync(0.5f, 0.2f, 0.5f);
        }

        #region Example Use Cases

        public async UniTask SimulateTeleportAsync(Vector3 targetPosition)
        {
            await screenFader.FadeToBlackAsync(0.3f);
            await UniTask.WaitForSeconds(0.1f);
            await screenFader.FadeToClearAsync(0.3f);
        }

        public async UniTask SimulateSceneTransitionAsync(string sceneName)
        {
            await screenFader.FadeToBlackAsync(1.5f);
            await UniTask.WaitForSeconds(1f);
            await screenFader.FadeToClearAsync(1.5f);
        }

        public async UniTask SimulateDeathRespawnAsync()
        {
            await screenFader.FadeToBlackAsync(0.5f);
            await UniTask.WaitForSeconds(1f);
            await screenFader.FadeToClearAsync(1f);
        }

        public async UniTask SimulateFlashbangAsync()
        {
            screenFader.SetFadeImmediate(10f);
            await screenFader.FadeToClearAsync(2f);
        }

        #endregion

        #region Context Menu

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
