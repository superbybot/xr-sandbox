# VR Dialogue Demo - Quick Reference

## 🚀 Quick Start

```csharp
using App.Demos.DialogueDemo.Scripts;

// Show a prompt
PromptManager.ShowPrompt("Hello VR!");

// Show with custom duration
PromptManager.ShowPrompt("5 second message", 5f);

// Hide current prompt
PromptManager.HidePrompt();
```

## 📦 Components

| Component | Purpose |
|-----------|---------|
| **FloatingText** | Positions text in world space, faces camera |
| **VRPrompt** | Manages text display and fade animations |
| **PromptManager** | Singleton with static API and queue system |
| **PromptTrigger** | Example trigger component (collision, interaction, etc.) |
| **DialogueDemoController** | Scene controller for welcome/tutorial sequences |

## 🎯 Trigger Types

```csharp
public enum TriggerType
{
    OnCollision,    // Player touches object
    OnInteraction,  // Player grabs/selects object
    OnProximity,    // Player gets close (2m)
    OnStart,        // Scene starts
    Timed,          // After delay
    Manual          // Call TriggerPrompt() from code
}
```

## ⚙️ Configuration

### FloatingText Settings
- **Distance From Camera**: 2m (default)
- **Fixed Size**: 0.002 (adjust if text too small/large)
- **Max Distance**: 50m (hide beyond this)
- **Offset**: (0, 0.5, 0) (vertical offset)

### VRPrompt Settings
- **Display Duration**: 3s (how long to show)
- **Fade Duration**: 0.5s (fade in/out speed)

### PromptManager Settings
- **Use Queue**: true (queue prompts sequentially)
- **Delay Between Prompts**: 0.5s (pause between queued prompts)

## 📝 Common Usage Patterns

### Welcome Message
```csharp
void Start()
{
    PromptManager.ShowPrompt("Welcome to VR!", 4f);
}
```

### Tutorial Sequence
```csharp
void ShowTutorial()
{
    // These queue automatically
    PromptManager.ShowPrompt("Step 1: Look around");
    PromptManager.ShowPrompt("Step 2: Grab objects");
    PromptManager.ShowPrompt("Step 3: Complete task");
}
```

### Contextual Hints
```csharp
void OnPlayerEnterArea()
{
    PromptManager.ShowPrompt("Danger zone ahead!", 3f);
}

void OnItemPickup(string itemName)
{
    PromptManager.ShowPrompt($"Picked up: {itemName}", 2f);
}
```

### Achievement Notifications
```csharp
void OnAchievementUnlocked(string name)
{
    PromptManager.ShowPrompt($"🎉 Achievement: {name}", 4f);
}
```

## 🔧 Prefab Setup (Unity Editor)

1. **Create GameObject** with:
   - Canvas (World Space, scale 0.002)
   - CanvasGroup (alpha 0)
   - FloatingText script
   - VRPrompt script

2. **Add UI Children**:
   - Panel (1200x300)
   - TextMeshProUGUI (font size 120, centered)

3. **Assign References** in VRPrompt:
   - Prompt Text → TextMeshProUGUI
   - Canvas Group → CanvasGroup
   - Floating Text → FloatingText

4. **Save as Prefab** in `Prefabs/` folder

## 🎬 Scene Setup (Unity Editor)

1. Add **XR Origin** (VR rig)
2. Create **PromptManager** GameObject
3. Add **VRPromptBox** prefab as child
4. Assign VRPromptBox to PromptManager's "Default Prompt"
5. (Optional) Add **DialogueDemoController** for welcome/tutorial

## 🐛 Troubleshooting

| Problem | Solution |
|---------|----------|
| Prompt doesn't appear | Check PromptManager has VRPromptBox assigned |
| Text too small/large | Adjust FloatingText `fixedSize` (0.001-0.003) |
| Prompts overlap | Enable `useQueue` in PromptManager |
| Collision trigger not working | Check collider has "Is Trigger" enabled |
| Interaction trigger not working | Add XRGrabInteractable component |

## 📚 Files

- **Scripts**: `Assets/App/Demos/Dialogue Demo/Scripts/`
- **Setup Guide**: `Assets/App/Demos/Dialogue Demo/SCENE_SETUP.md`
- **README**: `Assets/App/Demos/Dialogue Demo/README.md`
- **Walkthrough**: See artifacts folder

## 🎨 Customization

### Change Text Style
Edit TextMeshProUGUI in prefab:
- Font, size, color, alignment

### Change Background
Replace Image sprite in UI Panel

### Adjust Position
Modify FloatingText settings:
- `distanceFromCamera` - closer/further
- `offset` - vertical/horizontal adjustment

### Animation Timing
Modify VRPrompt settings:
- `displayDuration` - how long prompts show
- `fadeDuration` - speed of fade in/out

## 💡 Tips

- Keep messages **short** (VR users can't read long text)
- Use **2-4 second** duration for most prompts
- **Don't spam** - use queue system
- **Test in VR** - looks different than editor
- Use **readable fonts** and good contrast

## 🔗 API Reference

```csharp
// Static methods (call from anywhere)
PromptManager.ShowPrompt(string message);
PromptManager.ShowPrompt(string message, float duration);
PromptManager.HidePrompt();
PromptManager.ClearQueue();
bool isShowing = PromptManager.IsShowingPrompt();

// Instance methods (on VRPrompt component)
await vrPrompt.ShowPromptAsync(string message, float? duration);
await vrPrompt.HidePromptAsync();
bool isShowing = vrPrompt.IsShowing();

// PromptTrigger methods
promptTrigger.TriggerPrompt();  // Manual trigger
promptTrigger.ResetTrigger();   // Allow re-trigger
```

## 🚦 Next Steps

1. Follow **SCENE_SETUP.md** to create demo scene
2. Test in Unity Editor
3. Build and test in VR headset
4. Customize for your project
5. Integrate with game systems
