using UnityEngine;
using UnityEngine.UI;
using Unity.XR.CoreUtils;

namespace FloatingMenuDemo
{
    public class HeightAdjuster : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private XROrigin xrOrigin;
        [SerializeField] private Slider heightSlider;
        [SerializeField] private TMPro.TMP_Text valueText;

        [Header("Settings")]
        [SerializeField] private float minHeight = 0.5f;
        [SerializeField] private float maxHeight = 2.5f;

        private void Start()
        {
            Debug.Log("[HeightAdjuster] Start called.");
            if (heightSlider != null)
            {
                heightSlider.minValue = minHeight;
                heightSlider.maxValue = maxHeight;
                
                // Initialize slider value based on current height or default
                if (xrOrigin != null)
                {
                    float currentHeight = xrOrigin.CameraYOffset;
                    heightSlider.value = Mathf.Clamp(currentHeight, minHeight, maxHeight);
                }
                else
                {
                    heightSlider.value = 1.36f; // Default average height
                }

                heightSlider.onValueChanged.AddListener(OnHeightChanged);
                Debug.Log($"[HeightAdjuster] Slider initialized. Min: {minHeight}, Max: {maxHeight}, Current: {heightSlider.value}");
                OnHeightChanged(heightSlider.value); // Update text
            }
            else
            {
                Debug.LogError("[HeightAdjuster] Height Slider not assigned!");
            }
        }

        private void OnHeightChanged(float newHeight)
        {
            if (xrOrigin != null)
            {
                // Update the property (for consistency)
                xrOrigin.CameraYOffset = newHeight;

                // Force manual position update to ensure visual change
                // This overrides any potential XROrigin logic that might be ignoring the offset
                if (xrOrigin.CameraFloorOffsetObject != null)
                {
                    Vector3 currentPos = xrOrigin.CameraFloorOffsetObject.transform.localPosition;
                    xrOrigin.CameraFloorOffsetObject.transform.localPosition = new Vector3(currentPos.x, newHeight, currentPos.z);
                    Debug.Log($"[HeightAdjuster] Forced CameraFloorOffsetObject Local Y to: {newHeight}");
                }
            }
            else
            {
                Debug.LogError("[HeightAdjuster] XROrigin NOT assigned!");
            }

            if (valueText != null)
            {
                valueText.text = $"Height: {newHeight:F2}m";
            }
        }

        private void OnDestroy()
        {
            if (heightSlider != null)
            {
                heightSlider.onValueChanged.RemoveListener(OnHeightChanged);
            }
        }
    }
}
