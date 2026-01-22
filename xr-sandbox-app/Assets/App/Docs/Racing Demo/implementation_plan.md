# VR Racing Demo Implementation Plan

Build a VR racing demo where the player starts standing, enters a car, races 3 laps around a circular track, and sees a congratulations message upon completion.

---

## Existing Assets to Leverage

| Component | Source | Purpose |
|-----------|--------|---------|
| Car Physics | [CarController.cs](file:///d:/Unity%20Projects/xr-sandbox/xr-sandbox-app/Assets/App/Demos/Car%20Demo/Scripts/CarController.cs) | Wheel collider-based physics car |
| VR Car Entry | [CarTeleportAnchor.cs](file:///d:/Unity%20Projects/xr-sandbox/xr-sandbox-app/Assets/App/Demos/Car%20Demo/Scripts/CarTeleportAnchor.cs) | Teleport into car, fires `OnCarEnter` event |
| Input System | [CarInputManager.cs](file:///d:/Unity%20Projects/xr-sandbox/xr-sandbox-app/Assets/App/Demos/Car%20Demo/Scripts/CarInputManager.cs) | Steering wheel + joystick controls |
| Prompts/Countdown | [PromptManager.cs](file:///d:/Unity%20Projects/xr-sandbox/xr-sandbox-app/Assets/App/Demos/Dialogue%20Demo/Scripts/PromptManager.cs) | Show countdown & messages in VR |
| Lap Logic Reference | [ObjectiveCompleteLaps.cs](file:///d:/Unity%20Projects/unity_kartinggame/Assets/Karting/Scripts/GameModes/ObjectiveCompleteLaps.cs) | Lap counting pattern |
| Checkpoint Reference | [LapObject.cs](file:///d:/Unity%20Projects/unity_kartinggame/Assets/Karting/Scripts/GameModes/LapObject.cs) | Trigger-based checkpoint detection |
| Car Prefabs | `HotRod_Player Variant.prefab`, etc. | Existing VR-ready car prefabs |
| Track Assets | [Karting_Reference folder](file:///d:/Unity%20Projects/xr-sandbox/xr-sandbox-app/Assets/App/References/Karting_Reference/) | Track pieces from karting game |

---

## Proposed Changes

### Racing Demo Folder Structure
Create new demo folder: `Assets/App/Demos/Racing Demo/`

```
Racing Demo/
├── Scenes/
│   └── Racing Demo Scene.unity
├── Scripts/
│   ├── RacingGameManager.cs       [NEW]
│   ├── LapCheckpoint.cs           [NEW]
│   └── RaceCountdown.cs           [NEW]
├── Prefabs/
│   └── LapCheckpoint.prefab       [NEW]
└── Materials/
    └── StartFinishLine.mat        [NEW]
```

---

### Core Scripts

#### [NEW] [RacingGameManager.cs](file:///d:/Unity%20Projects/xr-sandbox/xr-sandbox-app/Assets/App/Demos/Racing%20Demo/Scripts/RacingGameManager.cs)

Central controller managing the entire race flow:

```csharp
public class RacingGameManager : MonoBehaviour
{
    // Configuration
    [SerializeField] private int totalLaps = 3;
    [SerializeField] private CarTeleportAnchor carTeleportAnchor;
    [SerializeField] private CarController carController;
    
    // State
    private RaceState currentState = RaceState.WaitingForPlayer;
    private int currentLap = 0;
    
    public enum RaceState
    {
        WaitingForPlayer,   // Player standing, car waiting
        Countdown,          // 3, 2, 1, GO!
        Racing,             // Active racing
        Finished            // Race complete
    }
    
    // Events
    public event Action<int> OnLapCompleted;
    public event Action OnRaceFinished;
}
```

**Flow Logic:**
1. **WaitingForPlayer** → Subscribe to `CarTeleportAnchor.OnCarEnter`
2. **OnCarEnter** → Disable car input, transition to **Countdown**
3. **Countdown** → Use `PromptManager.ShowPrompt()` for "3", "2", "1", "GO!"
4. **Racing** → Enable car input, track laps via checkpoint collisions
5. **Lap 3 Complete** → Transition to **Finished**, show congratulations

---

#### [NEW] [LapCheckpoint.cs](file:///d:/Unity%20Projects/xr-sandbox/xr-sandbox-app/Assets/App/Demos/Racing%20Demo/Scripts/LapCheckpoint.cs)

Trigger-based checkpoint inspired by [LapObject.cs](file:///d:/Unity%20Projects/unity_kartinggame/Assets/Karting/Scripts/GameModes/LapObject.cs):

```csharp
public class LapCheckpoint : MonoBehaviour
{
    [SerializeField] private bool isStartFinishLine = true;
    [SerializeField] private int checkpointIndex = 0;
    
    public event Action<LapCheckpoint> OnCheckpointPassed;
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if it's the player car
        if (other.TryGetComponent<CarController>(out var car))
        {
            OnCheckpointPassed?.Invoke(this);
        }
    }
}
```

---

#### [NEW] [RaceCountdown.cs](file:///d:/Unity%20Projects/xr-sandbox/xr-sandbox-app/Assets/App/Demos/Racing%20Demo/Scripts/RaceCountdown.cs)

Async countdown using the existing [PromptManager](file:///d:/Unity%20Projects/xr-sandbox/xr-sandbox-app/Assets/App/Demos/Dialogue%20Demo/Scripts/PromptManager.cs):

```csharp
public class RaceCountdown : MonoBehaviour
{
    public async UniTask StartCountdownAsync()
    {
        PromptManager.ShowPrompt("3", 1f);
        await UniTask.WaitForSeconds(1f);
        
        PromptManager.ShowPrompt("2", 1f);
        await UniTask.WaitForSeconds(1f);
        
        PromptManager.ShowPrompt("1", 1f);
        await UniTask.WaitForSeconds(1f);
        
        PromptManager.ShowPrompt("GO!", 1.5f);
    }
}
```

---

### Scene Setup

#### [NEW] [Racing Demo Scene.unity](file:///d:/Unity%20Projects/xr-sandbox/xr-sandbox-app/Assets/App/Demos/Racing%20Demo/Scenes/Racing%20Demo%20Scene.unity)

**Scene Hierarchy:**
```
Racing Demo Scene
├── XR Setup
│   ├── XR Origin
│   └── Teleportation Areas
├── Environment
│   ├── Ground Plane
│   └── CircularTrack (Loop using track pieces from Karting_Reference)
├── Racing
│   ├── VR Car (existing prefab from Car Demo)
│   ├── LapCheckpoint_StartFinish (trigger at start/finish line)
│   └── RacingGameManager
├── UI
│   ├── PromptManager (from Dialogue Demo)
│   └── LapCounterUI (optional world-space canvas)
└── Lighting
```

**Track Layout (Simple Oval/Circle):**
- Use track pieces from `References/Karting_Reference/AddOns/MgKarting_Racetrack/Prefabs/`
- Create a simple closed loop using turns and straights
- Place a single `LapCheckpoint` at the start/finish line

---

## Implementation Order

### Phase 1: Scene & Scripts Setup
| Step | Task | Details |
|------|------|---------|
| 1.1 | Create folder structure | `Assets/App/Demos/Racing Demo/` with subfolders |
| 1.2 | Create `RacingGameManager.cs` | Core state machine and race flow |
| 1.3 | Create `LapCheckpoint.cs` | Trigger-based lap detection |
| 1.4 | Create `RaceCountdown.cs` | Async countdown using PromptManager |

### Phase 2: Scene Assembly
| Step | Task | Details |
|------|------|---------|
| 2.1 | Create scene | Copy base from Car Demo Scene |
| 2.2 | Build circular track | Use Karting_Reference track pieces |
| 2.3 | Position car at start line | Existing VR car prefab |
| 2.4 | Add lap checkpoint | Trigger collider at start/finish |
| 2.5 | Wire up managers | Connect events and references |

### Phase 3: Polish & UI
| Step | Task | Details |
|------|------|---------|
| 3.1 | Add PromptManager | Copy from Dialogue Demo or add prefab |
| 3.2 | Lap counter display | Optional: world-space TextMeshPro |
| 3.3 | Finish message | "🎉 Race Complete! You finished in X time" |
| 3.4 | Reset/restart option | Optional: button to restart race |

---

## Verification Plan

### Automated Tests
```bash
# Build the project to verify no compile errors
# Unity will flag any missing references or namespace issues
```

### Manual Verification (VR Headset)
1. **Start Scene** → Player spawns standing, car is visible nearby
2. **Enter Car** → Teleport to car seat, countdown begins
3. **Countdown** → "3", "2", "1", "GO!" prompts appear sequentially
4. **Racing** → Car controls work, can drive around track
5. **Lap Detection** → Each pass through start/finish increments lap counter
6. **Lap 3** → Congratulations message appears
7. **End State** → Race is over, player remains in car (or can exit)

---

## User Review Required

> [!IMPORTANT]
> **Track Design Options:**
> 1. **Simple Oval** - Two curves and two straights (fastest to build)
> 2. **Figure-8** - More interesting but requires overpass logic
> 3. **Custom Layout** - Use more track pieces for variety
>
> Which track layout would you prefer for the initial demo?

> [!NOTE]
> **UI Consideration:**
> The current plan uses only `PromptManager` for countdown and messages. Would you like a persistent lap counter UI visible during racing (e.g., "Lap 2/3" display)?

> [!NOTE]
> **Timer Feature:**
> Should the race include a visible timer showing elapsed time? This could display the final time when the race completes.
