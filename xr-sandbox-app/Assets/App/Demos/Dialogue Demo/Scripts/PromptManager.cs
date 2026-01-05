using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace App.Demos.DialogueDemo.Scripts
{
    /// <summary>
    /// Singleton manager for showing VR prompts from anywhere in the code.
    /// Handles prompt queuing and provides static access for easy usage.
    /// </summary>
    public class PromptManager : MonoBehaviour
    {
        public static PromptManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private VRPrompt defaultPrompt;
        
        [Header("Queue Settings")]
        [SerializeField] private bool useQueue = true;
        [SerializeField] private float delayBetweenPrompts = 0.5f;
        
        private Queue<PromptData> promptQueue = new Queue<PromptData>();
        private bool isProcessingQueue = false;

        private struct PromptData
        {
            public string message;
            public float? duration;
        }

        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("PromptManager: Multiple instances detected. Destroying duplicate.");
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            
            // Optional: Don't destroy on load if you want it to persist across scenes
            // DontDestroyOnLoad(gameObject);
            
            // Validate references
            if (defaultPrompt == null)
            {
                defaultPrompt = GetComponentInChildren<VRPrompt>();
                if (defaultPrompt == null)
                {
                    Debug.LogError("PromptManager: No VRPrompt found! Please assign a default prompt.");
                }
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        /// <summary>
        /// Show a prompt with default duration
        /// </summary>
        public static void ShowPrompt(string message)
        {
            Instance?.ShowPromptInternal(message, null);
        }

        /// <summary>
        /// Show a prompt with custom duration
        /// </summary>
        public static void ShowPrompt(string message, float duration)
        {
            Instance?.ShowPromptInternal(message, duration);
        }

        /// <summary>
        /// Hide the current prompt immediately
        /// </summary>
        public static void HidePrompt()
        {
            if (Instance?.defaultPrompt != null)
            {
                Instance.defaultPrompt.HidePromptAsync().Forget();
            }
        }

        /// <summary>
        /// Clear all queued prompts
        /// </summary>
        public static void ClearQueue()
        {
            if (Instance != null)
            {
                Instance.promptQueue.Clear();
            }
        }

        private void ShowPromptInternal(string message, float? duration)
        {
            if (defaultPrompt == null)
            {
                Debug.LogError("PromptManager: Cannot show prompt - no default prompt assigned!");
                return;
            }

            if (useQueue)
            {
                // Add to queue
                promptQueue.Enqueue(new PromptData { message = message, duration = duration });
                
                // Start processing if not already running
                if (!isProcessingQueue)
                {
                    ProcessQueueAsync().Forget();
                }
            }
            else
            {
                // Show immediately, interrupting current prompt
                defaultPrompt.ShowPrompt(message, duration);
            }
        }

        private async UniTaskVoid ProcessQueueAsync()
        {
            isProcessingQueue = true;

            while (promptQueue.Count > 0)
            {
                var promptData = promptQueue.Dequeue();
                
                // Show the prompt and wait for it to complete
                await defaultPrompt.ShowPromptAsync(promptData.message, promptData.duration);
                
                // Wait for the prompt to finish displaying
                if (promptData.duration.HasValue)
                {
                    // Duration is handled in ShowPromptAsync
                }
                else
                {
                    // Wait for default duration + fade time
                    await UniTask.WaitWhile(() => defaultPrompt.IsShowing());
                }
                
                // Small delay between prompts
                if (promptQueue.Count > 0)
                {
                    await UniTask.WaitForSeconds(delayBetweenPrompts);
                }
            }

            isProcessingQueue = false;
        }

        /// <summary>
        /// Get the default prompt instance (for advanced usage)
        /// </summary>
        public static VRPrompt GetDefaultPrompt()
        {
            return Instance?.defaultPrompt;
        }

        /// <summary>
        /// Check if a prompt is currently showing
        /// </summary>
        public static bool IsShowingPrompt()
        {
            return Instance?.defaultPrompt != null && Instance.defaultPrompt.IsShowing();
        }

#if UNITY_EDITOR
        [ContextMenu("Test Show Prompt")]
        private void TestShowPrompt()
        {
            ShowPrompt("Test prompt from PromptManager!");
        }

        [ContextMenu("Test Queue Multiple Prompts")]
        private void TestQueuePrompts()
        {
            ShowPrompt("First prompt");
            ShowPrompt("Second prompt");
            ShowPrompt("Third prompt");
        }

        [ContextMenu("Test Clear Queue")]
        private void TestClearQueue()
        {
            ClearQueue();
        }
#endif
    }
}
