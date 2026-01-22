# Floating Menu Demo

A VR floating menu system that toggles visibility via the left controller's menu button, follows the player's view, and includes a height adjustment slider.

## Overview

This demo provides a floating UI panel that:
- **Toggles on/off** with the left controller's menu button (Y button on Quest, Menu button on other controllers)
- **Faces the player** and follows their head position (like the Dialogue Demo)
- **Includes comfort settings** like a height adjustment slider for player accessibility
- **Uses XRI's UI interaction** for button presses and slider manipulation

## Features

### Core Menu
- [x] Player-facing floating panel (similar to VRPromptBox)
- [ ] Toggle visibility via left menu button press
- [ ] Smooth fade in/out animations
- [ ] Panel with placeholder content (rect + button)

### Height Adjustment
- [ ] Slider control for floor height offset
- [ ] Persisted height preference (PlayerPrefs)
- [ ] Visual feedback during adjustment

## Architecture

### Input Handling Approaches

There are several industry-standard approaches for showing/hiding VR menus:

#### Option A: InputActionReference (Recommended)
Uses Unity's Input System with XRI's input action assets. This is what the existing `CarInputManager` uses.

```csharp
[SerializeField] private InputActionReference menuToggleAction;

private void OnEnable()
{
    if (menuToggleAction != null && menuToggleAction.action != null)
    {
        menuToggleAction.action.Enable();
        menuToggleAction.action.performed += OnMenuToggle;
    }
}

private void OnMenuToggle(InputAction.CallbackContext context)
{
    ToggleMenuVisibility();
}
```

**Bind to:** `XRI Left/Menu` action from `XRI Default Input Actions.inputactions`

#### Option B: Direct InputAction Definition
Define the action inline in the component (simpler but less flexible).

```csharp
[SerializeField] private InputActionProperty menuAction;
// Set binding to: <XRController>{LeftHand}/menuButton
```

#### Option C: OVR Input (Meta-specific)
If targeting only Meta Quest:
```csharp
if (OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.LTouch))
{
    ToggleMenuVisibility();
}
```

### Recommended Approach

**Use Option A (InputActionReference)** because:
1. Already used in the project (`CarInputManager.exitAction`)
2. Works with both controllers and hand tracking
3. Leverages existing XRI input action assets
4. Most flexible and maintainable

### Menu Button Mapping

The left controller menu button maps to:
| Controller | Button | XRI Path |
|------------|--------|----------|
| Quest Touch | Y Button (Start) | `<XRController>{LeftHand}/menuButton` |
| Valve Index | Left Menu | `<XRController>{LeftHand}/menuButton` |
| HTC Vive | Left Menu | `<XRController>{LeftHand}/menuButton` |

The XRI Default Input Actions already has a "Menu" action defined in the "XRI Left" action map that can be used directly.

## Components

### FloatingMenuManager.cs
Main controller for the floating menu system.

**Responsibilities:**
- Listen for menu button press
- Toggle menu visibility with fade animation
- Position menu in front of player
- Manage menu state

### FloatingMenuPanel.cs
The actual UI panel component.

**Responsibilities:**
- Handle UI layout
- Process button/slider interactions
- Apply height offset to XR Origin

### HeightAdjuster.cs
Manages player height offset setting.

**Responsibilities:**
- Slider value change handling
- Apply offset to XR Origin's Camera Y Offset
- Save/load from PlayerPrefs

## Prefab Structure

```
FloatingMenuPanel
├── Canvas (World Space)
│   └── CanvasGroup (for fading)
├── FloatingText (from Dialogue Demo - positioning/facing)
├── FloatingMenuPanel (management)
└── UI
    ├── Background (Panel/Image)
    ├── TitleText (TextMeshProUGUI)
    ├── HeightSlider (UI Slider)
    │   ├── Label
    │   └── ValueText
    └── CloseButton (Button)
```

## Implementation Plan

### Phase 1: Basic Toggle Menu
1. Create `FloatingMenuManager.cs`
   - Add InputActionReference for menu toggle
   - Bind to XRI Left/Menu action
   - Implement toggle logic with debounce
   
2. Create `FloatingMenuPanel.cs`
   - Reuse `FloatingText` component from Dialogue Demo
   - Add CanvasGroup for fade animations
   - Implement Show/Hide with UniTask fade

3. Create basic prefab
   - World Space Canvas
   - Simple panel background
   - One test button

### Phase 2: Height Adjustment
1. Create `HeightAdjuster.cs`
   - UI Slider interaction
   - Apply to XR Origin camera offset
   - PlayerPrefs persistence
   
2. Update prefab
   - Add height slider UI
   - Connect to HeightAdjuster

### Phase 3: Polish
1. Add haptic feedback on menu toggle
2. Add audio feedback
3. Test with hand tracking (palm-up gesture alternative?)

## Setup Steps

### 1. Create the Scene
1. Create new scene `Floating Menu Demo`
2. Add XR Origin
3. Add PromptManager (from Dialogue Demo for reference)

### 2. Configure Input Action Reference
1. In `XRI Default Input Actions`, locate `XRI Left/Menu` action
2. Create InputActionReference asset or reference directly
3. Assign to FloatingMenuManager

### 3. Add FloatingMenuPanel Prefab
1. Create prefab following structure above
2. Add to scene
3. Reference in FloatingMenuManager

## Height Adjustment Common Values

Based on VR industry standards:

| Setting | Range | Default | Notes |
|---------|-------|---------|-------|
| Height Offset | -0.5m to +0.5m | 0m | Relative to calibrated floor |
| Increment | 0.01m (1cm) | - | Slider step |

Popular VR games (Beat Saber, VRChat, etc.) typically offer:
- **Automatic height detection** (optional)
- **Manual adjustment slider** (what we're implementing)
- **Reset to default** button

## Hand Tracking Considerations

For hand tracking input (future enhancement):
- **Palm-up gesture** on left hand could trigger menu
- **Gaze + pinch** on a wrist UI element
- **Timeout auto-close** when hands aren't tracked

The XRI system should handle the transition between controllers and hands automatically for button presses (on Quest).

## Dependencies

- **TextMeshPro** - For high-quality text rendering
- **UniTask** - For async/await animations (same as Dialogue Demo)
- **XR Interaction Toolkit** - For VR interaction and input system
- **Dialogue Demo Assets** - Reuse FloatingText component

## Testing Checklist

- [ ] Menu toggles with left menu button
- [ ] Menu faces player correctly
- [ ] Menu follows player position
- [ ] Fade animations work smoothly
- [ ] Height slider adjusts camera offset
- [ ] Height setting persists across sessions
- [ ] Works in VR headset
- [ ] Button on menu is clickable with controller ray

## Questions to Consider

1. **Should the menu have a close button?** Or only toggle via controller button?
2. **What additional settings?** Beyond height, consider:
   - Turn sensitivity
   - Vignette during movement
   - Seated/standing mode toggle
3. **Should menu pause the game?** Or allow interaction while open?
4. **Menu sound effects?** Open/close swoosh sounds?

## References

- [Unity XR Interaction Toolkit Documentation](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.0/manual/index.html)
- VRPromptBox implementation from Dialogue Demo
- CarInputManager.cs for InputActionReference pattern
