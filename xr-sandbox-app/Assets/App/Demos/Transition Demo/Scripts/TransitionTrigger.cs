using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace App.Demos.TransitionDemo.Scripts
{
    /// <summary>
    /// Triggers screen fade transitions based on various events.
    /// Similar to PromptTrigger from Dialogue Demo, but for fade transitions.
    /// </summary>
    public class TransitionTrigger : MonoBehaviour
    {
        [Header("Trigger Type")]
        [SerializeField] private TriggerType triggerType = TriggerType.OnCollision;
        
        [Header("Fade Settings")]
        [SerializeField] private FadeType fadeType = FadeType.FadeOutAndIn;
        [SerializeField] private float fadeDuration = 1.0f;
        [SerializeField] private float holdDuration = 0.1f; // For FadeOutAndIn
        [SerializeField] private bool triggerOnce = true;
        
        [Header("Timed Trigger Settings")]
        [SerializeField] private float delayBeforeTrigger = 2f;
        [SerializeField] private bool repeatTimed = false;
        [SerializeField] private float repeatInterval = 5f;
        
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

        public enum FadeType
        {
            FadeOut,      // Fade to black
            FadeIn,       // Fade to clear
            FadeOutAndIn, // Fade out, hold, fade in
            FadeToWhite   // Fade to white (flashbang effect)
        }

        private void Start()
        {
            switch (triggerType)
            {
                case TriggerType.OnStart:
                    TriggerFade();
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
                    TriggerFade();
                    
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
            if (triggerType == TriggerType.OnCollision || triggerType == TriggerType.OnProximity)
            {
                if (IsPlayer(other))
                {
                    TriggerFade();
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (triggerType == TriggerType.OnCollision)
            {
                if (IsPlayer(collision.collider))
                {
                    TriggerFade();
                }
            }
        }

        private void SetupInteractionTrigger()
        {
            var interactable = GetComponent<XRBaseInteractable>();
            if (interactable != null)
            {
                interactable.selectEntered.AddListener(OnXRInteraction);
            }
            else
            {
                Debug.LogWarning($"TransitionTrigger on {gameObject.name}: OnInteraction trigger type requires an XRBaseInteractable component!");
            }
        }

        private void OnXRInteraction(SelectEnterEventArgs args)
        {
            TriggerFade();
        }

        private void CheckProximity()
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null) return;
            
            float distance = Vector3.Distance(transform.position, mainCamera.transform.position);
            float proximityDistance = 2f;
            
            if (distance < proximityDistance)
            {
                TriggerFade();
            }
        }

        private bool IsPlayer(Collider collider)
        {
            return collider.CompareTag("Player") || 
                   collider.GetComponentInParent<Camera>() != null ||
                   collider.name.Contains("Hand") ||
                   collider.name.Contains("XR");
        }

        /// <summary>
        /// Trigger the fade transition (can be called from other scripts or Unity Events)
        /// </summary>
        public void TriggerFade()
        {
            if (triggerOnce && hasTriggered)
                return;
            
            hasTriggered = true;
            
            if (ScreenFader.Instance == null)
            {
                Debug.LogError("TransitionTrigger: ScreenFader.Instance is null! Make sure ScreenFadeVolume is in the scene.");
                return;
            }
            
            // Execute the appropriate fade type
            switch (fadeType)
            {
                case FadeType.FadeOut:
                    ScreenFader.Instance.FadeToBlack(fadeDuration);
                    Debug.Log($"TransitionTrigger: Triggered FadeOut over {fadeDuration}s");
                    break;
                    
                case FadeType.FadeIn:
                    ScreenFader.Instance.FadeToClear(fadeDuration);
                    Debug.Log($"TransitionTrigger: Triggered FadeIn over {fadeDuration}s");
                    break;
                    
                case FadeType.FadeOutAndIn:
                    ScreenFader.Instance.FadeOutAndIn(fadeDuration, holdDuration, fadeDuration);
                    Debug.Log($"TransitionTrigger: Triggered FadeOutAndIn - Out:{fadeDuration}s Hold:{holdDuration}s In:{fadeDuration}s");
                    break;
                    
                case FadeType.FadeToWhite:
                    ScreenFader.Instance.FadeToWhite(fadeDuration);
                    Debug.Log($"TransitionTrigger: Triggered FadeToWhite over {fadeDuration}s");
                    break;
            }
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
            var interactable = GetComponent<XRBaseInteractable>();
            if (interactable != null)
            {
                interactable.selectEntered.RemoveListener(OnXRInteraction);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (triggerType == TriggerType.OnProximity)
            {
                Gizmos.color = new Color(1, 0.5f, 0, 0.3f);
                Gizmos.DrawWireSphere(transform.position, 2f);
            }
        }
    }
}
