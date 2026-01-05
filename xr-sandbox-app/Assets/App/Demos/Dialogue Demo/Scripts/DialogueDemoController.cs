using Cysharp.Threading.Tasks;
using UnityEngine;

namespace App.Demos.DialogueDemo.Scripts
{
    /// <summary>
    /// Demo controller showcasing programmatic usage of the VR Prompt system.
    /// Demonstrates welcome messages, sequential tutorials, and various prompt scenarios.
    /// </summary>
    public class DialogueDemoController : MonoBehaviour
    {
        [Header("Welcome Settings")]
        [SerializeField] private bool showWelcomeOnStart = true;
        [SerializeField] private float welcomeDelay = 1f;
        
        [Header("Tutorial Settings")]
        [SerializeField] private bool autoStartTutorial = false;
        [SerializeField] private float tutorialStartDelay = 10f;
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;

        private void Start()
        {
            if (showWelcomeOnStart)
            {
                ShowWelcomeMessageAsync().Forget();
            }
            
            if (autoStartTutorial)
            {
                StartTutorialAsync().Forget();
            }
        }

        /// <summary>
        /// Show welcome message after a short delay
        /// </summary>
        private async UniTaskVoid ShowWelcomeMessageAsync()
        {
            await UniTask.WaitForSeconds(welcomeDelay);
            
            PromptManager.ShowPrompt("Welcome to the VR Dialogue Demo!", 4f);
            
            if (enableDebugLogs)
                Debug.Log("DialogueDemoController: Welcome message displayed");
        }

        /// <summary>
        /// Start a sequential tutorial demonstrating the queue system
        /// </summary>
        private async UniTaskVoid StartTutorialAsync()
        {
            await UniTask.WaitForSeconds(tutorialStartDelay);
            
            if (enableDebugLogs)
                Debug.Log("DialogueDemoController: Starting tutorial sequence");
            
            // These will queue and display one after another
            PromptManager.ShowPrompt("Tutorial Step 1: Look around the environment", 3f);
            PromptManager.ShowPrompt("Tutorial Step 2: Find the colored cubes", 3f);
            PromptManager.ShowPrompt("Tutorial Step 3: Try touching or grabbing them", 3f);
            PromptManager.ShowPrompt("Tutorial Step 4: Walk near the proximity zone", 3f);
            PromptManager.ShowPrompt("Tutorial Complete! Explore freely.", 4f);
        }

        /// <summary>
        /// Example: Show a contextual prompt based on player action
        /// Call this from other scripts or Unity Events
        /// </summary>
        public void OnPlayerCompletedAction(string actionName)
        {
            PromptManager.ShowPrompt($"Great! You completed: {actionName}", 2.5f);
            
            if (enableDebugLogs)
                Debug.Log($"DialogueDemoController: Action completed - {actionName}");
        }

        /// <summary>
        /// Example: Show a warning prompt
        /// </summary>
        public void ShowWarning(string warningMessage)
        {
            PromptManager.ShowPrompt($"⚠️ {warningMessage}", 3f);
            
            if (enableDebugLogs)
                Debug.Log($"DialogueDemoController: Warning - {warningMessage}");
        }

        /// <summary>
        /// Example: Show an achievement/success prompt
        /// </summary>
        public void ShowAchievement(string achievementName)
        {
            PromptManager.ShowPrompt($"🎉 Achievement: {achievementName}", 4f);
            
            if (enableDebugLogs)
                Debug.Log($"DialogueDemoController: Achievement - {achievementName}");
        }

        /// <summary>
        /// Clear all queued prompts (emergency stop)
        /// </summary>
        public void ClearAllPrompts()
        {
            PromptManager.ClearQueue();
            PromptManager.HidePrompt();
            
            if (enableDebugLogs)
                Debug.Log("DialogueDemoController: All prompts cleared");
        }

        /// <summary>
        /// Check if a prompt is currently showing
        /// </summary>
        public bool IsPromptActive()
        {
            return PromptManager.IsShowingPrompt();
        }

#if UNITY_EDITOR
        [ContextMenu("Test Welcome Message")]
        private void TestWelcome()
        {
            ShowWelcomeMessageAsync().Forget();
        }

        [ContextMenu("Test Tutorial Sequence")]
        private void TestTutorial()
        {
            StartTutorialAsync().Forget();
        }

        [ContextMenu("Test Action Complete")]
        private void TestActionComplete()
        {
            OnPlayerCompletedAction("Test Action");
        }

        [ContextMenu("Test Warning")]
        private void TestWarning()
        {
            ShowWarning("This is a test warning!");
        }

        [ContextMenu("Test Achievement")]
        private void TestAchievement()
        {
            ShowAchievement("First Steps");
        }

        [ContextMenu("Clear All Prompts")]
        private void TestClearAll()
        {
            ClearAllPrompts();
        }
#endif
    }
}
