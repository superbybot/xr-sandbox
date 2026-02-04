# 🚀 Migration Plan: Unity XR Toolkit to Meta XR SDK

> [!IMPORTANT]
> **Backup Required**: Ensure your project is fully committed to source control before starting this migration.

This document outlines the steps to migrate `xr-sandbox` from **Unity XR Interaction Toolkit (XRI)** to **Meta XR SDK (Interaction SDK)**.

## 1. 🔍 Overview & Impact Analysis

We are replacing the generic XRI system with **Meta's native Interaction SDK** to gain access to superior hand tracking, precise "poke" interactions for UI, and optimized performance for Quest.

### ⚠️ Affected Systems
*   **Player Rig**: `XR Origin` ➡️ `OVRCameraRig` + `Interaction Rig`.
*   **Interactables**: Custom `XRBaseInteractable` scripts (Joystick, Wheel) ➡️ **Meta `Grabbable`** + **`Transformers`**.
*   **Events**: `SelectEntered` ➡️ `WhenSelect()`.
*   **Locomotion**: Teleport Anchors ➡️ Meta Locomotion.

---

## 2. 🛠️ Prerequisites & Setup

1.  **Backup Project**: Check your git status!
2.  **Install Meta XR All-in-One SDK**:
    *   Target: **Window > Package Manager**.
    *   Package: `com.meta.xr.sdk.all` (or via Asset Store).
    *   *Includes: Core, Interaction, Building Blocks, Audio.*
3.  **Project Settings**:
    *   **Edit > Project Settings > XR Plug-in Management**.
    *   ✅ Ensure **Oculus** is active.
    *   ✅ Run "Fix All" check under **Meta XR**.

---

## 3. 🏗️ Migration Steps

### Phase 1: Player Rig Replacement
**Goal**: Swap the camera and hands.

1.  ❌ **Remove**: Disable `XR Origin`.
2.  ✅ **Add**: From **Meta > Building Blocks**:
    *   `Camera Rig` (OVRCameraRig)
    *   `Interaction Rig` (Hands/Controllers/Interactors)
    *   *(Optional)* `Player Locomotion`

### Phase 2: Input Mapping Table

| Unity XRI Component | Meta SDK Equivalent | note |
| :--- | :--- | :--- |
| `XR Ray Interactor` | `Ray Interactor` | Hand/Controller rays are separate. |
| `XR Direct Interactor`| `Grab Interactor` | Detects grabs. |
| `XR Poke Interactor` | `Poke Interactor` | **Superior** for UI interaction. |
| `ActionBasedController`| `OVRInput` | Direct API calls. |

### Phase 3: Converting Interactables

> [!TIP]
> **Strategy**: Prefer Composition over Inheritance. Use components like `Grabbable` instead of inheriting from a base class when possible.

#### 🕹️ Example: Joystick & Steering Wheel
**Unity XRI** (Old):
```csharp
public class XRJoystick : XRBaseInteractable { ... }
```

**Meta SDK** (New):
*   Add `Grabbable` component.
*   Add `OneGrabRotateTransformer` (for Wheel).
*   Add `GrabbableUnityEventWrapper` to listen for events.

#### 🔔 Example: Prompt Trigger
**Unity XRI** (Old):
```csharp
interactable.selectEntered.AddListener(OnInteraction);
```

**Meta SDK** (New):
```csharp
// Use PointableUnityEventWrapper to hook into Grabbable events
GetComponent<PointableUnityEventWrapper>().WhenSelect.AddListener(OnInteraction);
```

### Phase 4: UI & Input
*   **Canvas**: Add `PointableCanvas` to any UI Canvas.
*   **Input**: Switch `InputSystem` calls to `OVRInput.Get(OVRInput.Button.One)`.

### Phase 5: 🧩 Architectural Abstraction (Cross-Platform)

> [!NOTE]
> This phase aligns with our **Clean Architecture** goal to decouple logic from SDKs.

**Goal**: Support switching between Meta SDK, generic inputs, or keyboard testing.

1.  **Define Interfaces** (`Domain/Inputs`):
    *   `IVehicleInput`: Returns `Steering`, `Throttle`.
    *   `IHandInput`: Returns `Pinching`, `Position`.
2.  **Implement Adapters**:
    *   `MetaVehicleInput`: Adapts Meta `Grabbable` values ➡️ `IVehicleInput`.
    *   `KeyboardVehicleInput`: Adapts `Input.GetKey` ➡️ `IVehicleInput` (for fast testing).
3.  **Dependency Injection** (`RacingLifetimeScope`):
    ```csharp
    // Switch implementation implementation based on platform
    #if UNITY_ANDROID
        builder.Register<MetaVehicleInput>(Lifetime.Singleton).As<IVehicleInput>();
    #else
        builder.Register<KeyboardVehicleInput>(Lifetime.Singleton).As<IVehicleInput>();
    #endif
    ```

---

## 4. 📂 Demo-Specific Impact

### 1. 🏎️ **Car Demo**
*   **Impact**: 🔴 **High**
*   **Action**: Rewrite Joystick/Wheel using `Grabbable` + `Transformer`. Secure Teleport logic.

### 2. 💬 **Dialogue Demo**
*   **Impact**: 🟡 **Medium**
*   **Action**: Update `PromptTrigger` to listen to Meta Events. Add `PointableCanvas` to UI.

### 3. 🌫️ **Floating Menu & Transition**
*   **Impact**: 🟡 **Medium**
*   **Action**: `PointableCanvas` for menus. Ensure Height Adjuster references `CenterEyeAnchor`.

### 4. 🕺 **IK Demo**
*   **Impact**: 🔴 **High**
*   **Action**: Remap IK Targets to `OVRCameraRig` anchors (HandLeft/HandRight).

### 5. 🏁 **Racing Demo**
*   **Impact**: 🟢 **Low** (Logic-wise)
*   **Action**: Update Tags. Inject `IVehicleInput` into `DriveCarUseCase`.

---

## 5. 📋 Specific File Remediation

| File | Priority | Action |
| :--- | :--- | :--- |
| `XRJoystick.cs` | 🔴 High | Rewrite implementation. |
| `XRSteeringWheel.cs` | 🔴 High | Rewrite implementation. |
| `CarInputManager.cs` | 🔴 High | Refactor to consume `IVehicleInput` interface. |
| `PromptTrigger.cs` | 🟡 Medium | Update event listeners. |
| `Floating Menu.unity`| 🔴 High | Replace Rig + Update Canvas. |

---

## 6. ✅ Verification
1.  **Editor Test**: Use **Meta XR Simulator**.
2.  **Interaction Check**:
    *   [ ] Can you grab the wheel?
    *   [ ] Does the UI spawn on button press?
    *   [ ] Does the car drive?

## 7. 📚 Resources
*   [Meta Interaction SDK Documentation](https://developer.oculus.com/documentation/unity/unity-isdk-interaction-sdk-overview/)
*   [Meta XR Building Blocks](https://developer.oculus.com/documentation/unity/unity-building-blocks-overview/)
