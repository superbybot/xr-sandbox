# North Star-Style Transition Implementation Guide

This document outlines how to implement a high-quality screen fade transition similar to the one used in the **Unity-NorthStar** project. Unlike simple UI overlays, this method uses **Post-Processing Volumes** for a smoother, more integrated look that works perfectly in VR.

## 1. Core Concept

The North Star transition uses a **Global Post-Processing Volume** to manipulate screen appearance.
- **Mechanism**: Controls the `weight` of a Volume containing a "Black Screen" effect (usually *Color Adjustments* with Exposure set to minimum or a black Color Filter).
- **Animation**: Uses a tweening library (like DOTween) to smoothly animate the weight from 0 (clear) to 1 (black).

## 2. Prerequisites

1.  **Render Pipeline**: Universal Render Pipeline (URP) or High Definition Render Pipeline (HDRP).
    *   *Note: The standard Built-in Pipeline can also use Post-Processing Stack v2, but the setup is slightly different.*
2.  **Tweening Library**: **DOTween** (used in North Star) or **UniTask** (used in this project).
    *   *Recommendation: Use UniTask to keep dependencies consistent with the rest of this project.*

## 3. Implementation Steps

### Step 1: Create the Fade Volume Profile
1.  Right-click in Project view → Create → **Volume Profile**. Name it `FadeProfile`.
2.  Add Override → **Color Adjustments**.
3.  Set **Color Filter** to **Black** (R:0, G:0, B:0).
4.  (Optional) Set **Exposure** to **-10** (or minimum) for extra darkness.
5.  Ensure all other overrides are disabled.

### Step 2: Setup the Scene Volume
1.  Create an Empty GameObject in the scene. Name it `GlobalFadeVolume`.
2.  Add component **Volume**.
3.  Assign the `FadeProfile` created in Step 1.
4.  Set **Weight** to `0` (Invisible).
5.  Set **Priority** to `100` (High) to ensure it overrides other volumes.
6.  Check **Is Global**.

### Step 3: Create the ScreenFader Script
Create a script named `ScreenFader.cs`. This will manage the volume's weight.

```csharp
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace App.Demos.CarDemo.Scripts
{
    public class ScreenFader : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private Volume fadeVolume;
        [SerializeField] private float defaultDuration = 0.5f;

        private void Awake()
        {
            if (fadeVolume == null)
                fadeVolume = GetComponent<Volume>();
                
            // Ensure we start clear
            if (fadeVolume != null)
                fadeVolume.weight = 0f;
        }

        public async UniTask FadeOut(float duration = -1f)
        {
            if (fadeVolume == null) return;
            
            float targetDuration = duration > 0 ? duration : defaultDuration;
            float elapsed = 0f;
            
            while (elapsed < targetDuration)
            {
                elapsed += Time.deltaTime;
                fadeVolume.weight = Mathf.Lerp(0f, 1f, elapsed / targetDuration);
                await UniTask.Yield();
            }
            fadeVolume.weight = 1f;
        }

        public async UniTask FadeIn(float duration = -1f)
        {
            if (fadeVolume == null) return;
            
            float targetDuration = duration > 0 ? duration : defaultDuration;
            float elapsed = 0f;
            
            while (elapsed < targetDuration)
            {
                elapsed += Time.deltaTime;
                fadeVolume.weight = Mathf.Lerp(1f, 0f, elapsed / targetDuration);
                await UniTask.Yield();
            }
            fadeVolume.weight = 0f;
        }
    }
}
```

### Step 4: Integration with Teleport Logic

Modify `CarTeleportAnchor.cs` to reference this `ScreenFader` instead of the old `VRScreenFade`.

```csharp
// In CarTeleportAnchor.cs

[SerializeField] private ScreenFader screenFader;

private async UniTaskVoid EnterCarAsync()
{
    // ...
    
    // Fade Out
    if (screenFader != null)
        await screenFader.FadeOut(0.3f);
        
    // Teleport Logic
    DisableLocomotion();
    
    // Wait briefly
    await UniTask.WaitForSeconds(0.1f);
    
    // Fade In
    if (screenFader != null)
        await screenFader.FadeIn(0.3f);
        
    // ...
}
```

## 4. Why This Approach?

*   **Professional Look**: Fading the entire render pipeline looks cleaner than overlaying a black quad or UI canvas.
*   **VR Compatible**: Works perfectly in VR because it affects the final rendered image for both eyes equally.
*   **Flexible**: You can easily change the fade color, add blur, or other effects just by modifying the Volume Profile.

## 5. North Star Reference
For the exact implementation used in the reference project, see:
*   `Assets/NorthStar/Scripts/Player/ScreenFader.cs`
*   `Assets/NorthStar/Scripts/Player/GrabTeleport.cs` (lines 380-400)
