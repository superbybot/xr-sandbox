using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace App.Demos.DialogueDemo.Scripts
{
    /// <summary>
    /// Example script showing different ways to trigger VR prompts.
    /// Demonstrates collision triggers, interaction triggers, and timed triggers.
    /// </summary>
    public class PromptTrigger : MonoBehaviour
    {
        [Header("Trigger Type")]
        [SerializeField] private TriggerType triggerType = TriggerType.OnCollision;
        
        [Header("Prompt Settings")]
        [SerializeField] private string promptMessage = "Hello from VR Prompt!";
        [SerializeField] private float customDuration = 0f; // 0 = use default
        [SerializeField] private bool triggerOnce = true;
        
        [Header("Timed Trigger Settings")]
        [SerializeField] private float delayBeforeTrigger = 2f;
        [SerializeField] private bool repeatTimed = false;
        [SerializeField] private float repeatInterval = 5f;
        
        private bool hasTriggered = false;
        private float nextTriggerTime = 0f;

        public enum TriggerType
        {
            OnCollision,        // Trigger when player collides with this object
            OnInteraction,      // Trigger when player interacts (grab/select) with this object
            OnProximity,        // Trigger when player gets close
            OnStart,            // Trigger when scene starts
            Timed,              // Trigger after a delay
            Manual              // Only trigger via code
        }

        private void Start()
        {
            // Setup based on trigger type
            switch (triggerType)
            {
                case TriggerType.OnStart:
                    TriggerPrompt();
                    break;
                    
                case TriggerType.Timed:
                    nextTriggerTime = Time.time + delayBeforeTrigger;
                    break;
                    
                case TriggerType.OnInteraction:
                    SetupInteractionTrigger();
                    break;
            }
        }

        private void Update()
        {
            // Handle timed triggers
            if (triggerType == TriggerType.Timed)
            {
                if (Time.time >= nextTriggerTime)
                {
                    TriggerPrompt();
                    
                    if (repeatTimed)
                    {
                        nextTriggerTime = Time.time + repeatInterval;
                    }
                    else if (triggerOnce)
                    {
                        enabled = false; // Disable script after triggering once
                    }
                }
            }
            
            // Handle proximity triggers
            if (triggerType == TriggerType.OnProximity && !hasTriggered)
            {
                CheckProximity();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (triggerType == TriggerType.OnCollision)
            {
                // Check if it's the player or player's hands
                if (IsPlayer(other))
                {
                    TriggerPrompt();
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (triggerType == TriggerType.OnCollision)
            {
                if (IsPlayer(collision.collider))
                {
                    TriggerPrompt();
                }
            }
        }

        private void SetupInteractionTrigger()
        {
            // Try to get XR interaction components
            var interactable = GetComponent<XRBaseInteractable>();
            if (interactable != null)
            {
                interactable.selectEntered.AddListener(OnXRInteraction);
            }
            else
            {
                Debug.LogWarning($"PromptTrigger on {gameObject.name}: OnInteraction trigger type requires an XRBaseInteractable component!");
            }
        }

        private void OnXRInteraction(SelectEnterEventArgs args)
        {
            TriggerPrompt();
        }

        private void CheckProximity()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null) return;
            
            float distance = Vector3.Distance(transform.position, mainCamera.transform.position);
            
            // Default proximity distance (can be made configurable)
            float proximityDistance = 2f;
            
            if (distance < proximityDistance)
            {
                TriggerPrompt();
            }
        }

        private bool IsPlayer(Collider collider)
        {
            // Check if collider belongs to player
            // This can be customized based on your player setup
            return collider.CompareTag("Player") || 
                   collider.GetComponentInParent<Camera>() != null ||
                   collider.name.Contains("Hand") ||
                   collider.name.Contains("XR");
        }

        /// <summary>
        /// Trigger the prompt (can be called from other scripts or Unity Events)
        /// </summary>
        public void TriggerPrompt()
        {
            if (triggerOnce && hasTriggered)
                return;
            
            hasTriggered = true;
            
            // Show the prompt
            if (customDuration > 0)
            {
                PromptManager.ShowPrompt(promptMessage, customDuration);
            }
            else
            {
                PromptManager.ShowPrompt(promptMessage);
            }
            
            Debug.Log($"PromptTrigger: Triggered prompt '{promptMessage}'");
        }

        /// <summary>
        /// Reset the trigger so it can be triggered again
        /// </summary>
        public void ResetTrigger()
        {
            hasTriggered = false;
        }

        private void OnDestroy()
        {
            // Clean up event listeners
            var interactable = GetComponent<XRBaseInteractable>();
            if (interactable != null)
            {
                interactable.selectEntered.RemoveListener(OnXRInteraction);
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Draw proximity radius for proximity triggers
            if (triggerType == TriggerType.OnProximity)
            {
                Gizmos.color = new Color(0, 1, 0, 0.3f);
                Gizmos.DrawWireSphere(transform.position, 2f); // Default proximity distance
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Trigger Prompt Now")]
        private void TriggerPromptNow()
        {
            TriggerPrompt();
        }
#endif
    }
}
