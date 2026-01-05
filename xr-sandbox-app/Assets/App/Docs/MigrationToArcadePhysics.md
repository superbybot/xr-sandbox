# Migration Guide: Simple Physics to Arcade Kart

This document outlines how to transition the current car implementation from our simple `CarController` to the robust, arcade-style `ArcadeKart` from the Karting Reference.

## Overview of Changes
*   **Current**: `CarController` uses standard Unity physics (torque/steering on WheelColliders).
*   **Target**: `ArcadeKart` uses custom physics for drifting, ground adhesion, and simplified handling.

## Step-by-Step Migration

### 1. Component Swapping
1.  Select your **Car** GameObject.
2.  **Remove** the `CarController` component.
3.  **Remove** `WheelController` scripts from the individual Wheel GameObjects (the `ArcadeKart` script handles visuals internally or via a separate helper).
4.  **Add** the `ArcadeKart` component (namespace `KartGame.KartSystems`).

### 2. Configuring ArcadeKart
1.  **Suspension**:
    *   Assign the 4 WheelCollider references in the `ArcadeKart` inspector (FrontLeft, FrontRight, RearLeft, RearRight).
    *   Adjust `Suspension Height` and `Suspension Spring` to match your car model's scale.
2.  **Visuals**:
    *   Assign the visual wheel GameObjects to the `Visual Wheels` list in `ArcadeKart`.
3.  **Physics**:
    *   Assign the `Center Of Mass` transform.
    *   Configure `Top Speed`, `Acceleration`, and `Steer` in the "Base Stats" section.

### 3. Updating Input Logic
You need to modify `CarInputManager.cs` to talk to `ArcadeKart` instead of `CarController`.

#### Changes to `CarInputManager.cs`:
1.  Change the reference field:
    ```csharp
    // [SerializeField] private CarController carController; // OLD
    [SerializeField] private KartGame.KartSystems.ArcadeKart arcadeKart; // NEW
    ```
2.  Create a custom Input struct for ArcadeKart. `ArcadeKart` expects an `InputData` struct.
    *   *Note: `ArcadeKart` usually polls for input via `IInput` interfaces. You can either implement `IInput` on `CarInputManager` or modify `ArcadeKart` to accept direct injection.*
    *   **Recommended Approach**: Implement `IInput` on `CarInputManager`.

    ```csharp
    public class CarInputManager : MonoBehaviour, KartGame.KartSystems.IInput
    {
        // ... existing XR input code ...

        public KartGame.KartSystems.InputData GenerateInput()
        {
            return new KartGame.KartSystems.InputData
            {
                Accelerate = _currentAcceleration > 0,
                Brake = _currentBrake > 0,
                TurnInput = _currentSteering // Normalized -1 to 1
            };
        }
    }
    ```

### 4. Handling Teleportation
The `CarTeleportAnchor` logic remains largely the same.
*   **Enter/Exit**: Still handled by `CarTeleportAnchor`.
*   **Locomotion**: Still disabled/enabled by `CarTeleportAnchor`.
*   **Input**: `CarInputManager` will only return valid input values when `_isSeated` is true.

## Verification
1.  Play the scene.
2.  Teleport into the car.
3.  The `ArcadeKart` script should now pick up inputs from `CarInputManager` (via the `IInput` interface).
4.  Test drifting (usually requires a "Hop" button mapping, which you might need to map to a controller button).
