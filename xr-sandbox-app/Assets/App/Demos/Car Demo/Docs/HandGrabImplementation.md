# Implementing HandGrabInteractable in XR Interaction Toolkit

This guide details how to implement a hand interaction system similar to Meta's `HandGrabInteractable` (from Oculus Interaction SDK) using Unity's XR Interaction Toolkit (XRI).

## Overview

The goal is to achieve high-fidelity hand interactions where the virtual hand snaps to a specific pose and position on an object when grabbed.

| Feature | Oculus Interaction SDK | XR Interaction Toolkit (Implementation) |
| :--- | :--- | :--- |
| **Component** | `HandGrabInteractable` | `XRGrabInteractable` + Custom `HandGrabPose` |
| **Hand Pose** | `HandGrabPose` / `HandPose` | Animator Parameters / Animation Clips |
| **Snapping** | `HandGrabPoint` | `Attach Transform` on `XRGrabInteractable` |

## Prerequisites

*   Unity 2022.3 or later
*   XR Interaction Toolkit 3.x installed
*   XR Hands package installed (optional but recommended for hand tracking)

## Implementation Steps

### 1. Setup the Interactable Object

1.  Select your target object (e.g., a cup or tool).
2.  Add a **Rigidbody** and **Collider** (if not present).
3.  Add the **XR Grab Interactable** component.
4.  Configure `XR Grab Interactable`:
    *   **Movement Type**: *Velocity Tracking* (for physics objects) or *Kinematic*.
    *   **Select Mode**: *Multiple* (if two-handed grab is needed) or *Single*.

### 2. Define the Hand Pose (The "Ghost Hand")

In XRI, we drive the hand visual using Unity's Animator system.

1.  **Prepare your Hand Animator**: Ensure your Hand Visual prefab (on the XR Rig) has an `Animator` component.
2.  **Create Animation Clips**: Create animation clips for your specific grabs (e.g., `CupGrab`, `PistolGrip`, `Pinch`).
3.  **Setup Animator Controller**:
    *   Add Boolean parameters to your Animator Controller corresponding to these poses (e.g., `IsGrabbingCup`, `IsGrabbingPistol`).
    *   Create transitions from the "Idle" or "Open" state to these grab states based on the parameters.

### 3. Create the `HandGrabPose` Script

Create a script to tag your interactable with the desired pose.

**File: `Assets/Scripts/HandGrabPose.cs`**

```csharp
using UnityEngine;

public class HandGrabPose : MonoBehaviour
{
    [Tooltip("The name of the boolean parameter in the Hand Animator to trigger when grabbed.")]
    public string poseAnimationParameter = "Grab";
    
    [Tooltip("Optional: Offset for the hand visual if needed.")]
    public Vector3 positionOffset;
    public Vector3 rotationOffset;
}
```

### 4. Create the `HandVisualController` Script

Create a script to control the hand visual based on what is being grabbed. Attach this to your Hand Interactor GameObject (the parent of the visual).

**File: `Assets/Scripts/HandVisualController.cs`**

```csharp
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandVisualController : MonoBehaviour
{
    [SerializeField] private Animator handAnimator;
    [SerializeField] private XRBaseInteractor interactor;

    private void OnEnable()
    {
        if (interactor == null) interactor = GetComponentInParent<XRBaseInteractor>();
        interactor.selectEntered.AddListener(OnSelectEnter);
        interactor.selectExited.AddListener(OnSelectExit);
    }

    private void OnDisable()
    {
        interactor.selectEntered.RemoveListener(OnSelectEnter);
        interactor.selectExited.RemoveListener(OnSelectExit);
    }

    private void OnSelectEnter(SelectEnterEventArgs args)
    {
        // Check if the grabbed object has a HandGrabPose definition
        if (args.interactableObject.transform.TryGetComponent<HandGrabPose>(out var pose))
        {
            // Trigger the specific pose
            if (handAnimator != null)
            {
                handAnimator.SetBool(pose.poseAnimationParameter, true);
            }
        }
        else
        {
            // Fallback to generic grab
            if (handAnimator != null)
            {
                handAnimator.SetBool("Grab", true);
            }
        }
    }

    private void OnSelectExit(SelectExitEventArgs args)
    {
        // Reset the pose
        if (args.interactableObject.transform.TryGetComponent<HandGrabPose>(out var pose))
        {
            if (handAnimator != null)
            {
                handAnimator.SetBool(pose.poseAnimationParameter, false);
            }
        }
        else
        {
            if (handAnimator != null)
            {
                handAnimator.SetBool("Grab", false);
            }
        }
    }
}
```

### 5. Configure the Snap Point (Attach Transform)

To ensure the hand meshes with the object perfectly (like `HandGrabPoint` in OIS):

1.  Create an empty GameObject as a child of your Interactable. Name it **GrabPoint**.
2.  Position and rotate this **GrabPoint** so that if a hand were at this position (0,0,0) and rotation (0,0,0), it would be holding the object correctly.
    *   *Tip*: Temporarily drag your Hand Visual prefab under the Interactable, reset its transform, and then move the **Interactable** (parent) until the hand looks right. Then move the Hand Visual back out. The inverse of that transform is your GrabPoint.
    *   *Easier Method*: Place the **GrabPoint** where the wrist should be.
3.  Assign this **GrabPoint** to the **Attach Transform** field in the `XR Grab Interactable` component.

### 6. Final Assembly

1.  Add the `HandGrabPose` component to your interactable object.
2.  Set the `Pose Animation Parameter` to match the parameter in your Animator (e.g., "CupGrab").
3.  Ensure your XR Rig's hand interactors have the `HandVisualController` script and references assigned.
4.  Run the scene. When you grab the object, the hand should snap to the Attach Transform and the Animator should transition to the defined pose.
