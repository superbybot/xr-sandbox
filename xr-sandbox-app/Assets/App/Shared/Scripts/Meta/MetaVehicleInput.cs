using UnityEngine;
using App.Shared.Scripts.Interfaces;

namespace App.Shared.Scripts.Meta
{
    /// <summary>
    /// Implements IVehicleInput using Meta SDK joystick and steering wheel components.
    /// Attach this to a GameObject and assign the Meta interactables to use interface-based input.
    /// </summary>
    public class MetaVehicleInput : MonoBehaviour, IVehicleInput
    {
        [Header("References")]
        [SerializeField] private MetaJoystick _joystick;
        [SerializeField] private MetaSteeringWheel _steeringWheel;
        
        [Header("Debug")]
        [SerializeField] private float _debugSteering;
        [SerializeField] private float _debugThrottle;
        [SerializeField] private float _debugBrake;
        
        /// <summary>
        /// Steering value from -1 (left) to 1 (right).
        /// </summary>
        public float Steering 
        { 
            get
            {
                float value = _steeringWheel != null ? _steeringWheel.Steering : 0f;
                _debugSteering = value;
                return value;
            }
        }
        
        /// <summary>
        /// Throttle value from 0 to 1.
        /// </summary>
        public float Throttle 
        { 
            get
            {
                float value = _joystick != null ? _joystick.Throttle : 0f;
                _debugThrottle = value;
                return value;
            }
        }
        
        /// <summary>
        /// Brake value from 0 to 1.
        /// </summary>
        public float Brake 
        { 
            get
            {
                float value = _joystick != null ? _joystick.Brake : 0f;
                _debugBrake = value;
                return value;
            }
        }
    }
}
