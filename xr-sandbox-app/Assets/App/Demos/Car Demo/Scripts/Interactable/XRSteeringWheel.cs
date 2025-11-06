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
        [Header("Custom Settings")] 
        [SerializeField] private bool _onSelect = true;
        
        [Header("Return to Origin")]
        [SerializeField] private bool _returnToOriginalRotation = true;
        [SerializeField] private float _returnSpeed = 5f;

        [SerializeField] private float _nearInteractionDistanceThreshold = 0.2f;
        
        [SerializeField] private Transform _wheelTransform;
        public UnityEvent<float> OnWheelRotated;
        
        private float _currentAngle;
        private Quaternion _originalRotation;
        private bool _isReturning = false;

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
                float distance = Vector3.Distance(nearFar.transform.position, this.transform.position);

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

        protected override void Awake()
        {
            base.Awake();
            _originalRotation = _wheelTransform.localRotation;
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            if (_onSelect == false)
            {
                return;
            }
            _currentAngle = FindWheelAngle();
            _isReturning = false;
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            if (_onSelect == false)
            {
                return;
            }
            _currentAngle = FindWheelAngle();
            
            if (_returnToOriginalRotation)
            {
                _isReturning = true;
            }
        }

        protected override void OnHoverEntered(HoverEnterEventArgs args)
        {
            base.OnHoverEntered(args);
            if (_onSelect)
            {
                return;
            }
            _currentAngle = FindWheelAngle();
            _isReturning = false;
        }

        protected override void OnHoverExited(HoverExitEventArgs args)
        {
            base.OnHoverExited(args);
            if (_onSelect)
            {
                return;
            }
            _currentAngle = FindWheelAngle();
            
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
            return (_onSelect && isSelected) || (_onSelect == false && isHovered);
        }

        private void RotateWheel()
        {
            float totalAngle = FindWheelAngle();
            float angleDifference = _currentAngle - totalAngle;
            _wheelTransform.Rotate(transform.forward, -angleDifference);
            _currentAngle = totalAngle;
            OnWheelRotated?.Invoke(angleDifference);
        }

        private float FindWheelAngle()
        {
            float totalAngle = 0f;

            if (_onSelect)
            {
                foreach (var interactor in interactorsSelecting)
                {
                    var direction = FindLocalPoint(interactor.transform.position);
                    totalAngle += ConvertToAngle(direction) * FindRotationSensitivity();
                }
            }
            else
            {
                foreach (var interactor in interactorsHovering)
                {
                    var direction = FindLocalPoint(interactor.transform.position);
                    totalAngle += ConvertToAngle(direction) * FindRotationSensitivity();
                }
            }
            return totalAngle;
        }

        private Vector2 FindLocalPoint(Vector3 point)
        {
            return transform.InverseTransformPoint(point).normalized;
        }

        private float ConvertToAngle(Vector2 direction)
        {
            return Vector2.SignedAngle(transform.up, direction.normalized);
        }

        private float FindRotationSensitivity()
        {
            if (_onSelect)
            {
                return 1f / interactorsSelecting.Count;
            }
            else
            {
                return 1f / interactorsHovering.Count;
            }
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