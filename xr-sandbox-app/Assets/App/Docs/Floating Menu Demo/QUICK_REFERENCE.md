# Floating Menu Demo - Quick Reference

## Overview

Floating VR menu that toggles with the left controller's menu button, faces the player, and includes height adjustment.

## Core Scripts

### FloatingMenuManager.cs
Main controller (Singleton)

**Key Methods:**
```csharp
// Toggle menu visibility
FloatingMenuManager.Instance.ToggleMenu();

// Direct show/hide
FloatingMenuManager.Instance.ShowMenu();
FloatingMenuManager.Instance.HideMenu();

// Check state
bool isOpen = FloatingMenuManager.Instance.IsMenuOpen;
```

### FloatingMenuPanel.cs
Panel UI controller

**Key Methods:**
```csharp
// Async show/hide with fade
await panel.ShowAsync();
await panel.HideAsync();

// Instant visibility
panel.SetVisible(true);
panel.SetVisible(false);
```

### HeightAdjuster.cs
Height offset controller

**Key Methods:**
```csharp
// Set height offset (meters)
HeightAdjuster.Instance.SetHeightOffset(0.1f);

// Get current offset
float offset = HeightAdjuster.Instance.GetHeightOffset();

// Reset to default
HeightAdjuster.Instance.ResetHeight();
```

## Input Binding

**Default Binding:** Left Controller Menu Button

| Controller | Physical Button |
|------------|-----------------|
| Quest Touch | Y Button |
| Valve Index | Left Menu Button |
| HTC Vive | Left Menu Button |

**XRI Path:** `<XRController>{LeftHand}/menuButton`

**Action Reference:** `XRI Left/Menu`

## Setup Steps

### 1. Add to Scene
1. Drag `FloatingMenuPanel` prefab into scene
2. Add `FloatingMenuManager` component to XR Origin or empty GameObject
3. Assign references:
   - Menu Panel reference
   - Menu Toggle Action (XRI Left/Menu)

### 2. Configure Input Action
1. Ensure `XRI Default Input Actions` is in your project
2. Create InputActionReference pointing to `XRI Left/Menu`
3. Assign to FloatingMenuManager's `menuToggleAction` field

### 3. Setup Height Adjuster
1. Add Slider to menu panel
2. Add `HeightAdjuster` component
3. Assign slider reference
4. Assign XR Origin reference (for floor offset)

## Height Adjustment

**Range:** -0.5m to +0.5m  
**Default:** 0m  
**Step:** 0.01m (1cm)

Height offset is applied to the XR Origin's Camera Y Offset and persists to PlayerPrefs.

## Common Use Cases

### Basic Toggle
```csharp
// Just listen to InputAction - handled automatically
// FloatingMenuManager subscribes to the action
```

### Manual Show/Hide
```csharp
// From code (e.g., pause game and show menu)
FloatingMenuManager.Instance.ShowMenu();

// On level start
FloatingMenuManager.Instance.HideMenu();
```

### Read Height Setting
```csharp
// Get player's preferred height offset
float heightOffset = HeightAdjuster.Instance.GetHeightOffset();
```

## Prefab Hierarchy

```
FloatingMenuPanel
├── Canvas (World Space, RenderMode: WorldSpace)
├── CanvasGroup (alpha for fading)
├── FloatingText (positioning - from Dialogue Demo)
├── FloatingMenuPanel.cs
└── Panel/
    ├── Background (Image)
    ├── TitleText (TMP)
    ├── HeightSlider/
    │   ├── SliderLabel (TMP)
    │   ├── Slider (UI.Slider)
    │   └── ValueText (TMP)
    └── CloseButton (Button - optional)
```

## Troubleshooting

**Menu doesn't toggle:**
- Check InputActionReference is assigned and enabled
- Verify XRI Default Input Actions asset is in project
- Test with `Debug.Log` in OnMenuToggle

**Menu doesn't face player:**
- Check FloatingText component is assigned
- Verify Camera.main returns XR camera
- Check FloatingText's isBillboard is true

**Height slider doesn't work:**
- Verify XR Origin reference is assigned
- Check slider min/max values
- Ensure CameraYOffset is accessible on XR Origin

**Can't interact with UI:**
- Ensure Canvas has XR Ray Interactable or TrackedDeviceGraphicRaycaster
- Check Event Camera is set to XR camera

## Testing

**Editor Testing:**
- Use XR Device Simulator
- Press mapped key for left menu button
- Verify menu appears in front of camera

**VR Testing:**
- Build and deploy to headset
- Press Y button (Quest) or menu button
- Verify:
  - Menu appears in view
  - Menu follows head rotation
  - Slider is interactable with ray
  - Height changes apply correctly

## Future Enhancements

- [ ] Palm-up gesture for hand tracking
- [ ] Sound effects
- [ ] Haptic feedback
- [ ] Additional settings (turn speed, vignette, etc.)
- [ ] Wrist-mounted mini menu option
