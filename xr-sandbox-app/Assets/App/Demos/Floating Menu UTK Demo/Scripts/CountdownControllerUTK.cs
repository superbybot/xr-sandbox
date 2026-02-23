using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

namespace FloatingMenuDemo
{
    public class CountdownControllerUTK : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private float displayScale = 1.0f;
        [SerializeField] private float distanceFromHead = 2.0f;
        [SerializeField] private float fadeOutDuration = 0.5f;

        private VisualElement root;
        private Label countdownLabel;
        private Coroutine countdownCoroutine;
        private Transform mainCameraTransform;

        private void Awake()
        {
            if (uiDocument != null)
            {
                root = uiDocument.rootVisualElement;
                if (root != null)
                {
                    countdownLabel = root.Q<Label>("countdown-label");
                    root.style.display = DisplayStyle.None;
                    root.style.opacity = 0f;
                }
            }

            if (Camera.main != null)
            {
                mainCameraTransform = Camera.main.transform;
            }
        }

        public void StartCountdown()
        {
            if (countdownCoroutine != null) StopCoroutine(countdownCoroutine);
            countdownCoroutine = StartCoroutine(CountdownSequence());
        }

        private IEnumerator CountdownSequence()
        {
            if (root == null || countdownLabel == null) yield break;

            // Position at front of player
            if (mainCameraTransform != null)
            {
                transform.position = mainCameraTransform.position + (mainCameraTransform.forward * distanceFromHead);
                transform.rotation = Quaternion.LookRotation(transform.position - mainCameraTransform.position);
            }

            root.style.display = DisplayStyle.Flex;
            root.style.opacity = 1f;

            // 3
            UpdateCount("3", "count-3");
            yield return new WaitForSeconds(1f);

            // 2
            UpdateCount("2", "count-2");
            yield return new WaitForSeconds(1f);

            // 1
            UpdateCount("1", "count-1");
            yield return new WaitForSeconds(1f);

            // GO!!!
            UpdateCount("GO!!!", "count-go");
            yield return new WaitForSeconds(1f);

            // Fade out
            float elapsed = 0f;
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                root.style.opacity = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
                yield return null;
            }

            root.style.display = DisplayStyle.None;
            root.style.opacity = 0f;
        }

        private void UpdateCount(string text, string className)
        {
            countdownLabel.text = text;
            countdownLabel.ClearClassList();
            countdownLabel.AddToClassList("countdown-text");
            countdownLabel.AddToClassList(className);
            
            // Pulse effect
            StartCoroutine(PulseLabel());
        }

        private IEnumerator PulseLabel()
        {
            float duration = 0.2f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float scale = Mathf.Lerp(1.5f, 1.0f, elapsed / duration);
                countdownLabel.style.scale = new StyleScale(new Vector2(scale, scale));
                yield return null;
            }
            countdownLabel.style.scale = new StyleScale(new Vector2(1.0f, 1.0f));
        }
    }
}
