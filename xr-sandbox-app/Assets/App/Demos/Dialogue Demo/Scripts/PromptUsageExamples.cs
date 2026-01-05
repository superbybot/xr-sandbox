using App.Demos.DialogueDemo.Scripts;
using UnityEngine;

namespace App.Demos.DialogueDemo.Examples
{
    /// <summary>
    /// Example script showing various ways to use the VR Prompt system.
    /// Attach this to any GameObject to test different prompt scenarios.
    /// </summary>
    public class PromptUsageExamples : MonoBehaviour
    {
        [Header("Test Buttons (Inspector)")]
        [SerializeField] private bool testBasicPrompt = false;
        [SerializeField] private bool testCustomDuration = false;
        [SerializeField] private bool testSequence = false;
        [SerializeField] private bool testClear = false;

        private void Update()
        {
            // Inspector test buttons
            if (testBasicPrompt)
            {
                testBasicPrompt = false;
                Example_BasicPrompt();
            }

            if (testCustomDuration)
            {
                testCustomDuration = false;
                Example_CustomDuration();
            }

            if (testSequence)
            {
                testSequence = false;
                Example_SequentialPrompts();
            }

            if (testClear)
            {
                testClear = false;
                Example_ClearPrompts();
            }
        }

        // ==================== BASIC USAGE ====================

        /// <summary>
        /// Example 1: Show a simple prompt with default duration (3 seconds)
        /// </summary>
        public void Example_BasicPrompt()
        {
            PromptManager.ShowPrompt("This is a basic VR prompt!");
        }

        /// <summary>
        /// Example 2: Show a prompt with custom duration
        /// </summary>
        public void Example_CustomDuration()
        {
            PromptManager.ShowPrompt("This prompt shows for 5 seconds", 5f);
        }

        /// <summary>
        /// Example 3: Hide the current prompt immediately
        /// </summary>
        public void Example_HidePrompt()
        {
            PromptManager.HidePrompt();
        }

        // ==================== SEQUENTIAL PROMPTS ====================

        /// <summary>
        /// Example 4: Show multiple prompts in sequence (queue system)
        /// </summary>
        public void Example_SequentialPrompts()
        {
            PromptManager.ShowPrompt("First prompt", 2f);
            PromptManager.ShowPrompt("Second prompt", 2f);
            PromptManager.ShowPrompt("Third prompt", 2f);
            // These will display one after another automatically
        }

        /// <summary>
        /// Example 5: Tutorial sequence
        /// </summary>
        public void Example_Tutorial()
        {
            PromptManager.ShowPrompt("Welcome to the tutorial!", 3f);
            PromptManager.ShowPrompt("Step 1: Look around", 3f);
            PromptManager.ShowPrompt("Step 2: Use your controllers", 3f);
            PromptManager.ShowPrompt("Step 3: Grab objects", 3f);
            PromptManager.ShowPrompt("Tutorial complete!", 4f);
        }

        // ==================== GAME EVENTS ====================

        /// <summary>
        /// Example 6: Show prompt when player enters an area
        /// </summary>
        public void OnPlayerEnterDangerZone()
        {
            PromptManager.ShowPrompt("⚠️ Warning: Danger Zone!", 3f);
        }

        /// <summary>
        /// Example 7: Show prompt when player picks up an item
        /// </summary>
        public void OnItemPickup(string itemName)
        {
            PromptManager.ShowPrompt($"Picked up: {itemName}", 2f);
        }

        /// <summary>
        /// Example 8: Show prompt when player completes an objective
        /// </summary>
        public void OnObjectiveComplete(string objectiveName)
        {
            PromptManager.ShowPrompt($"✓ Objective Complete: {objectiveName}", 4f);
        }

        /// <summary>
        /// Example 9: Show achievement notification
        /// </summary>
        public void OnAchievementUnlocked(string achievementName)
        {
            PromptManager.ShowPrompt($"🎉 Achievement Unlocked: {achievementName}", 5f);
        }

        // ==================== CONTEXTUAL HINTS ====================

        /// <summary>
        /// Example 10: Show hint when player looks at an object
        /// </summary>
        public void OnLookAtInteractable(string objectName)
        {
            PromptManager.ShowPrompt($"Press trigger to interact with {objectName}", 2.5f);
        }

        /// <summary>
        /// Example 11: Show hint when player is stuck
        /// </summary>
        public void OnPlayerStuck()
        {
            PromptManager.ShowPrompt("Hint: Try looking for the blue lever", 4f);
        }

        /// <summary>
        /// Example 12: Show error message
        /// </summary>
        public void OnInvalidAction()
        {
            PromptManager.ShowPrompt("❌ Cannot do that here", 2f);
        }

        // ==================== QUEUE MANAGEMENT ====================

        /// <summary>
        /// Example 13: Clear all queued prompts
        /// </summary>
        public void Example_ClearPrompts()
        {
            PromptManager.ClearQueue();
            PromptManager.HidePrompt();
            Debug.Log("All prompts cleared");
        }

        /// <summary>
        /// Example 14: Check if a prompt is currently showing
        /// </summary>
        public void Example_CheckIfShowing()
        {
            bool isShowing = PromptManager.IsShowingPrompt();
            Debug.Log($"Prompt is showing: {isShowing}");
        }

        // ==================== ADVANCED USAGE ====================

        /// <summary>
        /// Example 15: Conditional prompt based on game state
        /// </summary>
        public void Example_ConditionalPrompt(int playerHealth)
        {
            if (playerHealth < 20)
            {
                PromptManager.ShowPrompt("⚠️ Health Critical! Find a health pack.", 3f);
            }
            else if (playerHealth < 50)
            {
                PromptManager.ShowPrompt("Health low. Be careful!", 2f);
            }
        }

        /// <summary>
        /// Example 16: Timed reminder
        /// </summary>
        public void Example_TimedReminder()
        {
            Invoke(nameof(ShowReminder), 30f); // Show after 30 seconds
        }

        private void ShowReminder()
        {
            PromptManager.ShowPrompt("Don't forget to save your progress!", 3f);
        }

        /// <summary>
        /// Example 17: Dynamic prompt with variable
        /// </summary>
        public void Example_DynamicPrompt(int score, int coinsCollected)
        {
            PromptManager.ShowPrompt($"Score: {score} | Coins: {coinsCollected}", 2f);
        }

        // ==================== UNITY EVENTS ====================

        /// <summary>
        /// Example 18: Can be called from Unity Events (buttons, triggers, etc.)
        /// </summary>
        public void OnButtonPressed()
        {
            PromptManager.ShowPrompt("Button pressed!", 1.5f);
        }

        /// <summary>
        /// Example 19: Collision trigger
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PromptManager.ShowPrompt("You entered the trigger zone", 2f);
            }
        }

        /// <summary>
        /// Example 20: Scene start message
        /// </summary>
        private void Start()
        {
            // Uncomment to show welcome message on scene start
            // PromptManager.ShowPrompt("Welcome to the scene!", 3f);
        }

#if UNITY_EDITOR
        // ==================== EDITOR TESTING ====================

        [ContextMenu("Test All Examples")]
        private void TestAllExamples()
        {
            Example_BasicPrompt();
            Invoke(nameof(Example_CustomDuration), 3.5f);
            Invoke(nameof(Example_SequentialPrompts), 9f);
        }

        [ContextMenu("Test Tutorial")]
        private void TestTutorial()
        {
            Example_Tutorial();
        }

        [ContextMenu("Test Game Events")]
        private void TestGameEvents()
        {
            OnPlayerEnterDangerZone();
            Invoke(() => OnItemPickup("Health Potion"), 3.5f);
            Invoke(() => OnObjectiveComplete("Find the Key"), 7f);
            Invoke(() => OnAchievementUnlocked("First Steps"), 11f);
        }
#endif
    }
}
