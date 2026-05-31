# Shapez 2 Modding Reference

General reference for modding Shapez 2. PowerGrid-specific design lives in [Design](design.md).

## Tooling

- **Shapez Shifter** (Steam Workshop ID: 3542611357) — recommended API layer. Handles patching, backwards-compat, and building addition helpers.
- Add assembly ref via env var `SPZ2_SHIFTER`. Add to `manifest.json`: `"Dependencies":[{"ModId":"steam:3542611357","Version":"<1.0.0"}]`
- Use **publicizer** (`PublicizeAll=true` in `.csproj`) to expose private/internal members. Exclude files that cause name conflicts.
- Sample mods: DiagonalCutter, BiggerPlatforms, SandboxIslands ([shapez2-mod-samples](../../shapez2-mod-samples))
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

## Shapez Shifter Layers

| Layer | Power | Use case |
|---|---|---|
| `Sharp Detours` | Highest | Replace/prefix/postfix any method; requires deep game knowledge; **avoid for cross-mod compatibility** |
| `Hijack` | Mid | Intercept and rewire game structures |
| `Flow` | Lowest | High-level API for adding buildings (entity+render+toolbar+research+placement+sim) |
| `Kit` | Utility | Misc convenience methods |

## Resource Loading

**Reference mod:** [`shapez2-mod-samples/DiagonalCutter`](../../shapez2-mod-samples/DiagonalCutter) — especially `Resources/DiagonalCutter` (embedded AssetBundle + `.manifest`) and loose files in `Resources/` (FBX mesh, PNG icon). See `DiagonalCutterMod.cs` for `AssetBundleHelper.CreateForAssetBundleEmbeddedWithMod`, `ModFolderLocator`, and `FileMeshLoader` / `FileTextureLoader` usage.

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

## Initialization Order (abbreviated)

Mods load at **`PreloaderModsLoaderState`** — after settings, language, sounds, and core game data are initialized, but before specs, graphics settings, and the main menu.

Key steps before mod load: `InitSettings → Load Language → Init Sounds → Load Builtin GameData`

Key steps after mod load: `PreloaderSpecsState → graphics settings → Main Menu → GameCore.Init`
