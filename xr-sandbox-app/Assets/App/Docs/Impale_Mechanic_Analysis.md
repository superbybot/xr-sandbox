# Physics-Based Impale Mechanic Analysis (Downshot VR Style)

## Overview
This document breaks down the "impale" mechanics seen in high-end physics VR games like *Downshot*, *Boneworks*, and *Blade & Sorcery*. Commonly referred to as "frictional impalement" or "axis-constrained penetration," this mechanic allows players to stab objects and feel resistance (drag) while sliding the weapon in and out.

## Key Challenges
Standard physics engines (like Unity's PhysX) are designed to prevent object overlapping.
*   **Depenetration Force:** If a sword collider enters an enemy collider, the engine applies massive force to separate them, causing "explosions" or erratic movement.
*   **Drift:** If the virtual hand is forced to stay with the sword while it's stuck, it desyncs from the real world hand.

## Implementation Technique

### 1. Disable Collision (The "Ghosting" Trick)
Upon detecting a valid pierce (High Velocity + Tip Collision):
*   **Immediate Action:** `Physics.IgnoreCollision(swordCollider, enemyCollider, true)` must be called instantly.
*   **Purpose:** Prevents physics explosions and allows the blade to exist *inside* the mesh.

### 2. Constraints (The "Rail" System)
Once inside, the weapon is no longer a free physics object. It becomes a constrained object.
*   **Rotation:** Locked to the enemy's rotation (or allowed slight wiggle room via joints).
*   **Translation:** Locked to a single local axis (usually the weapon's forward vector).
*   **Mechanism:** This is often done using a `ConfigurableJoint` created at the point of impact, or by manually updating `Rigidbody.velocity` to only allow movement along the blade axis.

### 3. Frictional Drag (Haptics & Input)
To simulate density (meat/wood vs air):
*   **Virtual Hand Desync:** The virtual weapon moves slower than your real hand.
*   **Formula:** `WeaponMovement = HandMovement * FrictionFactor`
*   **Haptics:** Continuous low-frequency vibration is played on the controller as long as the weapon is moving *inside* the object.

### 4. Exit Condition
The system monitors the depth of the penetration every frame.
*   **Depth > 0:** Maintain constraints/joints.
*   **Depth <= 0:** 
    1.  Destroy the joint.
    2.  `Physics.IgnoreCollision(..., false)` (Re-enable collisions).
    3.  Play "Exit" sound.

## Pseudocode Example

```csharp
public class ImpaleMechanic : MonoBehaviour {
    private bool isImpaling = false;
    private Collider currentTarget;
    private float penetrationDepth = 0f;

    void OnCollisionEnter(Collision collision) {
        // Condition: Hit by tip & Fast enough
        if (IsTipHit(collision) && relativeVelocity.magnitude > pierceThreshold) {
            StartImpale(collision.collider);
        }
    }

    void StartImpale(Collider enemy) {
        isImpaling = true;
        currentTarget = enemy;

        // 1. Disable standard physics bounce
        Physics.IgnoreCollision(myCollider, currentTarget, true);

        // 2. Create Joint / Constraints
        // (Pseudocode: Lock rotation, allow sliding only on Z axis)
        LockPhysicsToAxis(enemy.transform);
    }

    void FixedUpdate() {
        if (!isImpaling) return;

        // 3. Apply Friction / Drag
        // Calculate projected movement along blade axis
        float userPush = Vector3.Dot(handVelocity, transform.forward);
        
        // Move rigidbody manually with drag
        rb.velocity = transform.forward * userPush * frictionCoefficient;

        // 4. Check Exit
        if (CheckTipExitedBody()) {
            StopImpale();
        }
    }

    void StopImpale() {
        isImpaling = false;
        Physics.IgnoreCollision(myCollider, currentTarget, false);
        UnlockPhysics();
        // Play Schwing Sound
    }
}
```

## Advanced Considerations
*   **Hierarchy:** For enemies, the weapon usually becomes a child of the enemy (or jointed to it) so if the enemy creates a fluid ragdoll motion, the sword moves with their limb.
*   **Damage:** Damage is often continuous (sawing motion) or triggered on "Depth Milestones" (e.g., hitting a vital organ depth).
