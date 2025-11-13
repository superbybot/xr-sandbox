using Unity.Tutorials.Core.Editor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace App.Demos.Car_Demo.Scripts.Interactable
{
    public class XRSteeringWheel : XRBaseInteractable
    {
        struct TrackedRotation
        {
            float m_BaseAngle;
            float m_CurrentOffset;
            float m_AccumulatedAngle;

            public float totalOffset => m_AccumulatedAngle + m_CurrentOffset;

            public void Reset()
            {
                m_BaseAngle = 0.0f;
                m_CurrentOffset = 0.0f;
                m_AccumulatedAngle = 0.0f;
            }

            public void SetBaseFromVector(Vector2 direction)
            {
                m_AccumulatedAngle += m_CurrentOffset;
                m_BaseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                m_CurrentOffset = 0.0f;
            }

            public void SetTargetFromVector(Vector2 direction)
            {
                var targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                m_CurrentOffset = Mathf.DeltaAngle(m_BaseAngle, targetAngle);

                if (Mathf.Abs(m_CurrentOffset) > 90.0f)
                {
                    m_BaseAngle = targetAngle;
                    m_CurrentOffset = 0.0f;
                }
            }
        }

        [Header("Custom Settings")]
        [SerializeField] private bool _onSelectByHover = false;

        [Header("Return to Origin Parameters")]
        [SerializeField] private bool _returnToOriginalRotation = true;
        [SerializeField] private float _returnSpeed = 5f;

        [Header("On Hover Parameters")]
        [SerializeField] private float _nearInteractionDistanceThreshold = 0.2f;
        
        [Header("Rotation Parameters")]
        [SerializeField] [Tooltip("Angle increments to snap to (0 = smooth)")]
        private float _angleIncrement = 0f;
        
        [SerializeField] [Tooltip("Minimum distance from center before tracking (prevents center jitter)")]
        private float _minTrackingRadius = 0.05f;
        
        [SerializeField] [Tooltip("Clamp rotation to min/max angles")]
        private bool _clampRotation = true;
        
        [SerializeField] [Tooltip("Maximum rotation angle")]
        private float _maxAngle = 450f;
        
        [SerializeField] [Tooltip("Minimum rotation angle")]
        private float _minAngle = -450f;
        
        [SerializeField] [Tooltip("Rotation smoothing (higher = smoother but less responsive)")]
        private float _rotationSmoothing = 8f;

        
        
        [SerializeField] private Transform _wheelTransform;
        public UnityEvent<float> OnWheelRotated;
        
        private IXRInteractor _leftInteractor;
        private IXRInteractor _rightInteractor;
        
        private Quaternion _originalRotation;
        private bool _isReturning = false;
        
        private TrackedRotation _leftTrackedRotation = new TrackedRotation();
        private TrackedRotation _rightTrackedRotation = new TrackedRotation();
        private float _baseWheelRotation = 0f;
        private float _smoothedWheelRotation = 0f;
        private IXRInteractor _primaryInteractor;

        protected override void Awake()
        {
            base.Awake();
            _originalRotation = _wheelTransform.localRotation;
        }
        
        public override Transform GetAttachTransform(IXRInteractor interactor)
        {
            return base.GetAttachTransform(interactor);
        }
        
        public override bool IsSelectableBy(IXRSelectInteractor interactor)
        {
            if (interactor is NearFarInteractor nearFar)
            {
                return base.IsSelectableBy(interactor) && 
                       nearFar.selectionRegion.Value == NearFarInteractor.Region.Near;
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

            if (_leftInteractor == null)
            {
                _leftInteractor = args.interactorObject;
                _leftTrackedRotation.Reset();
                
                if (_primaryInteractor == null)
                {
                    _primaryInteractor = _leftInteractor;
                }
                
                
                var interactorTransform = _leftInteractor.GetAttachTransform(this);
                var localPoint = FindLocalPoint(interactorTransform.position);
                _leftTrackedRotation.SetBaseFromVector(localPoint);
            }
            else if (_rightInteractor == null)
            {
                _rightInteractor = args.interactorObject;
                _rightTrackedRotation.Reset();
                
                if (_primaryInteractor == null)
                {
                    _primaryInteractor = _rightInteractor;
                }
                
                
                var interactorTransform = _rightInteractor.GetAttachTransform(this);
                var localPoint = FindLocalPoint(interactorTransform.position);
                _rightTrackedRotation.SetBaseFromVector(localPoint);
            }
    
            UpdateBaseWheelRotation();
            _smoothedWheelRotation = _baseWheelRotation;
            _isReturning = false;
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            if (_onSelectByHover)
            {
                return;
            }

            if (_leftInteractor == args.interactorObject)
            {
                _leftInteractor = null;
            }
            else if (_rightInteractor == args.interactorObject)
            {
                _rightInteractor = null;
            }
            
            if (_leftInteractor == null && _rightInteractor == null)
            {
                _primaryInteractor = null;
            }
    
            if (_returnToOriginalRotation && _leftInteractor == null && _rightInteractor == null)
            {
                _isReturning = true;
            }
        }

        protected override void OnHoverEntered(HoverEnterEventArgs args)
        {
            base.OnHoverEntered(args);
            if (_onSelectByHover == false)
            {
                return;
            }
            
            if (_leftInteractor == null)
            {
                _leftInteractor = args.interactorObject;
                _leftTrackedRotation.Reset();
                
                if (_primaryInteractor == null)
                {
                    _primaryInteractor = _leftInteractor;
                }
                
                
                var interactorTransform = _leftInteractor.GetAttachTransform(this);
                var localPoint = FindLocalPoint(interactorTransform.position);
                _leftTrackedRotation.SetBaseFromVector(localPoint);
            }
            else if (_rightInteractor == null)
            {
                _rightInteractor = args.interactorObject;
                _rightTrackedRotation.Reset();
                
                if (_primaryInteractor == null)
                {
                    _primaryInteractor = _rightInteractor;
                }
                
                
                var interactorTransform = _rightInteractor.GetAttachTransform(this);
                var localPoint = FindLocalPoint(interactorTransform.position);
                _rightTrackedRotation.SetBaseFromVector(localPoint);
            }
            
            UpdateBaseWheelRotation();
            _smoothedWheelRotation = _baseWheelRotation;
            _isReturning = false;
        }

        protected override void OnHoverExited(HoverExitEventArgs args)
        {
            base.OnHoverExited(args);
            if (_onSelectByHover == false)
            {
                return;
            }
            
            if (_leftInteractor == args.interactorObject)
            {
                _leftInteractor = null;
            }
            else if (_rightInteractor == args.interactorObject)
            {
                _rightInteractor = null;
            }
            
            if (_leftInteractor == null && _rightInteractor == null)
            {
                _primaryInteractor = null;
            }
            
            if (_returnToOriginalRotation)
            {
                _isReturning = true;
            }
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
                        RotateWheel();
                    }
                }
                    break;
            }
        }

        private bool IsSelected()
        {
            return (_onSelectByHover == false && isSelected) || (_onSelectByHover && isHovered);
        }

        private void RotateWheel()
        {
            float targetRotation = CalculateWheelRotation();
            
            if (_angleIncrement > 0)
            {
                targetRotation = Mathf.Round(targetRotation / _angleIncrement) * _angleIncrement;
            }
            
            if (_clampRotation)
            {
                targetRotation = Mathf.Clamp(targetRotation, _minAngle, _maxAngle);
            }
            
            _smoothedWheelRotation = Mathf.Lerp(_smoothedWheelRotation, targetRotation, Time.deltaTime * _rotationSmoothing);
            
            float currentWheelAngle = GetCurrentWheelRotation();
            float angleDifference = _smoothedWheelRotation - currentWheelAngle;
            
            _wheelTransform.Rotate(_wheelTransform.forward, angleDifference, Space.World);
            
            OnWheelRotated?.Invoke(angleDifference);
        }

private float CalculateWheelRotation()
{
    float totalOffset = 0f;
    int activeHandCount = 0;
    
    // Process left hand if active
    if (_leftInteractor != null)
    {
        var interactorTransform = _leftInteractor.GetAttachTransform(this);
        var localPoint = FindLocalPoint(interactorTransform.position);
        
        if (localPoint.magnitude > _minTrackingRadius)
        {
            _leftTrackedRotation.SetTargetFromVector(localPoint);
            totalOffset += _leftTrackedRotation.totalOffset;
            activeHandCount++;
        }
    }
    
    // Process right hand if active
    if (_rightInteractor != null)
    {
        var interactorTransform = _rightInteractor.GetAttachTransform(this);
        var localPoint = FindLocalPoint(interactorTransform.position);
        
        if (localPoint.magnitude > _minTrackingRadius)
        {
            _rightTrackedRotation.SetTargetFromVector(localPoint);
            totalOffset += _rightTrackedRotation.totalOffset;
            activeHandCount++;
        }
    }
    
    // Average the offsets from both hands
    if (activeHandCount > 1)
    {
        totalOffset /= activeHandCount;
    }
    
    return _baseWheelRotation + totalOffset;
}
        
        private void UpdateBaseWheelRotation()
        {
            _baseWheelRotation = GetCurrentWheelRotation();
        }
        
        private float GetCurrentWheelRotation()
        {
            Vector3 wheelForward = _wheelTransform.forward;
            Vector3 wheelUp = _wheelTransform.up;
            Vector3 originalUp = _originalRotation * Vector3.up;
            
            Vector3 currentUpProjected = Vector3.ProjectOnPlane(wheelUp, wheelForward);
            Vector3 originalUpProjected = Vector3.ProjectOnPlane(originalUp, wheelForward);
            
            float angle = Vector3.SignedAngle(originalUpProjected, currentUpProjected, wheelForward);
            return angle;
        }

        private Vector2 FindLocalPoint(Vector3 point)
        {
            Vector3 localPos = _wheelTransform.InverseTransformPoint(point);
            
            return new Vector2(localPos.x, localPos.y);
        }
    
        private void ReturnToOriginalRotation()
        {
            _wheelTransform.localRotation = Quaternion.Slerp(
                _wheelTransform.localRotation,
                _originalRotation,
                Time.deltaTime * _returnSpeed
            );
            
            if (Quaternion.Angle(_wheelTransform.localRotation, _originalRotation) < 0.1f)
            {
                _wheelTransform.localRotation = _originalRotation;
                _isReturning = false;
            }
        }
    }
}