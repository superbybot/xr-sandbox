using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace App.Demos.Car_Demo.Scripts.Interactable
{
    public class XRSteeringWheel : XRBaseInteractable
    {
        [Serializable]
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
        [SerializeField] private float _angleIncrement = 0f;
        [SerializeField] private float _minTrackingRadius = 0.05f;
        [SerializeField] private bool _clampRotation = true;
        [SerializeField] private float _maxAngle = 450f;
        [SerializeField] private float _minAngle = -450f;
        
        [SerializeField] private float _rotationSmoothing = 8f;
        [SerializeField] private Transform _wheelTransform;
        
        public UnityEvent<float> OnWheelRotated;
        private IXRInteractor _leftInteractor;
        private IXRInteractor _rightInteractor;
        private IXRInteractor _primaryInteractor;
        private Quaternion _originalRotation;
        private bool _isReturning = false;

        [SerializeField] private TrackedRotation _leftTrackedRotation = new ();
        [SerializeField] private TrackedRotation _rightTrackedRotation = new ();
        [SerializeField] private float _baseWheelRotation = 0f;
        [SerializeField] private float _smoothedWheelRotation = 0f;

        public float Value => Mathf.Clamp(_smoothedWheelRotation / _maxAngle, -1f, 1f);

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
            Vector3 localForward = Vector3.forward;
            Vector3 currentLocalUp = _wheelTransform.localRotation * Vector3.up;
            Vector3 originalLocalUp = _originalRotation * Vector3.up;

            Vector3 currentUpProjected = Vector3.ProjectOnPlane(currentLocalUp, localForward);
            Vector3 originalUpProjected = Vector3.ProjectOnPlane(originalLocalUp, localForward);

            float angle = Vector3.SignedAngle(originalUpProjected, currentUpProjected, localForward);
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

            float previousSmoothedRotation = _smoothedWheelRotation;
            _smoothedWheelRotation = Mathf.Lerp(_smoothedWheelRotation, 0f, Time.deltaTime * _rotationSmoothing);
            
            float angleDifference = _smoothedWheelRotation - previousSmoothedRotation;
            OnWheelRotated?.Invoke(angleDifference);

            if (Quaternion.Angle(_wheelTransform.localRotation, _originalRotation) < 0.1f)
            {
                _wheelTransform.localRotation = _originalRotation;
                _smoothedWheelRotation = 0f;
                _isReturning = false;
            }
        }
    }
}