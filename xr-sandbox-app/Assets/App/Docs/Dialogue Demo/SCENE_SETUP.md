# VR Dialogue Demo - Scene Setup Guide

## Overview

This guide walks you through setting up the complete VR Dialogue Demo scene in Unity Editor. Follow these steps to create a working demo with all example triggers.

## Prerequisites

- Unity project with XR Interaction Toolkit installed
- TextMeshPro package installed
- UniTask package installed
- All Dialogue Demo scripts compiled without errors

## Step 1: Create the Demo Scene

1. In Unity, go to **File → New Scene**
2. Choose **Basic (Built-in)** or **Basic (URP)** depending on your render pipeline
3. Save the scene as `Dialogue Demo Scene.unity` in:
   ```
   Assets/App/Demos/Dialogue Demo/Scenes/
   ```

## Step 2: Create VRPromptBox Prefab

### Create the GameObject Hierarchy

1. In the Hierarchy, create a new **GameObject** (right-click → Create Empty)
2. Rename it to `VRPromptBox`
3. Reset its transform (position 0,0,0, rotation 0,0,0, scale 1,1,1)

### Add Canvas Component

1. Select `VRPromptBox`
2. Add Component → **Canvas**
3. Configure Canvas:
   - **Render Mode**: World Space
   - **Event Camera**: (leave empty, will be set at runtime)
   - **Sorting Layer**: Default
   - **Order in Layer**: 0

4. Set the **RectTransform** scale to `(0.002, 0.002, 0.002)`

### Add CanvasGroup Component

1. With `VRPromptBox` selected
2. Add Component → **Canvas Group**
3. Configure:
   - **Alpha**: 0 (starts hidden)
   - **Interactable**: unchecked
   - **Block Raycasts**: unchecked

### Add FloatingText Script

1. With `VRPromptBox` selected
2. Add Component → **FloatingText** (from Dialogue Demo Scripts)
3. Configure:
   - **Distance From Camera**: 2
   - **Offset**: (0, 0.5, 0)
   - **Fixed Size**: 0.002
   - **Max Distance**: 50
   - **Canvas**: Drag the Canvas component here

### Add VRPrompt Script

1. With `VRPromptBox` selected
2. Add Component → **VRPrompt** (from Dialogue Demo Scripts)
3. Leave references empty for now (we'll assign them after creating UI)
4. Configure:
   - **Display Duration**: 3
   - **Fade Duration**: 0.5

### Create UI Hierarchy

1. Right-click `VRPromptBox` → **UI → Panel**
2. Rename the Panel to `UI`
3. Configure the `UI` RectTransform:
   - **Width**: 1200
   - **Height**: 300
   - **Anchors**: Center (0.5, 0.5)
   - **Pivot**: (0.5, 0.5)
   - **Position**: (0, 0, 0)

4. Select the `UI` GameObject's **Image** component
5. Configure:
   - **Color**: Black with alpha 200 (RGBA: 0, 0, 0, 0.78)
   - **Sprite**: UI-Background (Unity default) or leave as default
   - **Image Type**: Sliced (optional, for rounded corners)

6. Right-click `UI` → **UI → Text - TextMeshPro**
7. Rename to `PromptText`
8. Configure the **TextMeshProUGUI** component:
   - **Text**: "Sample Prompt Text"
   - **Font**: Default or choose a readable font
   - **Font Size**: 120
   - **Alignment**: Center (both horizontal and vertical)
   - **Color**: White (255, 255, 255, 255)
   - **Word Wrapping**: Enabled
   - **Overflow**: Truncate

9. Configure `PromptText` RectTransform:
   - **Anchors**: Stretch (left: 0, top: 0, right: 1, bottom: 1)
   - **Left, Right, Top, Bottom**: 40 (padding)

### Assign References in VRPrompt

1. Select `VRPromptBox`
2. In the **VRPrompt** component:
   - **Prompt Text**: Drag `PromptText` (TextMeshProUGUI) here
   - **Canvas Group**: Drag the CanvasGroup component here
   - **Floating Text**: Drag the FloatingText component here

### Save as Prefab

1. Drag `VRPromptBox` from Hierarchy to:
   ```
   Assets/App/Demos/Dialogue Demo/Prefabs/
   ```
2. Delete `VRPromptBox` from the Hierarchy (we'll add it back via PromptManager)

## Step 3: Set Up the Scene

### Add XR Origin

**Option A: Copy from existing demo**
1. Open `Car Demo Scene.unity`
2. Find the XR Origin GameObject
3. Copy it (Ctrl+C)
4. Open `Dialogue Demo Scene.unity`
5. Paste it (Ctrl+V)

**Option B: Create new XR Origin**
1. Right-click in Hierarchy → **XR → XR Origin (Action-based)** or **XR Origin (Device-based)**
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
   - Duplicate for other walls

### Add PromptManager

1. Create an empty GameObject (right-click → Create Empty)
2. Rename to `Prompt System`
3. Position: (0, 0, 0)
4. Add Component → **PromptManager**
5. Configure:
   - **Use Queue**: checked
   - **Delay Between Prompts**: 0.5

6. Drag the `VRPromptBox` prefab from the Prefabs folder into the scene as a child of `Prompt System`
7. In PromptManager component:
   - **Default Prompt**: Drag the `VRPromptBox` child GameObject here

### Add DialogueDemoController

1. Select `Prompt System` GameObject
2. Add Component → **DialogueDemoController**
3. Configure:
   - **Show Welcome On Start**: checked
   - **Welcome Delay**: 1
   - **Auto Start Tutorial**: checked (optional)
   - **Tutorial Start Delay**: 10
   - **Enable Debug Logs**: checked

## Step 4: Create Example Triggers

### Example 1: Welcome Message (OnStart)

Already handled by DialogueDemoController! No additional setup needed.

### Example 2: Collision Trigger

1. Create a **Cube** (3D Object → Cube)
2. Rename to `Collision Example`
3. Position: (2, 1, 3) - in front and to the right of spawn
4. Scale: (0.5, 0.5, 0.5)
5. Add Component → **Box Collider**
   - **Is Trigger**: checked
6. Add Component → **PromptTrigger**
7. Configure:
   - **Trigger Type**: OnCollision
   - **Prompt Message**: "You touched the collision cube!"
   - **Custom Duration**: 3
   - **Trigger Once**: checked

8. Create a material for visibility:
   - Right-click in Materials folder → **Create → Material**
   - Name it `CollisionExample_Mat`
   - Set **Albedo Color**: Cyan (0, 255, 255)
   - Drag material onto the cube

### Example 3: Interaction Object

1. Create a **Cube** (3D Object → Cube)
2. Rename to `Grab Example`
3. Position: (-2, 1, 3) - in front and to the left of spawn
4. Scale: (0.3, 0.3, 0.3)
5. Add Component → **Rigidbody**
   - **Mass**: 1
   - **Use Gravity**: checked
6. Add Component → **XR Grab Interactable** (from XR Interaction Toolkit)
   - Configure as needed for your XR setup
7. Add Component → **PromptTrigger**
8. Configure:
   - **Trigger Type**: OnInteraction
   - **Prompt Message**: "You grabbed the object! Try throwing it."
   - **Custom Duration**: 3
   - **Trigger Once**: unchecked (can trigger multiple times)

9. Create a material:
   - Name: `GrabExample_Mat`
   - Color: Yellow (255, 255, 0)
   - Drag onto the cube

### Example 4: Proximity Zone

1. Create an **Empty GameObject**
2. Rename to `Proximity Zone`
3. Position: (0, 0, 8) - further ahead
4. Add Component → **Sphere Collider**
   - **Is Trigger**: checked
   - **Radius**: 2
5. Add Component → **PromptTrigger**
6. Configure:
   - **Trigger Type**: OnProximity
   - **Prompt Message**: "You've entered the proximity zone"
   - **Custom Duration**: 2.5
   - **Trigger Once**: checked

7. (Optional) Add a visual indicator:
   - Create a child Sphere (3D Object → Sphere)
   - Scale: (4, 4, 4) to match collider radius
   - Make it semi-transparent
   - Or delete it after testing (gizmo shows in editor)

### Example 5: Timed Sequence

This is handled by the DialogueDemoController's auto-start tutorial feature. If you want a separate timed trigger:

1. Create an **Empty GameObject**
2. Rename to `Timed Prompt`
3. Add Component → **PromptTrigger**
4. Configure:
   - **Trigger Type**: Timed
   - **Prompt Message**: "This is a timed prompt!"
   - **Delay Before Trigger**: 15
   - **Repeat Timed**: unchecked
   - **Trigger Once**: checked

## Step 5: Organize the Hierarchy

Organize your scene hierarchy like this:

```
Dialogue Demo Scene
├── Directional Light
├── Environment
│   ├── Floor
│   └── (Optional walls)
├── XR Origin (XR Rig)
│   └── [XR components]
├── Prompt System
│   ├── PromptManager (component)
│   ├── DialogueDemoController (component)
│   └── VRPromptBox (prefab instance)
└── Examples
    ├── Collision Example
    ├── Grab Example
    ├── Proximity Zone
    └── Timed Prompt
```

To create the "Examples" parent:
1. Create Empty GameObject, name it `Examples`
2. Drag all example objects under it

## Step 6: Test in Editor

1. Press **Play** in Unity Editor
2. You should see:
   - Welcome message appears after 1 second
   - Tutorial sequence starts after 10 seconds (if enabled)
3. Test each trigger:
   - Walk into collision cube
   - Grab the interaction object
   - Walk near proximity zone
   - Wait for timed prompt

**Note**: Full VR testing requires building to headset.

## Step 7: Build and Test in VR

1. Go to **File → Build Settings**
2. Add the Dialogue Demo Scene to build
3. Configure for your VR platform (Quest, SteamVR, etc.)
4. Build and deploy to headset
5. Test all triggers in VR

## Troubleshooting

**Prompt doesn't appear:**
- Check PromptManager has VRPromptBox assigned
- Check VRPrompt has all references assigned
- Check Camera.main is set correctly
- Check console for errors

**Text is too small/large:**
- Adjust `Fixed Size` in FloatingText (try 0.001 to 0.003)
- Adjust Canvas scale (try 0.001 to 0.003)
- Adjust TextMeshPro font size

**Triggers don't work:**
- Collision: Check collider has "Is Trigger" enabled
- Interaction: Check XRGrabInteractable is configured
- Proximity: Check trigger distance is reasonable
- Check PromptTrigger component is enabled

**Prompts overlap:**
- Enable "Use Queue" in PromptManager
- Increase "Delay Between Prompts"

## Next Steps

- Customize prompt appearance (colors, fonts, background)
- Add more example scenarios
- Create custom trigger logic
- Integrate with your game systems
- Add sound effects
- Create multiple prompt styles

## Additional Resources

- [README.md](file:///c:/Users/Wonderfy149/Documents/xr-sandbox/xr-sandbox-app/Assets/App/Demos/Dialogue%20Demo/README.md) - Usage documentation
- [VRTextInput_DialogueSystem.md](file:///c:/Users/Wonderfy149/Documents/xr-sandbox/xr-sandbox-app/Assets/App/Demos/Car%20Demo/Docs/VRTextInput_DialogueSystem.md) - NorthStar reference
- [Implementation Walkthrough](file:///C:/Users/Wonderfy149/.gemini/antigravity/brain/556023dd-e14e-4789-bfe9-1a27ff1ba6e6/walkthrough.md) - Technical details
