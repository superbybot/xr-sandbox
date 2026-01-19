# VR Screen Fade Transition Demo - Quick Reference

## Overview

Simple VR screen fade system using Post-Processing Volumes for guaranteed full-screen coverage.

## Core Scripts

### ScreenFader.cs
Main fade controller (Singleton)

**Key Methods:**
```csharp
// Async
await ScreenFader.Instance.FadeToBlackAsync(duration);
await ScreenFader.Instance.FadeToClearAsync(duration);
await ScreenFader.Instance.FadeToWhiteAsync(duration);
await ScreenFader.Instance.FadeOutAndInAsync(fadeOut, hold, fadeIn);

// Fire-and-forget
ScreenFader.Instance.FadeToBlack(duration);
ScreenFader.Instance.FadeToClear(duration);

// Instant
ScreenFader.Instance.SetFadeImmediate(fadeValue);
ScreenFader.Instance.ClearFadeImmediate();
```

### TransitionTrigger.cs
Event-based fade triggers

**Trigger Types:**
- `OnStart` - Fade on scene start
- `OnCollision` - Fade when player touches
- `OnInteraction` - Fade when grabbed/interacted
- `OnProximity` - Fade when player enters area
- `Timed` - Fade after delay
- `Manual` - Call `TriggerFade()` from code

**Fade Types:**
- `FadeOut` - Fade to black
- `FadeIn` - Fade to clear
- `FadeOutAndIn` - Fade out, hold, fade in
- `FadeToWhite` - Flashbang effect

### TransitionDemoController.cs
Demo controller with example sequences

**Features:**
- Welcome fade-in sequence
- Automated demo sequence
- Example use case methods

## Setup Steps

### 1. Create Volume Profile
1. Right-click in `Assets/App/Demos/Transition Demo/`
2. Create → Rendering → URP Volume Profile
3. Name it `FadeVolumeProfile`
4. Add Override → Post-processing → Color Adjustments
5. Set Post Exposure to 0
6. Disable all other overrides

### 2. Create ScreenFadeVolume Prefab
1. Create empty GameObject, name it `ScreenFadeVolume`
2. Add Component → Volume
   - Profile: FadeVolumeProfile
   - Mode: Global
   - Weight: 0
   - Priority: 100
3. Add Component → ScreenFader
   - Fade Volume: (drag Volume component)
   - Default Fade Duration: 1.0
   - Fade To Black Value: -10
   - Fade To White Value: 10
4. Drag to Prefabs folder

### 3. Add to Scene
1. Drag ScreenFadeVolume prefab into scene
2. Position at (0, 0, 0)

### 4. Add Demo Controller (Optional)
1. Select ScreenFadeVolume
2. Add Component → TransitionDemoController
3. Configure welcome sequence settings

### 5. Create Example Triggers
See SCENE_SETUP.md for detailed trigger examples

## Common Use Cases

### Teleport with Fade
```csharp
await ScreenFader.Instance.FadeOutAndInAsync(0.5f, 0.1f, 0.5f);
// Move player during the hold period
```

### Scene Transition
```csharp
await ScreenFader.Instance.FadeToBlackAsync(1.5f);
// Load new scene
await ScreenFader.Instance.FadeToClearAsync(1.5f);
```

### Death/Respawn
```csharp
await ScreenFader.Instance.FadeToBlackAsync(0.5f);
await UniTask.WaitForSeconds(1f);
await ScreenFader.Instance.FadeToClearAsync(1f);
```

### Flashbang Effect
```csharp
ScreenFader.Instance.SetFadeImmediate(10f); // Instant white
await ScreenFader.Instance.FadeToClearAsync(2f);
```

## Troubleshooting

**Fade not working:**
- Check ScreenFader.Instance is not null
- Verify Volume Profile has ColorAdjustments override
- Check Volume component is on same GameObject as ScreenFader

**Gaps in VR peripheral vision:**
- This shouldn't happen with Post-Processing Volume approach
- If it does, increase fadeToBlackValue (make more negative)

**Performance issues:**
- Volume weight automatically sets to 0 when not fading
- Disable other Volume overrides you don't need
- Check only one ScreenFadeVolume exists in scene

## Testing

**Context Menu Functions:**
- Right-click ScreenFader component → Test: Fade to Black/Clear/White
- Right-click TransitionDemoController → Play Welcome/Demo Sequence

**In VR:**
- Build to headset
- Verify full peripheral coverage
- Test all trigger types
- Monitor frame rate during fades
