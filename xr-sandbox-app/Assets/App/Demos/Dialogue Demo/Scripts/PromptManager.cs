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

        [Header("Prefab")]
        [SerializeField] private GameObject vrPromptPrefab;
        
        [Header("Queue Settings")]
        [SerializeField] private bool useQueue = true;
        [SerializeField] private float delayBetweenPrompts = 0.5f;
        
        private VRPrompt defaultPrompt;
        private Queue<PromptData> promptQueue = new Queue<PromptData>();
        private bool isProcessingQueue = false;

        private struct PromptData
        {
            public string message;
            public float? duration;
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning("PromptManager: Multiple instances detected. Destroying duplicate.");
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            
            if (vrPromptPrefab != null)
            {
                GameObject promptObj = Instantiate(vrPromptPrefab, transform);
                defaultPrompt = promptObj.GetComponent<VRPrompt>();
                
                if (defaultPrompt == null)
                {
                    Debug.LogError("PromptManager: VRPrompt prefab does not have a VRPrompt component!");
                }
            }
            else
            {
                Debug.LogError("PromptManager: No VRPrompt prefab assigned!");
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
                promptQueue.Enqueue(new PromptData { message = message, duration = duration });
                
                if (!isProcessingQueue)
                {
                    ProcessQueueAsync().Forget();
                }
            }
            else
            {
                defaultPrompt.ShowPrompt(message, duration);
            }
        }

        private async UniTaskVoid ProcessQueueAsync()
        {
            isProcessingQueue = true;

            while (promptQueue.Count > 0)
            {
                var promptData = promptQueue.Dequeue();
                
                await defaultPrompt.ShowPromptAsync(promptData.message, promptData.duration);
                
                if (promptData.duration.HasValue)
                {
                }
                else
                {
                    await UniTask.WaitWhile(() => defaultPrompt.IsShowing());
                }
                
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
    }
}
