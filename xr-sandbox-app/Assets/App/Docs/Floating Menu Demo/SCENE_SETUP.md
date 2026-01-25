# Floating Menu Demo - Scene Setup Guide

Follow these steps to set up the Floating Menu Demo scene with the controller-based toggle.

## 1. Scene Creation
1.  Create a new Scene in `Assets/App/Docs/Floating Menu Demo/` named `FloatingMenuDemo`.
2.  Delete the default `Main Camera` and `Directional Light`.
3.  Add a `Directional Light` (Right-click > Light > Directional Light).
4.  Add the **XR Origin** prefab (or set it up manually):
    -   Ensure it has `Input Action Manager` with default XRI actions.
    -   Ensure it has `XR Interaction Manager`.

## 2. Create the Floating Menu Canvas
1.  Right-click in Hierarchy > UI > **Canvas**.
2.  Rename it to `FloatingMenu`.
3.  In the `Canvas` component:
    -   Set **Render Mode** to **World Space**.
    -   Set **Rect Transform**:
        -   Pos X/Y/Z: `0, 1.5, 2` (Initially in front of user)
        -   Width: `400`, Height: `300`
        -   Scale: `0.002, 0.002, 0.002` (Make it small for VR)
4.  Add a `CanvasGroup` component (Required for fading).
5.  Add the `FloatingMenuManager` script to this GameObject.
    -   **Menu Canvas Group**: Assign the `CanvasGroup` you just added.
    -   **Fade Duration**: `0.3` (default).
    -   **Distance From Head**: `1.5` (default).
    -   **Menu Toggle Action**:
        -   Click the small circle icon.
        -   Search for `XRI Left/Menu` (or `XRI Right/Menu` if preferred) and select the *Input Action Reference*.
        -   *Note*: Ensure `Use Reference` is checked.

## 3. Design the Menu UI
1.  Inside `FloatingMenu`, right-click > UI > **Panel**.
    -   Rename to `Background`.
    -   Set Color to Black with mild transparency (Alpha ~200).
2.  Right-click `FloatingMenu` > UI > **Text - TextMeshPro**.
    -   Text: "Floating Menu"
    -   Position: Top of the panel.
3.  Right-click `FloatingMenu` > UI > **Slider**.
    -   Rename to `HeightSlider`.
    -   Position: Center.

## 4. Configure Height Adjuster
1.  Select the **FloatingMenu** (or `HeightSlider` object).
2.  Add the `HeightAdjuster` script.
3.  **Assignments**:
    -   **XR Origin**: Drag your scene's `XR Origin` GameObject here.
    -   **Height Slider**: Drag the `HeightSlider` UI component here.
    -   **Value Text**: (Optional) Drag the TextMeshPro object if you added one for the value.

## 5. Play Mode Test
1.  Press **Play**.
2.  Put on headset or use XR Simulation.
3.  Press the **Menu Button** on your Left Controller.
    -   **Result**: The menu should fade in/out and snap to a position in front of you.
4.  Interact with the **Height Slider**.
    -   **Result**: The camera height should move up/down.
