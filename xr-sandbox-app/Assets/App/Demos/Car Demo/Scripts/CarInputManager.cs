using App.Shared.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace App.Demos.CarDemo.Scripts
{
    public class CarInputManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CarController carController;
        [SerializeField] private CarTeleportAnchor teleportAnchor;
        
        [Header("Inputs")]
        [Tooltip("Object implementing IVehicleInput (e.g., MetaSteeringWheel)")]
        [SerializeField] private MonoBehaviour steeringInputSource;
        
        [Tooltip("Object implementing IVehicleInput (e.g., MetaJoystick)")]
        [SerializeField] private MonoBehaviour throttleInputSource;
        
        [SerializeField] private InputActionReference exitAction;

        private bool _isSeated = false;

        [Header("Debug Info")]
        [SerializeField] private float debugSteering;
        [SerializeField] private float debugAcceleration;
        [SerializeField] private float debugBrake;

        private void OnEnable()
        {
            if (teleportAnchor != null)
            {
                teleportAnchor.OnCarEnter.AddListener(OnEnterCar);
                teleportAnchor.OnCarExit.AddListener(OnExitCar);
            }
            
            if (exitAction != null && exitAction.action != null)
            {
                exitAction.action.Enable();
            }
        }

        private void OnDisable()
        {
            if (teleportAnchor != null)
            {
                teleportAnchor.OnCarEnter.RemoveListener(OnEnterCar);
                teleportAnchor.OnCarExit.RemoveListener(OnExitCar);
            }

            if (exitAction != null && exitAction.action != null)
            {
                exitAction.action.Disable();
            }
        }

        private void OnEnterCar()
        {
            _isSeated = true;
        }

        private void OnExitCar()
        {
            _isSeated = false;
            if (carController != null)
            {
                carController.UpdateInput(0, 0, 0);
            }
        }

        private void Update()
        {
            if (!_isSeated) return;

            if (exitAction != null && exitAction.action != null && exitAction.action.WasPressedThisFrame())
            {
                if (teleportAnchor != null)
                {
                    teleportAnchor.ExitCar();
                    return;
                }
            }

            float steering = 0f;
            float acceleration = 0f;
            float brake = 0f;

            // Read Steering (Invert to match original logic if needed)
            if (steeringInputSource is IVehicleInput sInput)
            {
                steering = -sInput.Steering; 
            }

            // Read Throttle/Brake
            if (throttleInputSource is IVehicleInput tInput)
            {
                acceleration = tInput.Throttle;
                brake = tInput.Brake;
            }

            debugSteering = steering;
            debugAcceleration = acceleration;
            debugBrake = brake;

            if (carController != null)
            {
                carController.UpdateInput(steering, acceleration, brake);
            }
        }
    }
}
