using System.Collections.Generic;
using UnityEngine;

namespace App.Demos.CarDemo.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class CarController : MonoBehaviour
    {
        [Header("Wheel Colliders")]
        [SerializeField] private WheelCollider frontLeftWheel;
        [SerializeField] private WheelCollider frontRightWheel;
        [SerializeField] private WheelCollider rearLeftWheel;
        [SerializeField] private WheelCollider rearRightWheel;

        [Header("Visual Wheels")]
        [SerializeField] private Transform frontLeftVisual;
        [SerializeField] private Transform frontRightVisual;
        [SerializeField] private Transform rearLeftVisual;
        [SerializeField] private Transform rearRightVisual;

        [Header("Physics")]
        [SerializeField] private Transform centerOfMass;

        [Header("Settings")]
        [SerializeField] private float motorTorque = 1500f;
        [SerializeField] private float brakeTorque = 3000f;
        [SerializeField] private float maxSteeringAngle = 30f;

        private float _currentAcceleration;
        private float _currentBrake;
        private float _currentSteeringAngle;
        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            if (centerOfMass != null)
            {
                _rb.centerOfMass = centerOfMass.localPosition;
            }
        }

        public void UpdateInput(float steering, float acceleration, float brake)
        {
            _currentSteeringAngle = maxSteeringAngle * steering;
            _currentAcceleration = motorTorque * acceleration;
            _currentBrake = brakeTorque * brake;
        }

        private void FixedUpdate()
        {
            ApplySteering();
            ApplyDrive();
            ApplyBraking();
        }

        private void Update()
        {
            UpdateVisualWheels();
        }

        private void ApplySteering()
        {
            if (frontLeftWheel != null)
                frontLeftWheel.steerAngle = _currentSteeringAngle;
            if (frontRightWheel != null)
                frontRightWheel.steerAngle = _currentSteeringAngle;
        }

        private void ApplyDrive()
        {
            if (rearLeftWheel != null)
                rearLeftWheel.motorTorque = _currentAcceleration;
            if (rearRightWheel != null)
                rearRightWheel.motorTorque = _currentAcceleration;
        }

        private void ApplyBraking()
        {
            if (frontLeftWheel != null)
                frontLeftWheel.brakeTorque = _currentBrake;
            if (frontRightWheel != null)

                frontRightWheel.brakeTorque = _currentBrake;
            if (rearLeftWheel != null)
                rearLeftWheel.brakeTorque = _currentBrake;
            if (rearRightWheel != null)
                rearRightWheel.brakeTorque = _currentBrake;
        }

        private void UpdateVisualWheels()
        {
            UpdateVisualWheel(frontLeftWheel, frontLeftVisual);
            UpdateVisualWheel(frontRightWheel, frontRightVisual);
            UpdateVisualWheel(rearLeftWheel, rearLeftVisual);
            UpdateVisualWheel(rearRightWheel, rearRightVisual);
        }

        private void UpdateVisualWheel(WheelCollider wheelCollider, Transform visualWheel)
        {
            if (wheelCollider == null || visualWheel == null) return;

            wheelCollider.GetWorldPose(out Vector3 position, out Quaternion rotation);
            visualWheel.position = position;
            visualWheel.rotation = rotation;
        }
    }
}
