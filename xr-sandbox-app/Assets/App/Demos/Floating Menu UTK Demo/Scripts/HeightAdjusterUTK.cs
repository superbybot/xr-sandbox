using UnityEngine;
using UnityEngine.UIElements;
using Unity.XR.CoreUtils;

namespace FloatingMenuDemo
{
    public class HeightAdjusterUTK : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private XROrigin xrOrigin;

        [Header("Settings")]
        [SerializeField] private float minHeight = 0.5f;
        [SerializeField] private float maxHeight = 2.5f;

        private Slider heightSlider;
        private Button confirmButton;
        private Label valueLabel;
        private float pendingHeight;

        private void OnEnable()
        {
            if (uiDocument == null)
            {
                Debug.LogError("[HeightAdjusterUTK] UIDocument not assigned!");
                return;
            }

            var root = uiDocument.rootVisualElement;
            heightSlider = root.Q<Slider>("height-slider");
            confirmButton = root.Q<Button>("confirm-button");
            valueLabel = root.Q<Label>("value-label");

            if (heightSlider != null)
            {
                float currentHeight = 1.36f;
                if (xrOrigin != null && xrOrigin.CameraFloorOffsetObject != null)
                {
                    currentHeight = xrOrigin.CameraFloorOffsetObject.transform.localPosition.y;
                }

                heightSlider.lowValue = Mathf.Min(minHeight, currentHeight);
                heightSlider.highValue = Mathf.Max(maxHeight, currentHeight);
                heightSlider.value = currentHeight;

                heightSlider.RegisterValueChangedCallback(OnHeightChanged);
                pendingHeight = heightSlider.value;
                UpdateUI(pendingHeight);
            }

            if (confirmButton != null)
            {
                confirmButton.clicked += ConfirmHeight;
            }
        }

        private void OnDisable()
        {
            if (heightSlider != null)
            {
                heightSlider.UnregisterValueChangedCallback(OnHeightChanged);
            }

            if (confirmButton != null)
            {
                confirmButton.clicked -= ConfirmHeight;
            }
        }

        private void OnHeightChanged(ChangeEvent<float> evt)
        {
            pendingHeight = evt.newValue;
            UpdateUI(pendingHeight);
        }

        public void ConfirmHeight()
        {
            if (xrOrigin != null && xrOrigin.CameraFloorOffsetObject != null)
            {
                Vector3 currentPos = xrOrigin.CameraFloorOffsetObject.transform.localPosition;
                xrOrigin.CameraFloorOffsetObject.transform.localPosition = new Vector3(currentPos.x, pendingHeight, currentPos.z);
                UpdateUI(pendingHeight);
            }
        }

        private void UpdateUI(float height)
        {
            if (valueLabel != null)
            {
                bool isConfirmed = false;
                if (xrOrigin != null && xrOrigin.CameraFloorOffsetObject != null)
                {
                    isConfirmed = Mathf.Abs(xrOrigin.CameraFloorOffsetObject.transform.localPosition.y - height) < 0.01f;
                }

                valueLabel.text = $"Height: {height:F2}m";
                valueLabel.style.color = isConfirmed ? Color.green : Color.yellow;
            }
        }
    }
}
