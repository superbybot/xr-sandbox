# 🧪 XR Plugin Research & Integration Plan

> [!NOTE]
> This document analyzes plugins compatible with **Meta XR SDK** to enhance our development workflow.

## 1. 🏆 Recommended Plugins

### A. Productivity & Interactions
| Plugin | Type | Pros | Cons | Recommendation |
| :--- | :--- | :--- | :--- | :--- |
| **Meta XR All-in-One** | 🆓 Native | **Essential**. Native Hand Tracking, Passthrough, best performance. | N/A | ✅ **ADOPT** |
| **Unity XR Interaction Toolkit** | 🆓 Native | Great Cross-Platform support (Vision Pro, Vive, Quest). | Generic interaction feel. Hand tracking lacks "Meta polish". | ❌ Skip (Migrating Away) |
| **VR Interaction Framework** | 💲 Asset | Huge library of pre-made mechanics (guns, climbing). | Overlaps with Meta SDK. Adds dependency weight. | ❌ Skip (Overkill) |
| **Hurricane VR** | 💲 Asset | Best-in-class physics hands. | High learning curve. | ❌ Skip for now |

### B. Graphics & Optimization
| Plugin | Type | Benefit | Recommendation |
| :--- | :--- | :--- | :--- |
| **Mesh Baker** | 🛠️ Tool | **Critical for Quest**. Drastically reduces draw calls. | ✅ **ADOPT** for Racing Demo |
| **Beautify 3** | 🎨 Visuals | Cheap post-processing wrapper. | 🟡 Consider later |

### C. Development Tools
| Plugin | Type | Benefit |
| :--- | :--- | :--- |
| **Odin Inspector** | 🛠️ Editor | Faster custom inspectors. |
| **ParrelSync** | 🛠️ Editor | Test logic without building. |

---

## 2. ⚖️ Deep Dive: Ecosystem Comparison

| Feature | **Meta XR All-in-One (Interaction SDK)** | **Unity XR Interaction Toolkit (XRI)** | **VR Interaction Framework (VRIF)** |
| :--- | :--- | :--- | :--- |
| **Type** | Native Platform SDK | Generic Cross-Platform Toolkit | High-Level Game Toolkit (Asset) |
| **Cost** | 🆓 Free | 🆓 Free | 💵 ~$60 |
| **Hand Tracking** | 🌟 **Best in Class**. Native gestures, "Poke" UI. | 😐 Decent, but generic. | 😐 Wraps generic data. |
| **Physics** | ⚡ "Snappy" & Direct. | 📦 Basic rigidbodies. | 🏋️ **Heavy Physics** (weighted objects). |
| **Use Case** | **Production Apps** (UI, Utility, Business). | **Cross-Platform** (Vive + Quest). | **Rapid Game Prototyping** (Shooters). |

> [!TIP]
> **Why Meta SDK?**
> For a **Sandbox** with UI (Dialogue) and clean Inputs (Racing), Meta's modular tracking is superior to VRIF's "gamey" physics presets.

---

## 3. 🗺️ Integration Plan

**Strategy**: Use **Meta Interaction SDK** for core mechanics. Use **Mesh Baker** for performance.

### Step 1: Essential Setup (Priority: High)
- [ ] **Install Meta XR All-in-One SDK**.
- [ ] Configure Project Settings > Meta XR > **Fix All**.

### Step 2: Optimization (Priority: Medium)
- [ ] **Get Mesh Baker**:
    *   Use to combine track segments in the Racing Demo.
    *   *Goal*: Maintain >72 FPS on Quest 2/3.

### Step 3: Advanced Physics (Optional)
- [ ] Only consider **Hurricane VR** if users report "floaty" hands.

---

## 4. ⚙️ Workflow Integration

### 🏎️ For the Racing Demo
*   **Mesh Baker**: Combine track meshes.
*   **Meta Building Blocks**: Use `Player Locomotion` features if needed.

### 💬 For Dialogue & Menu Demos
*   **Meta Interaction SDK**: Use `PokeInteractor` for all UI.
*   **Editor Tools**: Use **ParrelSync** for testing network logic (if applicable).

---

## 5. ⏭️ Next Steps
1.  **Budget Check**: Confirm funds for Mesh Baker / Odin.
2.  **Execution**: Proceed with Meta SDK Installation.
