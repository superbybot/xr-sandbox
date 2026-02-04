using App.Shared.Scripts.Interfaces;
using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Events;

namespace App.Shared.Scripts.Meta
{
    /// <summary>
    /// Meta SDK-based joystick using Grabbable component.
    /// Tracks hand position to calculate joystick tilt angles.
    /// Implements IVehicleInput for generic car control.
    /// </summary>
    [RequireComponent(typeof(Grabbable))]
    public class MetaJoystick : MonoBehaviour, IVehicleInput
    {
        [Header("Movement Settings")]
        [SerializeField] 
        [Tooltip("Maximum angle the joystick can tilt")]
        [Range(1.0f, 90.0f)]
        private float _maxAngle = 30.0f;
        
        [Header("Return to Origin")]
        [SerializeField] private bool _returnToOriginalPosition = true;
        [SerializeField] private float _returnSpeed = 5f;
        
        [Header("References")]
        [SerializeField] private Transform _handleTransform;
        
        [Header("Events")]
        public UnityEvent<Vector2> OnJoystickMoved;
        
        private Grabbable _grabbable;
        private Quaternion _originalRotation;
        private bool _isGrabbed = false;
        
        /// <summary>
        /// Current normalized joystick value (x, y) from -1 to 1.
        /// </summary>
        public Vector2 Value { get; private set; }

        // IVehicleInput Implementation
        public float Steering => Value.x;
        public float Throttle
        {
            get
            {
                // Logic mimics generic Single-Stick Car Control:
                // > 0: Forward Throttle
                // < -0.9: Reverse Throttle (Negative)
                // 0 to -0.9: Deadzone for Throttle (Braking range)
                if (Value.y > 0) return Value.y;
                if (Value.y < -0.9f) return Value.y;
                return 0f;
            }
        }

        public float Brake
        {
            get
            {
                // Braking range: 0 to -0.9
                if (Value.y <= 0 && Value.y >= -0.9f) return Mathf.Abs(Value.y);
                return 0f;
            }
        }

        private void Awake()
        {
            _grabbable = GetComponent<Grabbable>();
            
            if (_handleTransform == null)
                _handleTransform = transform;
                
            _originalRotation = _handleTransform.localRotation;
        }
        
        private void OnEnable()
        {
            if (_grabbable != null)
            {
                _grabbable.WhenPointerEventRaised += OnPointerEvent;
            }
        }
        
        private void OnDisable()
        {
            if (_grabbable != null)
            {
                _grabbable.WhenPointerEventRaised -= OnPointerEvent;
            }
        }
        
        private void OnPointerEvent(PointerEvent evt)
        {
            switch (evt.Type)
            {
                case PointerEventType.Select:
                    _isGrabbed = true;
                    break;
                case PointerEventType.Unselect:
                    _isGrabbed = false;
                    break;
            }
        }
        
        private void Update()
        {
            if (_isGrabbed && _grabbable.SelectingPointsCount > 0)
            {
                UpdateJoystickRotation();
            }
            else if (_returnToOriginalPosition && !_isGrabbed)
            {
                ReturnToOriginalRotation();
            }
        }
        
        private void UpdateJoystickRotation()
        {
            // Get the grabbing hand position
            var grabPoint = _grabbable.GrabPoints[0];
            if (grabPoint == null) return;
            
            Vector3 direction = grabPoint.position - _handleTransform.position;
            direction = transform.InverseTransformDirection(direction);
            
            // Limit Y movement if needed (optional)
            // direction.y = Mathf.Clamp(direction.y, 0.01f, 1.0f);
            
            direction = direction.normalized;
            
            // Calculate angles
            // Note: Implementation specific to how the joystick is rigged (vertical vs horizontal)
            // Assuming Z is forward, Y is up, X is right
            
            float upDownAngle = Mathf.Atan2(direction.z, direction.y) * Mathf.Rad2Deg;
            float leftRightAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            
            upDownAngle = Mathf.Clamp(upDownAngle, -_maxAngle, _maxAngle);
            leftRightAngle = Mathf.Clamp(leftRightAngle, -_maxAngle, _maxAngle);
            
            SetHandleRotation(leftRightAngle, upDownAngle);
            
            Vector2 normalizedValues = new Vector2(
                leftRightAngle / _maxAngle,
                upDownAngle / _maxAngle
            );
            
            Value = normalizedValues;
            OnJoystickMoved?.Invoke(normalizedValues);
        }
        
        private void SetHandleRotation(float xAngle, float zAngle)
        {
            if (_handleTransform == null) return;
            
            Quaternion xRotation = Quaternion.Euler(0, 0, -xAngle);
            Quaternion zRotation = Quaternion.Euler(zAngle, 0, 0);
            
            _handleTransform.localRotation = _originalRotation * xRotation * zRotation;
        }
        
        private void ReturnToOriginalRotation()
        {
            _handleTransform.localRotation = Quaternion.Lerp(
                _handleTransform.localRotation,
                _originalRotation,
                Time.deltaTime * _returnSpeed
            );
            
            // Smoothly return value to zero
            Value = Vector2.Lerp(Value, Vector2.zero, Time.deltaTime * _returnSpeed);
            
            if (Quaternion.Angle(_handleTransform.localRotation, _originalRotation) < 0.1f)
            {
                _handleTransform.localRotation = _originalRotation;
                Value = Vector2.zero;
            }
        }
    }
}
