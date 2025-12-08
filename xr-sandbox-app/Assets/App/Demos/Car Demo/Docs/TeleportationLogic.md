# Car Demo Teleportation Logic

## Overview
The teleportation system in the Car Demo allows the player to teleport directly into the driver's seat of the car. This is handled by the `CarTeleportAnchor` script.

## Implementation Details
-   **Script**: `CarTeleportAnchor.cs`
-   **Base Class**: Inherits from `UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationAnchor`.
-   **Namespace**: `App.Demos.CarDemo.Scripts`

### How it Works
1.  **Teleportation Anchor**: The script acts as a standard XR Teleportation Anchor. When the player teleports to it, the `OnTeleporting` event is triggered.
2.  **Seat Anchor**: Instead of teleporting to the anchor's pivot, we override the destination to be the `SeatAnchor` transform (a child of the car).
3.  **Locomotion Switching**:
    -   **Enter**: When the player enters the car, we disable standard locomotion (teleport/move) to prevent walking out of the car while driving.
    -   **Exit**: When the player exits (via the Menu button), we teleport them to the `ExitPoint` and re-enable standard locomotion.

## References
This implementation is based on the standard **XR Interaction Toolkit** teleportation system.
-   **Reference Class**: `UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationAnchor`
-   **Documentation**: [Unity XR Interaction Toolkit Docs](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.0/manual/teleportation.html)

We customized it by adding the `EnterCar` and `ExitCar` logic to manage the player's state and inputs.
