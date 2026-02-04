using System;

namespace App.Shared.Scripts.Domain.Inputs
{
    /// <summary>
    /// Interface for vehicle input abstraction.
    /// Allows switching between different input implementations (Meta SDK, Keyboard, etc.)
    /// </summary>
    public interface IVehicleInput
    {
        /// <summary>
        /// Steering value from -1 (left) to 1 (right).
        /// </summary>
        float Steering { get; }
        
        /// <summary>
        /// Throttle value from 0 to 1.
        /// </summary>
        float Throttle { get; }
        
        /// <summary>
        /// Brake value from 0 to 1.
        /// </summary>
        float Brake { get; }
    }
}
