using App.Shared.Scripts.Interfaces;
using Oculus.Interaction;
using UnityEngine;

namespace App.Shared.Scripts.Meta
{
    /// <summary>
    /// Adapts a Grabbable object with a OneGrabRotateTransformer to the IVehicleInput interface.
    /// This allows the wheel to control steering logic without the car knowing about Meta SDK.
    /// </summary>
    [RequireComponent(typeof(Grabbable))]
    [RequireComponent(typeof(OneGrabRotateTransformer))]
    public class MetaSteeringWheel : MonoBehaviour, IVehicleInput
    {
        [Header("Settings")]
        [Tooltip("The maximum rotation angle of the transformer in degrees (e.g., 450). Used to normalize output.")]
        [SerializeField] private float _maxRotationAngle = 450f;

        [Tooltip("Inverts the steering output value.")]
        [SerializeField] private bool _invertOutput = false;

        // References
        private OneGrabRotateTransformer _transformer;
        private Grabbable _grabbable;

        // Interface Implementation
        public float Steering { get; private set; }
        public float Throttle => 0f; // Wheel doesn't control throttle
        public float Brake => 0f;    // Wheel doesn't control brake

        private void Awake()
        {
            _grabbable = GetComponent<Grabbable>();
            _transformer = GetComponent<OneGrabRotateTransformer>();
            
            if (_transformer == null)
            {
                Debug.LogError("MetaSteeringWheel requires a OneGrabRotateTransformer component!", this);
            }
        }

        private void Update()
        {
            if (_transformer != null)
            {
                // Pivot rotation is managed by the transformer. We just read the local Z rotation.
                // Note: Ensure the transformer constraints match _maxRotationAngle.
                
                // Get signed angle relative to initial state (assuming 0 is neutral)
                float currentAngle = _transformer.Pivot.localEulerAngles.z;
                
                // Unity euler angles are 0-360. Convert to -180 to 180 for steering math.
                if (currentAngle > 180) currentAngle -= 360;

                // Normalize to -1 to 1 range
                float normalizedValue = Mathf.Clamp(currentAngle / _maxRotationAngle, -1f, 1f);

                if (_invertOutput)
                {
                    normalizedValue = -normalizedValue;
                }

                Steering = normalizedValue;
            }
        }
    }
}
