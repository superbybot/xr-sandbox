# VR Text Input & Dialogue System - NorthStar Reference

This document explains how to implement a quick text box/prompt/chatbox system in VR, based on the implementation from **Unity-NorthStar**.

## Overview

NorthStar uses a **floating dialogue/subtitle system** that displays text boxes above characters or in world space. The system is designed for VR and automatically positions text to stay visible and readable.

## Core Components

### 1. **Subtitle Component** (`Subtitle.cs`)
The main component that displays text in VR. It handles:
- Text display with fade in/out animations
- Auto-hiding after a configurable duration
- Registration with the CharacterManager

**Key Features:**
- Uses **TextMeshProUGUI** for high-quality text rendering
- **CanvasGroup** for smooth fade animations (using DOTween)
- **FloatingText** component for world-space positioning
- Configurable show/fade times via GlobalSettings

**Location:** `Assets/NorthStar/Scripts/Dialogue/Subtitle.cs`

```csharp
public class Subtitle : MonoBehaviour
{
    [CharacterDropdown] public string Id;  // Unique identifier for this subtitle
    private TextMeshProUGUI m_text;
    private CanvasGroup m_canvasGroup;
    private FloatingText m_floatingText;
    
    public void DisplayText(TextObject text)
    {
        m_text.text = text.Text;
        // Fade in
        m_canvasGroup.DOFade(1, GlobalSettings.ScreenSettings.TextFadeTime);
        m_timeLastDisplayed = Time.time;
        m_floatingText.SyncPosition();
    }
}
```

### 2. **FloatingText Component** (`FloatingText.cs`)
Handles the world-space positioning logic to keep text visible and readable in VR.

**Key Features:**
- Automatically faces the camera
- Stays within screen bounds (clamped to viewport)
- Scales based on distance from camera
- Hides when too far away or behind the player
- Smooth interpolation for position changes

**Location:** `Assets/NorthStar/Scripts/Dialogue/FloatingText.cs`

**Important Settings:**
- `m_targetPosition`: Transform to follow (e.g., character's head)
- `m_fixedSize`: Size multiplier (0.000003 for dialogue boxes)
- `m_maxDistance`: Maximum visibility distance (50 units)
- `m_bottomLeft` / `m_topRight`: Bounds for screen clamping

### 3. **CharacterManager** (Singleton)
Manages all subtitle objects and routes dialogue to the correct character.

**Location:** `Assets/NorthStar/Scripts/Dialogue/CharacterManager.cs`

```csharp
public class CharacterManager
{
    public static CharacterManager Instance { get; } = new();
    
    private Dictionary<string, Subtitle> m_keySubtitlePairs = new();
    
    public void RegisterSubtitleObject(Subtitle subtitle)
    {
        m_keySubtitlePairs[subtitle.Id] = subtitle;
    }
    
    public void PlayDialogue(string id, TextObject textObject)
    {
        if (m_keySubtitlePairs.TryGetValue(id, out var subtitle))
        {
            subtitle.DisplayText(textObject);
        }
    }
}
```

### 4. **DialoguePlayer** 
Orchestrates dialogue sequences with timeline integration.

**Location:** `Assets/NorthStar/Scripts/Dialogue/DialoguePlayer.cs`

```csharp
public class DialoguePlayer : MonoBehaviour
{
    [SerializeField] private DialogueSection m_dialogueSection;
    [SerializeField] private PlayableDirector m_playableDirector;
    
    public void AdvanceDialogue()
    {
        CharacterManager.PlayDialogue(
            m_dialogueSection.TextObjects[m_dialogueIndex].CharacterId,
            m_dialogueSection.TextObjects[m_dialogueIndex].Text
        );
        m_dialogueIndex++;
    }
}
```

## UI Prefab Structure

**Location:** `Assets/NorthStar/Prefabs/UI/DialougeObject.prefab`

The dialogue prefab has this hierarchy:

```
DialougeObject (Root)
├── Canvas (World Space, RenderMode: 2)
├── CanvasGroup (for fading)
├── FloatingText (positioning script)
├── Subtitle (dialogue management)
└── RectTransform
    ├── Text (Container)
    │   ├── Image (Background - Textbox.png)
    │   └── Text (TMP) (TextMeshProUGUI)
    ├── Icon (Character portrait area)
    │   ├── RingBackground (decorative)
    │   ├── Foreground (decorative ring)
    │   └── Mask (for circular portrait)
    │       └── Icon (character image)
    └── RopeRingBackground (decorative element)
```

**Key Settings:**
- **Canvas:** World Space, scale 0.002
- **RectTransform:** Size 1144x287 pixels
- **TextMeshPro:** Font size 142.1, word wrapping enabled
- **Background:** Uses sliced sprite "Textbox.png"

## Visual Assets

**Location:** `Assets/NorthStar/UI/Subtitles/`

- `Textbox.png` - Main dialogue box background
- `RopeRing.png` - Decorative ring around character portrait
- `HeadshotMask.png` - Circular mask for character portraits
- Character portraits: `AudreyFront.png`, `BessieFront.png`, `ThomasFront.png`

## Implementation Steps

### Step 1: Create the Dialogue Prefab

1. Create a GameObject with:
   - **Canvas** (World Space)
   - **CanvasGroup**
   - **FloatingText** script
   - **Subtitle** script

2. Add child UI elements:
   - Background Image (sliced sprite)
   - TextMeshProUGUI for the text content
   - (Optional) Character portrait with mask

3. Configure FloatingText:
   - Set `m_targetPosition` to the transform it should follow
   - Set `m_fixedSize` to 0.000003 (or adjust for your scale)
   - Set `m_maxDistance` to 50

### Step 2: Display Dialogue

```csharp
// Simple usage
CharacterManager.Instance.PlayDialogue("CharacterID", textObject);

// Or create a quick prompt
public class QuickPrompt : MonoBehaviour
{
    [SerializeField] private Subtitle subtitle;
    
    public void ShowPrompt(string message)
    {
        var textObject = ScriptableObject.CreateInstance<TextObject>();
        // Set text directly (or use localization system)
        subtitle.DisplayText(textObject);
    }
}
```

### Step 3: Position the Dialogue

The `FloatingText` component automatically:
- Follows the target transform
- Faces the camera
- Stays within screen bounds
- Scales based on distance

## Quick Chatbox/Prompt Implementation

For a simple VR prompt or chatbox (without the full dialogue system):

```csharp
using TMPro;
using UnityEngine;
using DG.Tweening;

public class VRPrompt : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Transform targetPosition;
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private float fadeDuration = 0.5f;
    
    private Camera mainCamera;
    
    void Start()
    {
        mainCamera = Camera.main;
        canvasGroup.alpha = 0;
    }
    
    void Update()
    {
        // Face camera
        transform.rotation = mainCamera.transform.rotation;
        
        // Follow target if set
        if (targetPosition != null)
        {
            transform.position = targetPosition.position;
        }
    }
    
    public void ShowPrompt(string message)
    {
        promptText.text = message;
        
        // Fade in
        canvasGroup.DOKill();
        canvasGroup.DOFade(1, fadeDuration).OnComplete(() =>
        {
            // Auto-hide after duration
            DOVirtual.DelayedCall(displayDuration, () =>
            {
                canvasGroup.DOFade(0, fadeDuration);
            });
        });
    }
}
```

## Key Takeaways

1. **World Space Canvas** - Essential for VR text display
2. **FloatingText** - Handles complex positioning logic to keep text visible
3. **TextMeshProUGUI** - Better quality than legacy UI Text
4. **DOTween** - Smooth fade animations (or use UniTask/Coroutines)
5. **Character Manager** - Centralized system for managing multiple dialogue sources
6. **Auto-positioning** - Text automatically faces camera and stays in view

## Differences from Desktop UI

- **No Input Fields**: NorthStar doesn't include text *input* (keyboard), only text *display*
- **World Space**: All UI is in world space, not screen overlay
- **Distance-based scaling**: Text size adjusts based on distance
- **Viewport clamping**: Text stays within visible screen bounds
- **Auto-rotation**: Always faces the camera

## For Text Input (Keyboard)

If you need actual text input (typing), you'll need to implement a VR keyboard separately. NorthStar focuses on *displaying* dialogue, not capturing input. For VR keyboards, consider:

1. **Meta's System Keyboard** - Native Quest keyboard (requires platform-specific code)
2. **Custom Virtual Keyboard** - UI buttons for each key, interactable with ray/poke
3. **Voice Input** - Using Meta's Voice SDK (see VoiceSDK samples)

## References

- **NorthStar Dialogue Scripts:** `Assets/NorthStar/Scripts/Dialogue/`
- **UI Prefabs:** `Assets/NorthStar/Prefabs/UI/`
- **Visual Assets:** `Assets/NorthStar/UI/Subtitles/`
- **Character-specific variants:** `AudreyDialougeObject.prefab`, `BessieDialougeObject.prefab`, `ThomasDialougeObject.prefab`
