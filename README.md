# Shapez 2 Modding Reference

## Tooling
- **Shapez Shifter** (Steam Workshop ID: 3542611357) — recommended API layer. Handles patching, backwards-compat, and building addition helpers.
- Add assembly ref via env var `SPZ2_SHIFTER`. Add to `manifest.json`: `"Dependencies":[{"ModId":"steam:3542611357","Version":"<1.0.0"}]`
- Use **publicizer** (`PublicizeAll=true` in .csproj) to expose private/internal members. Exclude files that cause name conflicts.
- Sample mods: DiagonalCutter, BiggerPlatforms, SandboxIslands (GitHub: tobspr-games/shapez2-mod-samples)
- **Skipping Shapez Shifter** (not recommended): requires IL patching via MonoMod, Harmony, or Cecil; high risk of incompatibility with other mods.
- **`meta.json`** is required for every mod to load, even content-only mods.

## Content Loading (no code required)
- `scenarios/*.json` — gameplay scenarios (support `#include` tags for modular/reusable JSON composition)
- `scenario-presets/*.json` — map gen presets
- `translations.json` — merged into game translations at load time

**Translation rules:**
- Missing language keys fall back to the English entry in the mod
- If a mod overrides an English key but provides no translation for language X, language X gets the mod's English string (not the original game string for that language)
- Keys not defined in the mod's English block are ignored in all other languages

## Core Architecture

### Key Classes
| Class | Role |
|---|---|
| `ISimulationState` | Stores simulation data; must be serializable/deserializable |
| `ISimulation` / `Simulation<TState>` | Logic unit; no spatial awareness; works with indexed I/O |
| `IConnectableSimulation` | Simulation with spatial connectors; links to other sims |
| `SimulationGraph` | Manages connectable sims in parallel-processed clusters |
| `Entity` | Map object = Definition + Transform + State + Configuration |
| `SimulationSystem` | Pattern-matches map layout → creates/destroys simulations |
| `AtomicBuildingSimulationSystem` | Helper for 1-building → 1-simulation pattern |

### Simulation Patterns
- **1:1** (Rotator) — one building → one simulation
- **2:1** (Belt Port) — two buildings → one simulation; destroyed if either removed
- **N:1** (Conveyor) — N connected belts → one simulation; state distributed across belt entities
- **1:2** (Painter) — one building → multiple simulations (considered tech debt)

## PowerGrid V1 Design

No poles or wires. Power distributes automatically across connected rail/platform networks.

### Rules
- Connected platforms/rails = one power network.
- Disconnected islands = separate power networks.
- If a platform path exists, power is shared.
- **Platform power:** machines on a platform are powered when a generator sits on that platform **or** when the platform connects to a powered platform cluster via **rails** (same connected graph as the generator).
- No native power API exists; build a custom system.
- Belts, pipes, and transport-only entities never use energy.

### Core Systems
- `PowerNetworkManager`
  - Finds connected platform clusters.
  - Builds separate power networks.
  - Calculates supply/demand.
  - Applies efficiency multiplier.
- `PowerProducerSimulation`
  - Generates power as negative consumption.
- `PowerConsumerSimulation`
  - Calculates machine power usage.

### Network Equations

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

Behavior:

- `R >= 1`: full speed.
- `R < 1`: all powered machines slow proportionally.
- Example: if needed power is `500 kW` and generated power is `250 kW`, ratio is `0.5` and all powered machines run at `50%` speed.

### Implementation

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

Do not rely on:

- Native energy API.
- `WithEfficiencyData()` because it currently has a hardcoded issue.
- `BuffableBeltDelay.DiscreteDuration`.
- HUD APIs for V1.

Instead:

- Use custom tick/update timers.
- Ship V1 with logs/debug overlay.
- Use placeholder assets if needed.

V1 scope:

- Include power generation.
- Include consumption.
- Include platform-based networks.
- Include efficiency slowdown.
- Include Micro Reactor.
- Include debug readout.

Skip for V1:

- Poles.
- HUD.
- Fancy assets.
- Native integrations.

## TODOs — Power Generators & Models

Each generator needs the same asset types as Micro Reactor (see `Resources/MicroReactor.fbx`, `PowerGrid_Icon.png`):

| Asset | Purpose |
|---|---|
| **FBX mesh** | Building body; single- or multi-mesh; load via `FileMeshLoader` or Assimp |
| **PNG icon** | Toolbar + building variant icon (`FileTextureLoader.LoadTextureAsSprite`) |
| **Belt lane draw data** | Animated item paths on belt inputs (see `MicroReactorDrawData`, `MicroReactorBeltLaneRenderingDefinition`) |
| **Fluid pipe connectors** | Fusion / Quantum / Antimatter only — coolant intake on pipe network |

Per-building checklist:

### Micro Reactor — done
- [x] 2×2 footprint, 1 shape input (fuel)
- [x] `MicroReactor.fbx`, icon, simulation, renderer

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

## TODOs — Power Distribution (post-V1)

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

## Shapez Shifter Layers
| Layer | Power | Use case |
|---|---|---|
| `Sharp Detours` | Highest | Replace/prefix/postfix any method; requires deep game knowledge; **avoid for cross-mod compatibility** |
| `Hijack` | Mid | Intercept and rewire game structures |
| `Flow` | Lowest | High-level API for adding buildings (entity+render+toolbar+research+placement+sim) |
| `Kit` | Utility | Misc convenience methods |

## Resource Loading

**Reference mod:** [`shapez2-mod-samples/DiagonalCutter`](../shapez2-mod-samples/DiagonalCutter) — especially `Resources/DiagonalCutter` (embedded AssetBundle + `.manifest`) and loose files in `Resources/` (FBX mesh, PNG icon). See `DiagonalCutterMod.cs` for `AssetBundleHelper.CreateForAssetBundleEmbeddedWithMod`, `ModFolderLocator`, and `FileMeshLoader` / `FileTextureLoader` usage.

```csharp
// Single mesh FBX
Mesh m = FileMeshLoader.LoadSingleMeshFromFile("path.fbx");

// Multi-mesh FBX
using AssimpContext importer = new();
Scene scene = importer.ImportFile(fbxPath, PostProcessPreset.TargetRealTimeMaximumQuality);
foreach (Assimp.Mesh assimpMesh in scene.Meshes)
{
    var mesh = AssimpToUnityMeshConverter.ConvertStaticMesh(assimpMesh);
}
// scene also exposes: scene.Textures, scene.Materials, scene.Cameras

// Texture / Sprite (PNG, JPG, EXR supported)
FileTextureLoader.LoadTexture("path.png");
FileTextureLoader.LoadTextureAsSprite("path.png", out var tex);
// ⚠ Destroy texture when done to avoid memory leaks

// Shader/Material — no runtime shader compilation; use AssetBundles built in Unity Editor
// AssetBundles also support: models, textures, audio, and more
var bundle = AssetBundle.LoadFromFile(path);
var shader = bundle.LoadAsset<Shader>("Assets/Shaders/MyShader.hlsl");
var mat = bundle.LoadAsset<Material>("Assets/Materials/MyMat.mat");
var mat2 = new Material(shader); // or create from shader directly
```

## Mod Lifetime
- Loaded at `PreloaderModsLoaderState` (early preload, single-threaded, deterministic)
- Persists entire session; unloaded only on game shutdown
- Must implement `IMod` interface or mod is skipped (non-fatal if no init needed)

## Publishing
```xml
<!-- In .csproj -->
<Target Name="SteamPublish">
  <Exec Command='sh .\Steam\SteamPublish.sh "$(OutputPath)'/>
</Target>
```
```powershell
dotnet msbuild .\MyMod.csproj -t:SteamPublish -v:detailed
```
- Script requires `preview.png` in the Steam publish folder
- Also add mod dependencies on the Steam Workshop page so users get the "Subscribe to All" prompt

## Initialization Order (abbreviated)
Mods load at **`PreloaderModsLoaderState`** — after settings, language, sounds, and core game data are initialized, but before specs, graphics settings, and the main menu.

Key steps before mod load: `InitSettings → Load Language → Init Sounds → Load Builtin GameData`
Key steps after mod load: `PreloaderSpecsState → graphics settings → Main Menu → GameCore.Init`

## Common Errors
| Error | Fix |
|---|---|
| No `IMod` implementation found | Add class implementing `IMod`, or ignore if no init needed |
| Assembly loaded twice | Remove duplicate local+Steam copy of same mod |
| Mods not in list | Validate game files, restart Steam, re-subscribe |
| Unresolved references | Check env vars; restart IDE/computer; use Rider's "Diagnose Reference" |

