using UnityEngine;
using Oculus.Interaction;

namespace App.Shared.Scripts.Meta
{
    /// <summary>
    /// Meta SDK replacement for PromptTrigger.
    /// Uses PointableUnityEventWrapper instead of XRBaseInteractable.
    /// triggers prompts based on Collision, Interaction (Poke/Select), Proximity, or Time.
    /// </summary>
    public class MetaPromptTrigger : MonoBehaviour
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
        
        [Header("Interaction Settings")]
        [Tooltip("Required for OnInteraction type. Listens for Meta SDK events.")]
        [SerializeField] private PointableUnityEventWrapper _eventWrapper;

        private bool hasTriggered = false;
        private float nextTriggerTime = 0f;

        public enum TriggerType
        {
            OnCollision,
            OnInteraction,
            OnProximity,
            OnStart,
            Timed,
            Manual
        }

        private void Start()
        {
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
                        enabled = false;
                    }
                }
            }
            
            if (triggerType == TriggerType.OnProximity && !hasTriggered)
            {
                CheckProximity();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (triggerType == TriggerType.OnCollision)
            {
                // Simple check for Player presence (Headset or Controllers)
                if (IsPlayer(other))
                {
                    TriggerPrompt();
                }
            }
        }

        private void SetupInteractionTrigger()
        {
            if (_eventWrapper == null)
                _eventWrapper = GetComponent<PointableUnityEventWrapper>();

            if (_eventWrapper != null)
            {
                _eventWrapper.WhenSelect.AddListener(OnMetaInteraction);
            }
            else
            {
                Debug.LogWarning($"MetaPromptTrigger on {gameObject.name}: OnInteraction type requires a PointableUnityEventWrapper!", this);
            }
        }

        private void OnMetaInteraction(PointerEvent evt)
        {
            TriggerPrompt();
        }

        private void CheckProximity()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null) return;
            
            float distance = Vector3.Distance(transform.position, mainCamera.transform.position);
            float proximityDistance = 2f;
            
            if (distance < proximityDistance)
            {
                TriggerPrompt();
            }
        }

        private bool IsPlayer(Collider collider)
        {
            // OVRCameraRig parts usually tagged Player or named appropriately
            return collider.CompareTag("Player") || 
                   collider.name.Contains("OVR") || 
                   collider.name.Contains("Hand") ||
                   collider.name.Contains("Controller");
        }

        public void TriggerPrompt()
        {
            if (triggerOnce && hasTriggered)
                return;
            
            hasTriggered = true;
            
            var promptMgr = FindObjectOfType<App.Demos.DialogueDemo.Scripts.VRPrompt>();
            if (promptMgr != null)
            {
                // Logic adjusted to match PromptManager overloads
                if (customDuration > 0)
                {
                    App.Demos.DialogueDemo.Scripts.PromptManager.ShowPrompt(promptMessage, customDuration);
                }
                else
                {
                    App.Demos.DialogueDemo.Scripts.PromptManager.ShowPrompt(promptMessage);
                }
            }
            
            Debug.Log($"MetaPromptTrigger: Triggered prompt '{promptMessage}'");
        }

        private void OnDestroy()
        {
            if (_eventWrapper != null)
            {
                _eventWrapper.WhenSelect.RemoveListener(OnMetaInteraction);
            }
        }
    }
}
