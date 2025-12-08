using App.Demos.Car_Demo.Scripts.Interactable;
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
        [SerializeField] private XRSteeringWheel steeringWheel;
        [SerializeField] private XRJoystick joystick;
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

            if (steeringWheel != null)
            {
                steering = -steeringWheel.Value; 
            }

            if (joystick != null)
            {
                Vector2 input = joystick.Value;

                if (input.y > 0)
                {
                    acceleration = input.y;
                }
                else if (input.y < -0.9f) // Full reverse when joystick is nearly fully back
                {
                    acceleration = input.y; // Negative acceleration = reverse
                }
                else if (input.y < 0)
                {
                    brake = -input.y;
                }
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
