# Hand Snapping Demo Plan (Steering Wheel)

## Goal
Create a demo where a VR hand "snaps" to specific handles on a steering wheel when grabbed, ensuring a high-fidelity visual connection similar to the "NorthStar" demo, but using the **XR Interaction Toolkit (XRI)**.

## Reference
- **Target Behavior**: NorthStar's `SteeringWheel.prefab`.
- **Key Mechanic**: When the user's hand hovers/grabs a handle, the virtual hand model snaps to a pre-defined pose and position on the wheel, while the controller continues to drive the wheel's rotation.

## Technical Mapping (Meta vs. XRI)

We are basing our implementation on **`HandGrabInteractable`** from the **Oculus Interaction SDK**.

| Feature | Meta (Oculus Interaction SDK) | Our Implementation (XR Interaction Toolkit) |
| :--- | :--- | :--- |
| **Main Component** | **`HandGrabInteractable`** | **`XRGrabInteractable`** |
| **Pose Definition** | `HandGrabPose` / `HandPose` (ScriptableObject) | **`HandGrabPose`** (Custom MonoBehaviour) |
| **Snapping Logic** | `HandGrabPoint` | `Attach Transform` (Standard XRI) + `HandVisualController` |
| **Hand Visuals** | `HandVisual` (Driven by OIS) | `HandVisualController` (Drives Unity Animator) |

> **Note**: In Meta's SDK, `HandGrabInteractable` is a complex script that handles both the physics of the grab AND the hand pose data. In XRI, we separate these: `XRGrabInteractable` handles physics, and our custom `HandGrabPose` handles the visual data.

## Implementation Strategy

### 1. Core Mechanics (XRI)
We will use standard XRI components with custom scripts to handle the visual snapping.

*   **Interaction**: `XRGrabInteractable`.
*   **Physics**: `HingeJoint` (to constrain the wheel to rotate around one axis).
*   **Snapping**:
    *   Instead of one single grab point, we will define **discrete handles** (or "Snap Zones") around the wheel.
    *   Dynamic Snapping: If grabbing the rim anywhere, we need a script to calculate the nearest snap point.

### 2. Proposed Architecture

#### A. Scripts
We will create (or reuse) the following scripts:

1.  **`HandGrabPose.cs`** (Component)
    *   Attachments to the object (the Wheel Handle).
    *   Stores data: `HandPoseName` (string), `PositionOffset`, `RotationOffset`.
    *   *Purpose*: Tells the hand *how* to hold this specific object.

2.  **`HandVisualController.cs`** (Component)
    *   Attached to the **Hand Interactor** (the player's hand).
    *   Listens to `SelectEnter` / `SelectExit` events.
    *   *Purpose*: When grabbing an object with `HandGrabPose`, it snaps the **Hand Visual** (mesh) to the target's attach point and triggers the Animator.

3.  **`SteeringWheelController.cs`** (Optional Wrapper)
    *   Manages the wheel's physics and feedback (haptics, return-to-center force, etc.).

#### B. Hierarchy Structure
```text
SteeringWheel (Rigidbody + HingeJoint)
├── Visuals (Mesh)
├── Colliders (Wheel Collider)
├── Handle_Top (XRGrabInteractable + HandGrabPose)
│   └── AttachTransform (Positioned perfectly for the hand)
├── Handle_Left (XRGrabInteractable + HandGrabPose)
│   └── AttachTransform
├── Handle_Right (XRGrabInteractable + HandGrabPose)
│   └── AttachTransform
└── ...
```

### 3. Step-by-Step Plan

#### Phase 1: Preparation
1.  **Create Folder**: `Assets/App/Docs/Hand Snapping Demo` (Done).
2.  **Create Scripts**: Implement `HandGrabPose` and `HandVisualController` if not present.

#### Phase 2: Scene Setup
1.  **Wheel Object**: Import or create a cylinder/torus to act as the wheel.
2.  **Physics**: Add `Rigidbody` and `HingeJoint` to anchor it to a base.
3.  **Handles**: Create empty GameObjects around the rim to serve as "Grab Anchors".

#### Phase 3: Interaction & Snapping
1.  Add `XRGrabInteractable` to the Handles (or the main wheel with multiple colliders).
    *   *Note*: For a continuous rim, ONE interactable is better. We will use **Dynamic Attach**.
    *   *Refined Approach*: One `XRGrabInteractable` on the Wheel.
    *   On `SelectEnter`:
        1.  Calculate the nearest point on the rim to the hand.
        2.  Move the `AttachTransform` of the Interactable to that point *instantly* before the grab processes.
        3.  Snap the visual hand to that point.

#### Phase 4: Animation
1.  Create a "Fist" or "Grip_Bar" animation for the hand.
2.  Trigger this animation when grabbing the wheel.

## Next Steps
1.  Review this plan.
2.  Begin "Phase 1": Create the Scripts.
