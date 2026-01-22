# HUD & Countdown Dialogue Extensions - Implementation Plan

This document outlines the plan to extend the existing VR Dialogue Demo system with two new features:
1. **VR HUD Display** - A persistent, always-available UI overlay that follows the user
2. **Countdown Dialogue** - A specialized countdown variant for racing games (3, 2, 1, GO!)

---

## Overview

Both features build upon the existing dialogue system architecture (`FloatingText`, `VRPrompt`, `PromptManager`) but serve different use cases:

| Feature | Use Case | Visibility | Auto-Hide |
|---------|----------|------------|-----------|
| **Prompt** (existing) | Temporary notifications, tutorials | On-demand | Yes (timer) |
| **VR HUD** | Persistent game info, status displays | Always visible (toggle) | No |
| **Countdown** | Race start, game begins | On-demand | Yes (auto-sequence) |

---

## Feature 1: VR HUD Display

### Description

A permanent UI element that follows the user's view, similar to the dialogue prompt but designed to always be available. Perfect for:
- Lap counter in racing games
- Score display
- Health/status indicators
- Timer displays
- Any persistent game information

### Key Differences from Existing Prompt

| Aspect | VRPrompt (Existing) | VRHUD (New) |
|--------|---------------------|-------------|
| Purpose | Temporary messages | Persistent display |
| Auto-hide | Yes (timed) | No |
| Visibility control | Show/hide per message | Toggle on/off via code |
| Content | Text only | Text + optional icon/value |
| Queue support | Yes | N/A (always visible) |

### Proposed Architecture

```
New Files:
├── Scripts/
│   ├── VRHUD.cs              # Main HUD component
│   └── HUDManager.cs         # Singleton manager (like PromptManager)
├── Prefabs/
│   └── VRHUDBox.prefab       # HUD prefab
```

### VRHUD.cs - Component Design

```csharp
public class VRHUD : MonoBehaviour
{
    // UI References
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private FloatingText floatingText;
    
    // HUD-specific settings
    [SerializeField] private bool startVisible = false;
    [SerializeField] private float fadeDuration = 0.3f;
    
    // Public API
    public void Show();                           // Fade in
    public void Hide();                           // Fade out
    public void SetText(string text);             // Update text content
    public void SetTextImmediate(string text);    // Update without animation
    public bool IsVisible { get; }                // Check visibility state
    public void Toggle();                         // Toggle visibility
}
```

### HUDManager.cs - Singleton Design

```csharp
public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; }
    
    // Multiple HUD support (named HUDs for different purposes)
    private Dictionary<string, VRHUD> registeredHUDs;
    
    // Default HUD
    public static void Show();
    public static void Hide();
    public static void SetText(string text);
    public static void Toggle();
    
    // Named HUD support (for multiple HUDs)
    public static void Show(string hudId);
    public static void SetText(string hudId, string text);
    
    // Factory method
    public static VRHUD CreateHUD(string id, Vector3? offset = null);
}
```

### Usage Examples

```csharp
// Show lap counter
HUDManager.Show();
HUDManager.SetText("Lap 1 / 3");

// Update during gameplay
HUDManager.SetText("Lap 2 / 3");

// Hide when race ends
HUDManager.Hide();

// Toggle with controller button
void OnMenuButtonPressed()
{
    HUDManager.Toggle();
}
```

### Prefab Structure

```
VRHUDBox
├── Canvas (World Space, scale 0.002)
├── CanvasGroup (alpha 0)
├── FloatingText (positioning)
├── VRHUD (management)
└── UI
    ├── Background (Image, smaller/simpler than prompt)
    └── DisplayText (TextMeshProUGUI)
```

### Configuration Options

| Setting | Default | Description |
|---------|---------|-------------|
| `startVisible` | false | Show HUD on scene start |
| `fadeDuration` | 0.3s | Fade in/out animation time |
| `distanceFromCamera` | 1.8m | Closer than prompts for easy reading |
| `offset` | (0.3, 0.2, 0) | Offset to corner (not center) |
| `fixedSize` | 0.0015 | Smaller than prompts |

---

## Feature 2: Countdown Dialogue

### Description

A specialized dialogue variant that displays a countdown sequence (3, 2, 1, GO!) with optional callbacks. Designed for:
- Racing game starts
- Mini-game introductions
- Timed challenge beginnings
- Any countdown scenario

### Key Features

- **Automatic sequence** - 3 → 2 → 1 → GO!
- **Customizable numbers/text** - Can be any sequence
- **Callback support** - OnCountdownComplete event
- **Different visual style** - Larger, centered numbers
- **Sound integration ready** - Hook for audio cues

### Proposed Architecture

```
New Files:
├── Scripts/
│   └── CountdownPrompt.cs    # Countdown-specific component
├── Prefabs/
│   └── VRCountdownBox.prefab # Countdown prefab (larger text)
```

### CountdownPrompt.cs - Component Design

```csharp
public class CountdownPrompt : MonoBehaviour
{
    // UI References
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private FloatingText floatingText;
    
    // Countdown Settings
    [SerializeField] private string[] defaultSequence = { "3", "2", "1", "GO!" };
    [SerializeField] private float intervalTime = 1f;          // Time between numbers
    [SerializeField] private float goDisplayTime = 0.5f;       // How long "GO!" shows
    [SerializeField] private float fadeDuration = 0.2f;
    
    // Animation Settings
    [SerializeField] private bool scaleOnEachNumber = true;    // Punch scale effect
    [SerializeField] private float scaleMultiplier = 1.2f;
    
    // Events
    public event System.Action OnCountdownComplete;
    public event System.Action<string> OnNumberChanged;        // Called each step
    
    // Public API
    public async UniTask StartCountdownAsync();                 // Default 3,2,1,GO
    public async UniTask StartCountdownAsync(string[] sequence);// Custom sequence
    public async UniTask StartCountdownAsync(int from);         // Start from number
    public void CancelCountdown();                             // Stop mid-countdown
    public bool IsCountingDown { get; }
}
```

### Integration with PromptManager

Add static convenience method to `PromptManager`:

```csharp
// In PromptManager.cs
public static async UniTask ShowCountdownAsync()
{
    await Instance?.countdownPrompt.StartCountdownAsync();
}

public static async UniTask ShowCountdownAsync(int from)
{
    await Instance?.countdownPrompt.StartCountdownAsync(from);
}

public static async UniTask ShowCountdownAsync(string[] sequence)
{
    await Instance?.countdownPrompt.StartCountdownAsync(sequence);
}
```

### Usage Examples

```csharp
// Basic countdown (3, 2, 1, GO!)
await PromptManager.ShowCountdownAsync();
StartRace();

// Custom start number
await PromptManager.ShowCountdownAsync(5);  // 5, 4, 3, 2, 1, GO!

// Custom sequence
await PromptManager.ShowCountdownAsync(new[] { "Ready?", "Set!", "GO!" });

// With callback
var countdown = PromptManager.GetCountdownPrompt();
countdown.OnCountdownComplete += () => StartRace();
countdown.StartCountdownAsync().Forget();
```

### Racing Game Integration Example

```csharp
public class RaceController : MonoBehaviour
{
    public async void StartRaceSequence()
    {
        // Show "Get Ready!"
        PromptManager.ShowPrompt("Get Ready!", 2f);
        await UniTask.WaitForSeconds(2f);
        
        // Show countdown
        await PromptManager.ShowCountdownAsync();
        
        // Race starts!
        EnablePlayerControls();
        StartLapTimer();
    }
}
```

### Prefab Structure

```
VRCountdownBox
├── Canvas (World Space, scale 0.003)     # Larger than prompt
├── CanvasGroup (alpha 0)
├── FloatingText (positioning)
├── CountdownPrompt (management)
└── UI
    ├── Background (Image, circular/radial style)
    └── CountdownText (TextMeshProUGUI, large centered font)
```

### Visual Design Suggestions

- **Larger font size** - 200+ for easy reading
- **Bold/impact font** - Racing feel
- **Centered text** - Numbers in center of view
- **Optional: Color changes** - Red → Yellow → Green
- **Optional: Pulsing animation** - Scale punch on each number

---

## Implementation Order

### Phase 1: VR HUD System
1. [ ] Create `VRHUD.cs` script
2. [ ] Create `HUDManager.cs` singleton
3. [ ] Create `VRHUDBox.prefab`
4. [ ] Add documentation
5. [ ] Test in Dialogue Demo scene

### Phase 2: Countdown System
1. [ ] Create `CountdownPrompt.cs` script
2. [ ] Create `VRCountdownBox.prefab`
3. [ ] Integrate with `PromptManager`
4. [ ] Add documentation
5. [ ] Test in Dialogue Demo scene

### Phase 3: Racing Demo Integration
1. [ ] Integrate HUD for lap counter
2. [ ] Integrate countdown for race start
3. [ ] Test complete race flow

---

## File Changes Summary

### New Files

| File | Purpose |
|------|---------|
| `Scripts/VRHUD.cs` | Persistent HUD component |
| `Scripts/HUDManager.cs` | HUD singleton manager |
| `Scripts/CountdownPrompt.cs` | Countdown sequence component |
| `Prefabs/VRHUDBox.prefab` | HUD UI prefab |
| `Prefabs/VRCountdownBox.prefab` | Countdown UI prefab |

### Modified Files

| File | Changes |
|------|---------|
| `PromptManager.cs` | Add countdown convenience methods |
| `README.md` | Document new features |
| `QUICK_REFERENCE.md` | Add HUD and Countdown API reference |

---

## API Reference Summary

### VRHUD API
```csharp
HUDManager.Show();                    // Show default HUD
HUDManager.Hide();                    // Hide default HUD
HUDManager.SetText(string text);      // Update HUD content
HUDManager.Toggle();                  // Toggle visibility
HUDManager.Show(string hudId);        // Show specific HUD
HUDManager.SetText(string id, string text);
```

### Countdown API
```csharp
await PromptManager.ShowCountdownAsync();                    // 3, 2, 1, GO!
await PromptManager.ShowCountdownAsync(5);                   // 5, 4, 3, 2, 1, GO!
await PromptManager.ShowCountdownAsync(new[] { "A", "B" });  // Custom sequence

// Direct access
CountdownPrompt cp = PromptManager.GetCountdownPrompt();
cp.OnCountdownComplete += () => DoSomething();
```

---

## Estimated Effort

| Task | Estimated Time |
|------|----------------|
| VRHUD + HUDManager | 1-2 hours |
| VRHUDBox prefab | 30 minutes |
| CountdownPrompt | 1-2 hours |
| VRCountdownBox prefab | 30 minutes |
| Documentation | 30 minutes |
| Testing | 1 hour |
| **Total** | **4-6 hours** |

---

## Questions for Review

1. **HUD Position**: Should the HUD be offset to a corner (e.g., bottom-right) or centered like prompts? Corner positioning is more typical for HUDs.

2. **Multiple HUDs**: Do you need support for multiple HUDs simultaneously (e.g., lap counter + timer + speed)?

3. **Countdown Sound**: Should we prepare audio hooks for countdown sounds (beeps), or is that handled separately?

4. **Color Scheme**: Should the countdown change colors (red → yellow → green) or use a consistent style?

---

## References

- [FloatingText.cs](file:///d:/Unity%20Projects/xr-sandbox/xr-sandbox-app/Assets/App/Demos/Dialogue%20Demo/Scripts/FloatingText.cs) - Positioning logic
- [VRPrompt.cs](file:///d:/Unity%20Projects/xr-sandbox/xr-sandbox-app/Assets/App/Demos/Dialogue%20Demo/Scripts/VRPrompt.cs) - Animation patterns
- [PromptManager.cs](file:///d:/Unity%20Projects/xr-sandbox/xr-sandbox-app/Assets/App/Demos/Dialogue%20Demo/Scripts/PromptManager.cs) - Singleton pattern
- [README.md](file:///d:/Unity%20Projects/xr-sandbox/xr-sandbox-app/Assets/App/Docs/Dialogue%20Demo/README.md) - Existing documentation
