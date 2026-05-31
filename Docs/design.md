# Power System Design

PowerGrid adds a custom electricity layer to Shapez 2. There is no native power API — supply, demand, and slowdown are handled entirely by this mod.

## V1 Overview

V1 has **no poles or wires**. Power distributes automatically across connected platform and rail networks.

### Rules

- Connected platforms/rails = one power network.
- Disconnected islands = separate power networks.
- If a platform path exists, power is shared.
- **Platform power:** machines on a platform are powered when a generator sits on that platform **or** when the platform connects to a powered platform cluster via **rails** (same connected graph as the generator).
- Belts, pipes, and transport-only entities never use energy.

### Core Systems

- **`PowerNetworkManager`**
  - Finds connected platform clusters.
  - Builds separate power networks.
  - Calculates supply/demand.
  - Applies efficiency multiplier.
- **`PowerProducerSimulation`** — generates power as negative consumption.
- **`PowerConsumerSimulation`** — calculates machine power usage.

## Network Equations

Generated power:

```text
Pgen = ΣGi
```

Connector complexity:

```text
Cc = 1 + 0.15(Ni + No - 2) + 0.25 * abs(No - Ni)
```

Machine multipliers:

- `Mechanical = 1.0`
- `Fluid = 1.3`
- `Precision = 1.6`
- `Advanced = 2.0`

Consumer power:

```text
PowerKW = BaseKW + (OPM / 60) * WorkKW * Cc * Cm
```

Recommended defaults:

```text
BaseKW = 0.5
WorkKW = 4
```

Power ratio:

```text
R = Pgen / Puse
```

Efficiency:

```text
E = min(1, R)
```

Machine throughput:

```text
ActualOPM = BaseOPM * E
```

### Behavior

- `R >= 1`: full speed.
- `R < 1`: all powered machines slow proportionally.
- Example: if needed power is `500 kW` and generated power is `250 kW`, ratio is `0.5` and all powered machines run at `50%` speed.

## Implementation

Power profile:

```csharp
class PowerProfile
{
    float BaseKW;
    float WorkKW;
    float MachineMultiplier;
    float ManualOverrideKW = -1;
}
```

Registry:

```csharp
Dictionary<BuildingDefinitionId, PowerProfile>
```

Power logic:

```text
if (override exists)
    use override
else
    calculate power automatically
```

### Do not rely on

- Native energy API.
- `WithEfficiencyData()` — currently has a hardcoded issue.
- `BuffableBeltDelay.DiscreteDuration`.
- HUD APIs for V1.

### Instead

- Use custom tick/update timers.
- Ship V1 with logs/debug overlay.
- Use placeholder assets if needed.

## V1 Scope

**Included:**

- Power generation.
- Consumption.
- Platform-based networks.
- Efficiency slowdown.
- Micro Reactor.
- Debug readout.

**Skipped for V1:**

- Poles.
- HUD polish.
- Fancy assets.
- Native integrations.

See [Roadmap](roadmap.md) for post-V1 distribution (poles, transformers) and additional generators.
