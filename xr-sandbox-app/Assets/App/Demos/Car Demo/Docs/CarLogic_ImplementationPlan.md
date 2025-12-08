# Add Car Logic to Car Demo

## Goal Description
Implement a physics-based car controller for the Car Demo scene in the `xr-sandbox` project. This will allow the user to drive the "KartClassic" model using the existing `XRSteeringWheel` and `XRJoystick` interactables.

## Reference Material
As per project rules, we will reference the following project for car mechanics and gameplay patterns:
-   `C:\Users\Wonderfy149\Documents\unity_kartinggame`

## Proposed Changes

### Car Demo Assets
#### [NEW] [Imported Assets]
-   Copy `Karting` folder from `unity_kartinggame` to `Assets/App/Demos/Car Demo/Karting_Reference`.
-   This includes `PhysicsPlayground.unity` and its dependencies (models, materials, etc.).
-   **Note**: We will use `PhysicsPlayground` as the test environment for our new car logic.

### Car Demo Scripts
#### [NEW] [CarController.cs](file:///c:/Users/Wonderfy149/Documents/xr-sandbox/xr-sandbox-app/Assets/App/Demos/Car Demo/Scripts/CarController.cs)
-   Manages the overall car physics (Rigidbody).
-   Handles acceleration, braking, and steering logic.
-   Applies forces to the wheels.

#### [NEW] [WheelController.cs](file:///c:/Users/Wonderfy149/Documents/xr-sandbox/xr-sandbox-app/Assets/App/Demos/Car Demo/Scripts/WheelController.cs)
-   Manages individual wheel physics (WheelCollider).
-   Updates visual wheel transforms to match physics.

#### [NEW] [CarTeleportAnchor.cs](file:///c:/Users/Wonderfy149/Documents/xr-sandbox/xr-sandbox-app/Assets/App/Demos/Car Demo/Scripts/CarTeleportAnchor.cs)
-   Implements `TeleportationAnchor` (from XR Toolkit) to allow teleporting to the car.
-   Provides a custom reticle/preview when hovering over the car.
-   **Seat Anchor**: Requires a `SeatAnchor` Transform (child of the car) to define exactly where the player sits.
-   On teleport select:
    -   Positions the player at the `SeatAnchor`.
    -   Disables standard locomotion (teleport/continuous move).
    -   Enables car input controls.
-   **Exit Logic**:
    -   Defines an `ExitPoint` transform near the car.
    -   Exposes a public `ExitCar()` method that teleports the player to the `ExitPoint` and re-enables standard locomotion.

#### [NEW] [CarInputManager.cs](file:///c:/Users/Wonderfy149/Documents/xr-sandbox/xr-sandbox-app/Assets/App/Demos/Car Demo/Scripts/CarInputManager.cs)
-   Reads input from `XRSteeringWheel` (steering) and `XRJoystick` (throttle/brake).
-   **Exit Input**: Listens for the **Left Controller Menu Button**.
-   When pressed, calls `CarTeleportAnchor.ExitCar()`.
-   Passes input values to the `CarController`.
-   **Update**: Only active when player is "seated" in the car.

## Verification Plan

### Manual Verification
-   **Setup**:
    -   Create a Car GameObject using `KartClassic_Body` and `KartClassic_Wheels`.
    -   Add `CarController`, `CarInputManager`, `WheelController`, and `CarTeleportAnchor` components.
    -   Link `XRSteeringWheel` and `XRJoystick` to the `CarInputManager`.
    -   Configure `CarTeleportAnchor` with a seat transform.
-   **Test**:
    -   Enter Play Mode.
    -   Aim teleport ray at car -> Verify custom preview appears.
    -   Teleport to car -> Verify player moves to seat.
    -   Try to move with joystick -> Verify standard movement is disabled.
    -   Grab and turn the steering wheel -> Verify front wheels turn.
    -   Push joystick forward -> Verify car accelerates.
    -   Pull joystick back -> Verify car brakes/reverses.
    -   Exit car -> Verify standard movement is restored.
