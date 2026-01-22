# VR Screen Fade Transition Demo - Scene Setup Guide

## Overview

This guide walks you through setting up the VR Screen Fade Transition Demo scene in Unity Editor.

## Prerequisites

- Unity project with XR Interaction Toolkit installed
- URP (Universal Render Pipeline) configured
- UniTask package installed
- All Transition Demo scripts compiled without errors

## Step 1: Create the Demo Scene

1. In Unity, go to **File → New Scene**
2. Choose **Basic (URP)**
3. Save the scene as `Transition Demo Scene.unity` in:
   ```
   Assets/App/Demos/Transition Demo/Scenes/
   ```

## Step 2: Create FadeVolumeProfile Asset

1. Navigate to `Assets/App/Demos/Transition Demo/`
2. Right-click → **Create → Rendering → URP Volume Profile**
3. Rename it to `FadeVolumeProfile`
4. Select it and click **Add Override → Post-processing → Color Adjustments**
5. Check **Post Exposure** and set value to `0`

## Step 3: Create ScreenFadeVolume Prefab

### Create the GameObject

1. Create a new **GameObject** (right-click → Create Empty)
2. Rename it to `ScreenFadeVolume`
3. Reset its transform (position 0,0,0)

### Add Volume Component

1. Add Component → **Volume**
2. Configure:
   - **Mode**: Global
   - **Profile**: Drag `FadeVolumeProfile` asset here
   - **Weight**: 0
   - **Priority**: 100

### Add ScreenFader Script

1. Add Component → **Screen Fader**
2. Configure:
   - **Fade Volume**: Drag the Volume component here
   - **Default Fade Duration**: 1.0
   - **Fade To Black Value**: -10
   - **Fade To White Value**: 10

### Save as Prefab

1. Drag `ScreenFadeVolume` from Hierarchy to:
   ```
   Assets/App/Demos/Transition Demo/Prefabs/
   ```

## Step 4: Set Up the Scene

### Add XR Origin

1. Open `Dialogue Demo Scene.unity` or `Car Demo Scene.unity`
2. Copy the XR Origin GameObject
3. Paste it into `Transition Demo Scene.unity`

### Create Environment

1. Create a **Plane** for the floor
   - Position: (0, 0, 0)
   - Scale: (5, 1, 5)

### Add TransitionDemoController

1. Select `ScreenFadeVolume` GameObject
2. Add Component → **Transition Demo Controller**
3. **IMPORTANT**: Drag the `ScreenFader` component into the **Screen Fader** field
4. Configure:
   - **Play Welcome Sequence**: checked
   - **Welcome Delay**: 0.5
   - **Auto Start Demo Sequence**: unchecked

## Step 5: Organize the Hierarchy

```
Transition Demo Scene
├── Directional Light
├── Environment
│   └── Floor
├── XR Origin (XR Rig)
└── ScreenFadeVolume
    ├── Volume (component)
    ├── ScreenFader (component)
    └── TransitionDemoController (component)
```

## Step 6: Test in Editor

1. Press **Play**
2. You should see scene fade in from black
3. Use context menu on components to test fades

## Step 7: Build and Test in VR

1. Build and deploy to headset
2. Verify **no gaps** in peripheral vision during fades

## Troubleshooting

**Fade doesn't work:**
- Check ScreenFader reference is assigned in TransitionDemoController
- Verify FadeVolumeProfile has ColorAdjustments override
- Verify Volume Priority is 100

**NullReferenceException:**
- Make sure to drag the ScreenFader component into the TransitionDemoController's Screen Fader field
