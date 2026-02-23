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
        [SerializeField] private Button confirmButton;
        [SerializeField] private TMPro.TMP_Text valueText;

        [Header("Settings")]
        [SerializeField] private float minHeight = 0.5f;
        [SerializeField] private float maxHeight = 2.5f;

        private float pendingHeight;

        private void Start()
        {
            Debug.Log("[HeightAdjuster] Start called.");
            if (heightSlider != null)
            {
                float currentHeight = 1.36f;
                if (xrOrigin != null && xrOrigin.CameraFloorOffsetObject != null)
                {
                    currentHeight = xrOrigin.CameraFloorOffsetObject.transform.localPosition.y;
                }

                // Adjust range if current height is outside bounds to avoid instant mismatch
                float effectiveMin = Mathf.Min(minHeight, currentHeight);
                float effectiveMax = Mathf.Max(maxHeight, currentHeight);

                heightSlider.minValue = effectiveMin;
                heightSlider.maxValue = effectiveMax;
                heightSlider.value = currentHeight;

                heightSlider.onValueChanged.AddListener(OnHeightChanged);
                
                if (confirmButton != null)
                {
                    confirmButton.onClick.AddListener(ConfirmHeight);
                }

                pendingHeight = heightSlider.value;
                Debug.Log($"[HeightAdjuster] Slider initialized. Min: {minHeight}, Max: {maxHeight}, Current: {pendingHeight}");
                UpdateUI(pendingHeight);
            }
            else
            {
                Debug.LogError("[HeightAdjuster] Height Slider not assigned!");
            }
        }

        private void OnHeightChanged(float newHeight)
        {
            pendingHeight = newHeight;
            UpdateUI(newHeight);
        }

        public void ConfirmHeight()
        {
            if (xrOrigin != null && xrOrigin.CameraFloorOffsetObject != null)
            {
                Vector3 currentPos = xrOrigin.CameraFloorOffsetObject.transform.localPosition;
                xrOrigin.CameraFloorOffsetObject.transform.localPosition = new Vector3(currentPos.x, pendingHeight, currentPos.z);
                
                Debug.Log($"[HeightAdjuster] Height CONFIRMED. Set CameraFloorOffsetObject Local Y to: {pendingHeight}");
                UpdateUI(pendingHeight);
            }
            else
            {
                Debug.LogError("[HeightAdjuster] XROrigin or CameraFloorOffsetObject NOT assigned!");
            }
        }

        private void UpdateUI(float height)
        {
            if (valueText != null)
            {
                bool isConfirmed = false;
                if (xrOrigin != null && xrOrigin.CameraFloorOffsetObject != null)
                {
                    isConfirmed = Mathf.Abs(xrOrigin.CameraFloorOffsetObject.transform.localPosition.y - height) < 0.01f;
                }

                valueText.text = $"Height: {height:F2}m";
                valueText.color = isConfirmed ? Color.green : Color.yellow;
            }
        }

        private void OnDestroy()
        {
            if (heightSlider != null)
            {
                heightSlider.onValueChanged.RemoveListener(OnHeightChanged);
            }

            if (confirmButton != null)
            {
                confirmButton.onClick.RemoveListener(ConfirmHeight);
            }
        }
    }
}
