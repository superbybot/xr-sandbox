# VR Screen Fade Transition Demo - Quick Reference

## Overview

Simple VR screen fade system using Post-Processing Volumes for guaranteed full-screen coverage.

## Core Scripts

### ScreenFader.cs
Main fade controller

**Key Methods:**
```csharp
// Async
await screenFader.FadeToBlackAsync(duration);
await screenFader.FadeToClearAsync(duration);
await screenFader.FadeToWhiteAsync(duration);
await screenFader.FadeOutAndInAsync(fadeOut, hold, fadeIn);

// Fire-and-forget
screenFader.FadeToBlack(duration);
screenFader.FadeToClear(duration);

// Instant
screenFader.SetFadeImmediate(fadeValue);
screenFader.ClearFadeImmediate();
```

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
3. **Drag ScreenFader component to the Screen Fader field**
4. Configure welcome sequence settings

## Common Use Cases

### Teleport with Fade
```csharp
await screenFader.FadeOutAndInAsync(0.5f, 0.1f, 0.5f);
// Move player during the hold period
```

### Scene Transition
```csharp
await screenFader.FadeToBlackAsync(1.5f);
// Load new scene
await screenFader.FadeToClearAsync(1.5f);
```

### Death/Respawn
```csharp
await screenFader.FadeToBlackAsync(0.5f);
await UniTask.WaitForSeconds(1f);
await screenFader.FadeToClearAsync(1f);
```

### Flashbang Effect
```csharp
screenFader.SetFadeImmediate(10f); // Instant white
await screenFader.FadeToClearAsync(2f);
```

## Troubleshooting

**Fade not working:**
- Check ScreenFader reference is assigned
- Verify Volume Profile has ColorAdjustments override
- Check Volume component is on same GameObject as ScreenFader

**Gaps in VR peripheral vision:**
- This shouldn't happen with Post-Processing Volume approach
- If it does, increase fadeToBlackValue (make more negative)

## Testing

**Context Menu Functions:**
- Right-click ScreenFader component → Test: Fade to Black/Clear/White
- Right-click TransitionDemoController → Play Welcome/Demo Sequence

**In VR:**
- Build to headset
- Verify full peripheral coverage
- Monitor frame rate during fades
