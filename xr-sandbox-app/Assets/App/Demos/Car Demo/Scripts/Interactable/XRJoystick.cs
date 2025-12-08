using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace App.Demos.Car_Demo.Scripts.Interactable
{
    public class XRJoystick : XRBaseInteractable
    {
        public enum MovementDirection
        {
            X_Axis,
            Z_Axis,
            Both_Axes
        }

        [Header("Custom Settings")]
        [SerializeField] private bool _onSelectByHover = false;

        [Header("Movement Settings")]
        [SerializeField] private MovementDirection _movementDirection = MovementDirection.Both_Axes;
        [SerializeField] 
        [Tooltip("Maximum angle the joystick can tilt")]
        [Range(1.0f, 90.0f)]
        private float _maxAngle = 30.0f;

        [Header("Return to Origin Parameters")]
        [SerializeField] private bool _returnToOriginalPosition = true;
        [SerializeField] private bool _roundToNearestPosition = false;
        [SerializeField] private float _returnSpeed = 5f;

        [Header("On Hover Parameters")]
        [SerializeField] private float _nearInteractionDistanceThreshold = 0.2f;

        [SerializeField] private Transform _handleTransform;
        public UnityEvent<Vector2> OnJoystickMoved;

        [Header("Debug Values")]
        [SerializeField, Tooltip("Current normalized joystick value (read-only)")] private Vector2 _currentValue;
        [SerializeField, Tooltip("Current up-down angle")] private float _currentUpDownAngle;
        [SerializeField, Tooltip("Current left-right angle")] private float _currentLeftRightAngle;
        [SerializeField, Tooltip("Current direction vector from interactor")] private Vector3 _currentDirection;
        [SerializeField, Tooltip("Snapped cardinal target values")] private Vector2 _snappedCardinalTarget;

        private IXRInteractor _interactor;
        private Quaternion _originalRotation;
        private Quaternion _targetRotation;
        private bool _isReturning = false;

        /// <summary>
        /// Current normalized joystick value (x, y).
        /// </summary>
        public Vector2 Value { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            _originalRotation = _handleTransform.localRotation;
        }

        public override bool IsSelectableBy(IXRSelectInteractor interactor)
        {
            if (_onSelectByHover)
            {
                return false;
            }

            if (interactor is NearFarInteractor nearFar)
            {
                Transform interactorAttachTransform = nearFar.GetAttachTransform(this);
                float distance = Vector3.Distance(interactorAttachTransform.position, this.transform.position);

                if (distance <= _nearInteractionDistanceThreshold)
                {
                    return base.IsSelectableBy(interactor);
                }
                else
                {
                    return false;
                }
            }

            return base.IsSelectableBy(interactor) && interactor is not XRRayInteractor;
        }

        public override bool IsHoverableBy(IXRHoverInteractor interactor)
        {

            if (interactor is NearFarInteractor nearFar)
            {
                Transform interactorAttachTransform = nearFar.GetAttachTransform(this);
                float distance = Vector3.Distance(interactorAttachTransform.position, this.transform.position);

                if (distance <= _nearInteractionDistanceThreshold)
                {
                    return base.IsHoverableBy(interactor);
                }
                else
                {
                    return false;
                }
            }

            return base.IsHoverableBy(interactor) && interactor is not XRRayInteractor;
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            if (_onSelectByHover)
            {
                return;
            }

            _interactor = args.interactorObject;
            _isReturning = false;
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            if (_onSelectByHover)
            {
                return;
            }

            if (_returnToOriginalPosition)
            {
                if (_roundToNearestPosition)
                {
                    SetNearestCardinalTarget();
                }
                _isReturning = true;
            }

            _interactor = null;
        }

        protected override void OnHoverEntered(HoverEnterEventArgs args)
        {
            base.OnHoverEntered(args);
            if (_onSelectByHover == false)
            {
                return;
            }

            _interactor = args.interactorObject;
            _isReturning = false;
        }

        protected override void OnHoverExited(HoverExitEventArgs args)
        {
            base.OnHoverExited(args);
            if (_onSelectByHover == false)
            {
                return;
            }

            if (_returnToOriginalPosition)
            {
                if (_roundToNearestPosition)
                {
                    SetNearestCardinalTarget();
                }
                _isReturning = true;
            }

            _interactor = null;
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractionStrength(updatePhase);

            switch (updatePhase)
            {
                case XRInteractionUpdateOrder.UpdatePhase.Dynamic:
                {
                    if (_isReturning && !IsSelected())
                    {
                        ReturnToOriginalRotation();
                    }
                    else if (IsSelected())
                    {
                        UpdateJoystickRotation();
                    }
                }
                    break;
            }
        }

        private bool IsSelected()
        {
            return (_onSelectByHover == false && isSelected) || (_onSelectByHover && isHovered);
        }

        private void UpdateJoystickRotation()
        {
            if (_interactor == null || _handleTransform == null) return;

            Vector3 direction = _interactor.GetAttachTransform(this).position - _handleTransform.position;
            direction = transform.InverseTransformDirection(direction);

            switch (_movementDirection)
            {
                case MovementDirection.X_Axis:
                    direction.z = 0;
                    break;
                case MovementDirection.Z_Axis:
                    direction.x = 0;
                    break;
                case MovementDirection.Both_Axes:
                    break;
            }

            direction.y = Mathf.Clamp(direction.y, 0.01f, 1.0f);
            direction = direction.normalized;

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
            _currentValue = normalizedValues;
            _currentUpDownAngle = upDownAngle;
            _currentLeftRightAngle = leftRightAngle;
            _currentDirection = direction;
            OnJoystickMoved?.Invoke(normalizedValues);
        }

        private void SetHandleRotation(float xAngle, float zAngle)
        {
            if (_handleTransform == null) return;

            Quaternion xRotation = Quaternion.Euler(0, 0, -xAngle);
            Quaternion zRotation = Quaternion.Euler(zAngle, 0, 0);

            _handleTransform.localRotation = _originalRotation * xRotation * zRotation;
        }

        private void SetNearestCardinalTarget()
        {
            var currentRotation = _handleTransform.localRotation * Quaternion.Inverse(_originalRotation);
            var euler = currentRotation.eulerAngles;

            float currentLeftRightAngle = Mathf.DeltaAngle(0, -euler.z);
            float currentUpDownAngle = Mathf.DeltaAngle(0, euler.x);

            float normalizedX = Mathf.Clamp(currentLeftRightAngle / _maxAngle, -1f, 1f);
            float normalizedZ = Mathf.Clamp(currentUpDownAngle / _maxAngle, -1f, 1f);

            float snappedX = Mathf.Round(normalizedX);
            float snappedZ = Mathf.Round(normalizedZ);

            snappedX = Mathf.Clamp(snappedX, -1f, 1f);
            snappedZ = Mathf.Clamp(snappedZ, -1f, 1f);

            _snappedCardinalTarget = new Vector2(snappedX, snappedZ);

            float targetXAngle = snappedX * _maxAngle;
            float targetZAngle = snappedZ * _maxAngle;

            Quaternion xRotation = Quaternion.Euler(0, 0, -targetXAngle);
            Quaternion zRotation = Quaternion.Euler(targetZAngle, 0, 0);
            _targetRotation = _originalRotation * xRotation * zRotation;
        }

        private void ReturnToOriginalRotation()
        {
            Quaternion targetRotation = _roundToNearestPosition ? _targetRotation : _originalRotation;

            _handleTransform.localRotation = Quaternion.Lerp(
                _handleTransform.localRotation,
                targetRotation,
                Time.deltaTime * _returnSpeed
            );

            if (Quaternion.Angle(_handleTransform.localRotation, targetRotation) < 0.1f)
            {
                _handleTransform.localRotation = targetRotation;
                _isReturning = false;
            }
        }
    }
}
