# Roadmap

## Power Generators & Models

Each generator needs the same asset types as Micro Reactor (see `Resources/MicroReactor.fbx`, `PowerGrid_Icon.png`):

| Asset | Purpose |
|---|---|
| **FBX mesh** | Building body; single- or multi-mesh; load via `FileMeshLoader` or Assimp |
| **PNG icon** | Toolbar + building variant icon (`FileTextureLoader.LoadTextureAsSprite`) |
| **Belt lane draw data** | Animated item paths on belt inputs (see `MicroReactorDrawData`, `MicroReactorBeltLaneRenderingDefinition`) |
| **Fluid pipe connectors** | Fusion / Quantum / Antimatter only — coolant intake on pipe network |

### Micro Reactor

- [x] 2×2 footprint, 1 shape input (fuel)
- [x] `MicroReactor.fbx`, icon, simulation, renderer
- [ ] Improve model and generate asset bundle

### Solar Array

- [ ] **Model:** `SolarArray.fbx` + icon
- [ ] **Footprint:** 3×3 *or* 1×4 (pick one before modeling)
- [ ] **Inputs:** none — passive power, no belts/pipes
- [ ] **Gameplay:** constant `PowerProducerSimulation` output; no fuel tick

### Fusion Reactor

- [ ] **Model:** `FusionReactor.fbx` + icon
- [ ] **Footprint:** 5×5
- [ ] **Inputs:** shapes (belt) + coolant fluid (default game fluid color)
- [ ] **Belt lane draw data** for shape intake(s)
- [ ] **Pipe connector** for coolant

### Quantum Generator

- [ ] **Model:** `QuantumGenerator.fbx` + icon
- [ ] **Footprint:** 8×8 (larger tier than Fusion)
- [ ] **Inputs:** 3 belt inputs — higher shape throughput, medium complexity recipes
- [ ] **Coolant:** fluid intake (default color), same pipe pattern as Fusion
- [ ] **Belt lane draw data** for all 3 inputs

### Antimatter Reactor

- [ ] **Model:** `AntimatterReactor.fbx` + icon
- [ ] **Footprint:** 8×8
- [ ] **Inputs:** 5 belt inputs + mixed-color coolant (not default fluid color)
- [ ] **Belt lane draw data** for all 5 inputs
- [ ] **Pipe connector** for mixed coolant color

**Modeling notes (all new buildings):**

- Match game scale — use Micro Reactor FBX as reference for tile alignment and height.
- Multi-tile buildings (5×5, 8×8) need connector layout + `WithPreferredPlacement` decided before mesh work.
- Placeholder cubes OK for sim work; swap FBX when art ready.

---

## Power Distribution (post-V1)

V1 uses platform/rail graphs only (no poles). Poles + transformers come after generators ship.

### Distribution rules (target design)

| Layer | How power moves |
|---|---|
| **Platform / rail** | Generator on platform → all machines on same connected platform/rail cluster get power |
| **Power pole** | Separate wire network; carries power between poles (not platforms) |
| **Transformer** | Bridges platform-network energy **into** pole network when a pole is **not** on the same cluster as a generator |

Flow:

```text
Generator → platform/rail cluster → (optional) Transformer → pole wire network → distant loads
```

- Pole **on** generator platform/rail cluster: inherits power directly — no transformer needed.
- Pole **off** cluster: needs a **Transformer** on a powered platform to feed the pole network.
- Transformer consumes a slice of platform-network supply and pushes equivalent capacity onto connected poles.

### Power Pole

- [ ] **Model:** `PowerPole.fbx` + icon (lean tower or game-style pylon)
- [ ] **Footprint:** 1×1 (or 1×1 platform tile)
- [ ] **Wire graph:** adjacent poles (or defined range) = one pole network
- [ ] **No shape/fluid inputs** — passive carrier only
- [ ] **Asset extras:** optional wire/rope visual between linked poles (mesh or line renderer TBD)

### Transformer

- [ ] **Model:** `Transformer.fbx` + icon
- [ ] **Footprint:** 2×2 or 3×3 (TBD)
- [ ] **Placement:** on a **powered platform** (generator on platform or rail-linked to one)
- [ ] **Outputs:** connects to adjacent power pole(s) — converts platform-network energy to pole-network supply
- [ ] **Gameplay:** only active when platform cluster has surplus; pole side gets `min(surplus, transformer rating)`
- [ ] **No fuel** — conversion loss/rating TBD (e.g. 90% efficiency cap)

**Distribution modeling notes:**

- Poles and transformers are simpler meshes than generators — no belt lanes or pipe connectors.
- Decide pole link rules (orthogonal adjacency vs diagonal vs max wire length) before wire art.
- `PowerNetworkManager` will need a second graph type (pole networks) plus transformer bridge logic.
