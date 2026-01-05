# VR Dialogue Demo

A simple VR prompt/dialogue system for displaying floating text in VR, based on Unity-NorthStar's dialogue implementation.

## Overview

This demo provides a clean, easy-to-use system for showing text prompts to VR users. Prompts automatically position themselves in front of the user, face the camera, and scale appropriately based on distance.

## Features

- ✅ **User-facing prompts** - Text appears in front of the player
- ✅ **Smooth animations** - Fade in/out using UniTask
- ✅ **Queue system** - Multiple prompts display sequentially
- ✅ **Easy API** - Simple static methods for showing prompts
- ✅ **Multiple trigger types** - Collision, interaction, proximity, timed, manual
- ✅ **Character attachment** - Optional feature (commented out, ready to enable)

## Quick Start

### 1. Setup in Scene

1. Add the `VRPromptBox` prefab to your scene
2. Add a `PromptManager` component to a GameObject
3. Assign the VRPromptBox to the PromptManager's "Default Prompt" field

### 2. Show a Prompt from Code

```csharp
using App.Demos.DialogueDemo.Scripts;

// Show a prompt with default duration (3 seconds)
PromptManager.ShowPrompt("Hello, VR World!");

// Show a prompt with custom duration
PromptManager.ShowPrompt("This shows for 5 seconds", 5f);

// Hide the current prompt immediately
PromptManager.HidePrompt();
```

### 3. Using Triggers

Add a `PromptTrigger` component to any GameObject to trigger prompts based on:
- **Collision** - When player touches the object
- **Interaction** - When player grabs/selects the object
- **Proximity** - When player gets close
- **Timed** - After a delay
- **On Start** - When scene starts
- **Manual** - Call `TriggerPrompt()` from code

## Components

### FloatingText.cs
Handles positioning and scaling of text in world space.

**Key Settings:**
- `Distance From Camera` - How far in front of user (default: 2m)
- `Fixed Size` - Size multiplier (default: 0.002)
- `Max Distance` - Hide if further than this (default: 50m)
- `Target Position` - Optional transform to follow (for character attachment)

### VRPrompt.cs
Manages text display and fade animations.

**Key Settings:**
- `Display Duration` - How long to show (default: 3s)
- `Fade Duration` - Fade in/out time (default: 0.5s)

**Methods:**
```csharp
// Async version
await vrPrompt.ShowPromptAsync("Message");

// Fire-and-forget version
vrPrompt.ShowPrompt("Message");

// Hide
await vrPrompt.HidePromptAsync();

// Check if showing
bool isShowing = vrPrompt.IsShowing();
```

### PromptManager.cs
Singleton manager for global prompt access.

**Static Methods:**
```csharp
PromptManager.ShowPrompt(string message);
PromptManager.ShowPrompt(string message, float duration);
PromptManager.HidePrompt();
PromptManager.ClearQueue();
bool isShowing = PromptManager.IsShowingPrompt();
```

**Settings:**
- `Use Queue` - Queue prompts or interrupt current one
- `Delay Between Prompts` - Pause between queued prompts

### PromptTrigger.cs
Example component for triggering prompts.

**Settings:**
- `Trigger Type` - How to trigger the prompt
- `Prompt Message` - Text to display
- `Custom Duration` - Override default duration (0 = use default)
- `Trigger Once` - Only trigger once, or allow repeats

## Character Attachment (Optional)

To attach prompts to characters instead of showing to the user:

### In VRPrompt.cs:
```csharp
// Uncomment these lines:
[Header("Character Attachment (Optional)")]
[SerializeField] private bool attachToCharacter = false;
[SerializeField] private Transform characterAttachPoint;

// And in Start():
if (attachToCharacter && characterAttachPoint != null && floatingText != null)
{
    floatingText.SetTarget(characterAttachPoint);
}
```

### From Code:
```csharp
// Get the prompt instance
VRPrompt prompt = PromptManager.GetDefaultPrompt();

// Attach to character's head transform
prompt.AttachToCharacter(characterHeadTransform);

// Detach and return to user-facing
prompt.DetachFromCharacter();
```

## Prefab Structure

```
VRPromptBox
├── Canvas (World Space)
├── CanvasGroup (for fading)
├── FloatingText (positioning)
├── VRPrompt (management)
└── UI
    ├── Background (Image)
    └── PromptText (TextMeshProUGUI)
```

## Example Usage Scenarios

### Welcome Message
```csharp
void Start()
{
    PromptManager.ShowPrompt("Welcome to the VR Experience!");
}
```

### Tutorial Hints
```csharp
void OnPlayerEnterArea()
{
    PromptManager.ShowPrompt("Use the trigger to grab objects");
}
```

### Sequential Instructions
```csharp
void ShowTutorial()
{
    PromptManager.ShowPrompt("Step 1: Look around");
    PromptManager.ShowPrompt("Step 2: Grab the cube");
    PromptManager.ShowPrompt("Step 3: Place it on the table");
    // These will show one after another thanks to the queue system
}
```

### Contextual Prompts
```csharp
void OnLookAtObject()
{
    PromptManager.ShowPrompt("This is a mysterious artifact", 2f);
}
```

## Customization

### Change Text Style
Edit the TextMeshProUGUI component in the prefab:
- Font
- Size
- Color
- Alignment
- Material

### Change Background
Replace the Background Image sprite in the prefab with your own design.

### Adjust Positioning
Modify FloatingText settings:
- `Distance From Camera` - Closer or further
- `Offset` - Vertical/horizontal adjustment
- `Fixed Size` - Larger or smaller text

### Animation Timing
Adjust VRPrompt settings:
- `Display Duration` - How long prompts show
- `Fade Duration` - Speed of fade in/out

## Dependencies

- **TextMeshPro** - For high-quality text rendering
- **UniTask** - For async/await animations
- **XR Interaction Toolkit** - For VR interaction (optional, only for PromptTrigger)

## Tips

1. **Keep messages short** - VR users can't read long text comfortably
2. **Use appropriate duration** - 2-4 seconds is usually good
3. **Don't spam prompts** - Use the queue system to avoid overwhelming users
4. **Test in VR** - Text positioning looks different in headset vs editor
5. **Consider accessibility** - Use readable fonts and good contrast

## Troubleshooting

**Prompt doesn't appear:**
- Check that PromptManager has a default prompt assigned
- Ensure Canvas is enabled
- Check that Camera.main is set correctly

**Text is too small/large:**
- Adjust `Fixed Size` in FloatingText
- Check the scale of the VRPromptBox GameObject

**Prompts overlap:**
- Enable `Use Queue` in PromptManager
- Increase `Delay Between Prompts`

**Character attachment not working:**
- Uncomment the character attachment code in VRPrompt.cs
- Assign a valid Transform to `characterAttachPoint`
- Ensure the transform is at the character's head position

## Future Enhancements

Ideas for extending this system:
- Multiple prompt styles (warning, info, success)
- Sound effects for prompts
- Choice buttons (A/B options)
- Text input with VR keyboard
- Localization support
- Animated entrance/exit effects
- Multiple simultaneous prompts at different positions

## Credits

Based on the dialogue system from **Unity-NorthStar** by Meta.
Adapted and simplified for general VR prompt usage.
