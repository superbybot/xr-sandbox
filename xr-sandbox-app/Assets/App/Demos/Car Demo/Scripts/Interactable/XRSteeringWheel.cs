using Unity.Tutorials.Core.Editor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace App.Demos.Car_Demo.Scripts.Interactable
{
    public class XRSteeringWheel : XRBaseInteractable
    {
        [Header("Custom Settings")] 
        [SerializeField] private bool _onSelect = true;
        
        [SerializeField] private Transform _wheelTransform;
        public UnityEvent<float> OnWheelRotated;
        
        private float _currentAngle;

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            if (_onSelect == false)
            {
                return;
            }
            _currentAngle = FindWheelAngle();
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            if (_onSelect == false)
            {
                return;
            }
            _currentAngle = FindWheelAngle();
        }

        protected override void OnHoverEntered(HoverEnterEventArgs args)
        {
            base.OnHoverEntered(args);
            if (_onSelect)
            {
                return;
            }
            _currentAngle = FindWheelAngle();
        }

        protected override void OnHoverExited(HoverExitEventArgs args)
        {
            base.OnHoverExited(args);
            if (_onSelect)
            {
                return;
            }
            _currentAngle = FindWheelAngle();
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            base.ProcessInteractionStrength(updatePhase);

            switch (updatePhase)
            {
                case XRInteractionUpdateOrder.UpdatePhase.Dynamic:
                    if (IsSelected())
                    {
                        RotateWheel();
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
            
            Debug.Log("forward: " + transform.forward);
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
    }
}