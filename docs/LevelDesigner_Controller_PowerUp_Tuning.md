# Level Designer Tuning Guide: Movement, Chaser, and Power-Ups

This guide explains the **Inspector fields you can tune** in the following scripts and what those values do in gameplay:

- `FarmerController`
- `PlayerController`
- `SpeedModel`
- `SpeedModifiers` / `SpeedModifier` / `SpeedAxes`
- `PowerUp`

---

## 1) FarmerController (`SuperPorkOut.Characters.Farmer.FarmerController`)

The farmer is a chaser that moves forward in world Z and tracks the player's X lane.

### Tweakable fields

| Inspector Field | What it affects in gameplay | Practical tuning notes |
|---|---|---|
| `farmerForwardSpeed` | Base forward speed of farmer (world Z movement). | Higher = farmer catches up faster. Lower = more forgiving chase pressure. |
| `forwardSpeedMultiplierOverTime` | Multiplier curve applied to base forward speed over elapsed game time. | Use this to ramp pressure over time. Example: start easy, then escalate after 60–120s. |
| `playerTag` | Which tagged object is followed as the player target. | Must match your player object's tag or farmer won't track X / behind-distance correctly. |
| `followMode` (`Snap` / `Smooth`) | X-axis tracking behavior. `Snap` instantly matches player's X; `Smooth` moves toward it over time. | `Snap` feels relentless and precise. `Smooth` feels more natural and can be dodged with quick lane changes. |
| `lateralFollowSpeed` | X tracking speed used in `Smooth` mode (units/sec). | Higher = quicker lane matching; lower = more lag and more player escape windows. |
| `maxLateralStepPerFrame` | Optional per-frame clamp on X movement while smoothing. `0` = disabled. | Use to prevent big visual jumps on variable frame steps or teleport-like reactions. |
| `neverPassPig` | If enabled, farmer cannot move ahead of player in Z. | Recommended for classic chase feel where farmer stays behind and threatens from rear. |
| `minBehindDistance` | Minimum Z distance farmer must stay behind player when `neverPassPig` is on. | Smaller distance = tighter pressure. Larger distance = safer buffer for player. |

### Important behavior details

- Actual forward speed each frame is:
  - `farmerForwardSpeed * forwardSpeedMultiplierOverTime(time)`
- Curve time is clamped to the first/last keyframe times.
- Negative curve outputs are clamped to `0`, so farmer will never move backward from this multiplier.

---

## 2) PlayerController (`SuperPorkOut.Characters.Player.PlayerController`)

This script applies movement velocity to the player rigidbody, handles wall sliding, optional extra down-force, and optional world X boundaries.

### Tweakable fields

| Inspector Field | What it affects in gameplay | Practical tuning notes |
|---|---|---|
| `movementBasis` | Reference transform for forward/right movement directions. | Set to camera/world anchor if you want movement aligned to that object. Leave null for world forward/right. |
| `extraDownForce` | Additional downward acceleration each physics tick. | Higher = stronger ground adhesion; helps reduce floatiness. Too high can feel heavy/sticky. |
| `clampX` | Enables world X position boundaries. | Turn on for lane-bounded levels / play spaces. |
| `minX` / `maxX` | Horizontal limits when `clampX` is enabled. | Defines playable width. At bounds, X velocity is zeroed to avoid jitter/buzzing against edge. |

### Important behavior details

- Player desired movement comes from `SpeedModel.GetPlanarVelocity(...)`.
- Collision normals from walls are used to remove into-wall velocity components, so player slides along surfaces instead of sticking.
- Floor/ceiling-ish normals are ignored for this planar slide logic (`abs(normal.y) > 0.5`).

---

## 3) SpeedModel (`SuperPorkOut.Characters.Player.SpeedModel`)

Converts base movement speeds + stamina + speed modifiers into final forward and strafe speeds.

### Tweakable fields

| Inspector Field | What it affects in gameplay | Practical tuning notes |
|---|---|---|
| `baseForwardSpeed` | Base automatic forward speed before multipliers. | Main control for overall game pace/progression pressure. |
| `baseStrafeSpeed` | Base horizontal lane-change speed before multipliers. | Main control for dodge responsiveness and lane-switch difficulty. |
| `staminaToSpeed` (AnimationCurve) | Maps normalized stamina (0..1) to a speed multiplier. | Key curve for "fatigue feel". Lower low-end values punish exhaustion; >1 high-end values reward high stamina with speed boost. |

### Final speed formula

Each frame:

- `staminaNormalized = currentStamina / maxStamina` (clamped 0..1)
- `staminaMul = staminaToSpeed(staminaNormalized)`
- Axis modifiers come from `SpeedModifiers.GetAxisFactors()`

Then:

- `ForwardSpeed = baseForwardSpeed * staminaMul * forwardModifier`
- `StrafeSpeed  = baseStrafeSpeed  * staminaMul * strafeModifier`

So stamina influences **both** forward pressure and lane-change responsiveness.

---

## 4) SpeedModifiers / SpeedModifier / SpeedAxes

These scripts handle temporary axis-specific speed multipliers (e.g., terrain slow, debuff, etc.).

### What designers need to know

- A modifier has:
  - target axis (`Forward`, `Strafe`, or `Both`)
  - multiplier (`0+`)
- Multiple active modifiers are combined with a **"lowest wins" rule per axis**.
  - Example: forward modifiers of `0.9` and `0.6` result in `0.6` forward factor.
- Multipliers are clamped to minimum `0`.
  - `0` = fully stops that axis.
  - `1` = no change.
  - Values above `1` are effectively capped by the current implementation because per-axis aggregation starts at `1` and takes minimum values.

### SpeedAxes options

- `None`: affects nothing
- `Forward`: affects forward motion only
- `Strafe`: affects lateral/strafe motion only
- `Both`: affects both axes

---

## 5) PowerUp (`SuperPorkOut.Gameplay.Pickups.PowerUp`)

Pickup object that publishes a pickup event and is consumed on contact with player.

### Tweakable fields

| Inspector Field | What it affects in gameplay | Practical tuning notes |
|---|---|---|
| `foodType` | Category emitted in pickup event (for scoring/collection logic). | Use to tag collectible types for UI/objectives/analytics. |
| `staminaAmount` | Stamina restored on pickup (minimum 0). | Higher values give larger movement recovery via stamina system and speed curve. |
| `pickupSfx` | Audio clip played at pickup location. | Use for feedback clarity and reward feel. |

### Important behavior details

- On player enter:
  1. Plays SFX (if set)
  2. Raises `PickedUp` event with `foodType`, `staminaAmount`, and position
  3. Destroys pickup object
- Stamina restoration typically occurs via `Stamina` listening to this event.

---

## Quick balancing recipes

### Make early game forgiving, late game intense

- Lower `farmerForwardSpeed`
- Use `forwardSpeedMultiplierOverTime` to ramp aggressively after first minute
- Keep `baseStrafeSpeed` moderately high so lane swaps remain skillful

### Make stamina management central

- Lower `staminaToSpeed` output at low stamina (e.g., strong drop below 30%)
- Increase pickup `staminaAmount` differences by pickup type
- Keep some hazards applying `Strafe`-only slow to punish dodging when exhausted

### Tight lane runner feel

- Enable `clampX`, set narrow `minX`/`maxX`
- Use `followMode = Smooth` with reasonably high `lateralFollowSpeed`
- Small `minBehindDistance` for constant chase pressure
