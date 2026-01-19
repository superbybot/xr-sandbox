# VR Screen Fade Transition Demo - Scene Setup Guide

## Overview

This guide walks you through setting up the complete VR Screen Fade Transition Demo scene in Unity Editor. Follow these steps to create a working demo with all example triggers.

## Prerequisites

- Unity project with XR Interaction Toolkit installed
- URP (Universal Render Pipeline) configured
- UniTask package installed
- All Transition Demo scripts compiled without errors

## Step 1: Create the Demo Scene

1. In Unity, go to **File → New Scene**
2. Choose **Basic (Built-in)** or **Basic (URP)** depending on your render pipeline
3. Save the scene as `Transition Demo Scene.unity` in:
   ```
   Assets/App/Demos/Transition Demo/Scenes/
   ```

## Step 2: Create FadeVolumeProfile Asset

### Create the Volume Profile

1. In the Project window, navigate to `Assets/App/Demos/Transition Demo/`
2. Right-click → **Create → Rendering → URP Volume Profile**
3. Rename it to `FadeVolumeProfile`

### Configure the Profile

1. Select `FadeVolumeProfile` in the Project window
2. In the Inspector, click **Add Override → Post-processing → Color Adjustments**
3. Configure Color Adjustments:
   - Check the checkbox next to **Post Exposure** to enable override
   - Set **Post Exposure** value to `0`
4. **Important**: Do NOT add any other overrides (for performance)

## Step 3: Create ScreenFadeVolume Prefab

### Create the GameObject

1. In the Hierarchy, create a new **GameObject** (right-click → Create Empty)
2. Rename it to `ScreenFadeVolume`
3. Reset its transform (position 0,0,0, rotation 0,0,0, scale 1,1,1)

### Add Volume Component

1. Select `ScreenFadeVolume`
2. Add Component → **Volume** (from Rendering)
3. Configure Volume:
   - **Mode**: Global
   - **Profile**: Drag `FadeVolumeProfile` asset here
   - **Weight**: 0 (will be animated at runtime)
   - **Priority**: 100

### Add ScreenFader Script

1. With `ScreenFadeVolume` selected
2. Add Component → **Screen Fader** (from Transition Demo Scripts)
3. Configure:
   - **Fade Volume**: Drag the Volume component here
   - **Default Fade Duration**: 1.0
   - **Fade To Black Value**: -10
   - **Fade To White Value**: 10
   - **Enable Debug Logs**: Unchecked (check for testing)

### Save as Prefab

1. Drag `ScreenFadeVolume` from Hierarchy to:
   ```
   Assets/App/Demos/Transition Demo/Prefabs/
   ```
2. Keep the instance in the Hierarchy (don't delete it)

## Step 4: Set Up the Scene

### Add XR Origin

**Option A: Copy from existing demo**
1. Open `Dialogue Demo Scene.unity` or `Car Demo Scene.unity`
2. Find the XR Origin GameObject
3. Copy it (Ctrl+C)
4. Open `Transition Demo Scene.unity`
5. Paste it (Ctrl+V)

**Option B: Create new XR Origin**
1. Right-click in Hierarchy → **XR → XR Origin (Action-based)**
2. Configure as needed for your project

### Create Environment

1. Create a **Floor**:
   - Right-click in Hierarchy → **3D Object → Plane**
   - Rename to `Floor`
   - Position: (0, 0, 0)
   - Scale: (5, 1, 5) for a 50x50m floor

2. Create **Walls** (optional):
   - Right-click in Hierarchy → **3D Object → Cube**
   - Rename to `Wall_North`
   - Position: (0, 2.5, 25)
   - Scale: (50, 5, 0.5)
   - Duplicate for other walls if desired

### Add TransitionDemoController (Optional)

1. Select `ScreenFadeVolume` GameObject
2. Add Component → **Transition Demo Controller**
3. Configure:
   - **Play Welcome Sequence**: checked
   - **Welcome Delay**: 0.5
   - **Auto Start Demo Sequence**: unchecked (enable for automated demo)
   - **Demo Sequence Delay**: 5
   - **Enable Debug Logs**: checked

## Step 5: Create Example Triggers

### Example 1: Welcome Fade-In (OnStart)

1. Create an empty GameObject (right-click → Create Empty)
2. Rename to `1_WelcomeSequence`
3. Add Component → **Transition Trigger**
4. Configure:
   - **Trigger Type**: OnStart
   - **Fade Type**: FadeIn
   - **Fade Duration**: 2
   - **Trigger Once**: checked

**Note**: This will fade in from black when the scene starts. Make sure to set the initial fade state in TransitionDemoController.

### Example 2: Collision Trigger

1. Create a **Cube** (3D Object → Cube)
2. Rename to `2_CollisionExample`
3. Position: (2, 1, 3) - in front and to the right of spawn
4. Scale: (0.5, 0.5, 0.5)
5. Add Component → **Box Collider**
   - **Is Trigger**: checked
6. Add Component → **Transition Trigger**
7. Configure:
   - **Trigger Type**: OnCollision
   - **Fade Type**: FadeOutAndIn
   - **Fade Duration**: 0.5
   - **Hold Duration**: 0.2
   - **Trigger Once**: checked

8. Create a material for visibility:
   - Right-click in Materials folder → **Create → Material**
   - Name it `CollisionExample_Mat`
   - Set **Albedo Color**: Cyan (0, 255, 255)
   - Enable **Emission** and set to cyan
   - Drag material onto the cube

### Example 3: Proximity Zone

1. Create an **Empty GameObject**
2. Rename to `3_ProximityZone`
3. Position: (0, 0, 8) - further ahead
4. Add Component → **Sphere Collider**
   - **Is Trigger**: checked
   - **Radius**: 2
5. Add Component → **Transition Trigger**
6. Configure:
   - **Trigger Type**: OnProximity
   - **Fade Type**: FadeOut
   - **Fade Duration**: 1.0
   - **Trigger Once**: checked

7. (Optional) Add a visual indicator:
   - Create a child Sphere (3D Object → Sphere)
   - Scale: (4, 4, 4) to match collider radius
   - Make it semi-transparent
   - Or delete it after testing (gizmo shows in editor)

### Example 4: Timed Fade

1. Create an **Empty GameObject**
2. Rename to `4_TimedFade`
3. Add Component → **Transition Trigger**
4. Configure:
   - **Trigger Type**: Timed
   - **Fade Type**: FadeToWhite
   - **Fade Duration**: 0.5
   - **Delay Before Trigger**: 10
   - **Repeat Timed**: unchecked
   - **Trigger Once**: checked

### Example 5: Interaction Object (Grabbable)

1. Create a **Cube** (3D Object → Cube)
2. Rename to `5_GrabExample`
3. Position: (-2, 1, 3) - in front and to the left of spawn
4. Scale: (0.3, 0.3, 0.3)
5. Add Component → **Rigidbody**
   - **Mass**: 1
   - **Use Gravity**: checked
6. Add Component → **XR Grab Interactable** (from XR Interaction Toolkit)
   - Configure as needed for your XR setup
7. Add Component → **Transition Trigger**
8. Configure:
   - **Trigger Type**: OnInteraction
   - **Fade Type**: FadeOutAndIn
   - **Fade Duration**: 0.3
   - **Hold Duration**: 0.1
   - **Trigger Once**: unchecked (can trigger multiple times)

9. Create a material:
   - Name: `GrabExample_Mat`
   - Color: Yellow (255, 255, 0)
   - Enable emission
   - Drag onto the cube

## Step 6: Organize the Hierarchy

Organize your scene hierarchy like this:

```
Transition Demo Scene
├── Directional Light
├── Environment
│   ├── Floor
│   └── (Optional walls)
├── XR Origin (XR Rig)
│   └── [XR components]
├── ScreenFadeVolume
│   ├── Volume (component)
│   ├── ScreenFader (component)
│   └── TransitionDemoController (component)
└── Examples
    ├── 1_WelcomeSequence
    ├── 2_CollisionExample
    ├── 3_ProximityZone
    ├── 4_TimedFade
    └── 5_GrabExample
```

To create the "Examples" parent:
1. Create Empty GameObject, name it `Examples`
2. Drag all example objects under it

## Step 7: Test in Editor

1. Press **Play** in Unity Editor
2. You should see:
   - Scene fades in from black on start (if welcome sequence enabled)
   - Timed fade triggers after 10 seconds
3. Test each trigger:
   - Walk into collision cube → fade out/in
   - Walk near proximity zone → fade to black
   - Grab the interaction object → fade out/in

**Note**: Full VR testing requires building to headset.

## Step 8: Build and Test in VR

1. Go to **File → Build Settings**
2. Add the Transition Demo Scene to build
3. Configure for your VR platform (Quest, SteamVR, etc.)
4. Build and deploy to headset
5. **Critical Test**: Verify **no gaps** in peripheral vision during fades
6. Test all triggers in VR

## Troubleshooting

**Fade doesn't work:**
- Check ScreenFader.Instance is not null (check console)
- Verify FadeVolumeProfile has ColorAdjustments override enabled
- Check Volume component references FadeVolumeProfile
- Verify Volume Priority is high (100)

**Gaps in peripheral vision:**
- This shouldn't happen with Post-Processing Volume approach
- If it does, increase `fadeToBlackValue` (make more negative, e.g., -15)

**Triggers don't work:**
- Collision: Check collider has "Is Trigger" enabled
- Interaction: Check XRGrabInteractable is configured
- Proximity: Check trigger distance is reasonable
- Check TransitionTrigger component is enabled

**Performance issues:**
- Volume weight automatically sets to 0 when not fading
- Disable other Post-Processing overrides you don't need
- Check only one ScreenFadeVolume exists in scene

**Welcome sequence doesn't play:**
- Check TransitionDemoController has "Play Welcome Sequence" enabled
- Verify ScreenFader.Instance exists (check console for errors)

## Next Steps

- Customize fade durations for your use case
- Add more example scenarios
- Integrate with your game systems (teleport, scene loading, etc.)
- Create custom trigger logic
- Add sound effects during fades
- Experiment with different fade values

## Additional Resources

- [QUICK_REFERENCE.md](file:///c:/Users/Wonderfy149/Documents/xr-sandbox/xr-sandbox-app/Assets/App/Docs/Transition%20Demo/QUICK_REFERENCE.md) - API reference and common use cases
- [Implementation Plan](file:///C:/Users/Wonderfy149/.gemini/antigravity/brain/fab4eb3a-6dd2-458e-be76-10d2e30c3ac4/implementation_plan.md) - Technical details
- NorthStar ScreenFader reference - Original inspiration
