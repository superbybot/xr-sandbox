using UnityEngine;

namespace App.Demos.CarDemo.Scripts
{
    [RequireComponent(typeof(WheelCollider))]
    public class WheelController : MonoBehaviour
    {
        [SerializeField] private Transform visualWheel;

        private WheelCollider _wheelCollider;

        private void Awake()
        {
            _wheelCollider = GetComponent<WheelCollider>();
        }

        private void Update()
        {
            UpdateVisuals();
        }

        public void Steer(float angle)
        {
            _wheelCollider.steerAngle = angle;
        }

        public void Accelerate(float torque)
        {
            _wheelCollider.motorTorque = torque;
        }

        public void Brake(float torque)
        {
            _wheelCollider.brakeTorque = torque;
        }

        private void UpdateVisuals()
        {
            if (visualWheel == null) return;

            Vector3 position;
            Quaternion rotation;
            _wheelCollider.GetWorldPose(out position, out rotation);

            visualWheel.position = position;
            visualWheel.rotation = rotation;
        }
    }
}
