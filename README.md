# PowerGrid

A [Shapez 2](https://store.steampowered.com/app/2162800/shapez_2/) mod that adds electricity to your factory. Generators feed connected platform and rail networks; machines draw power and slow down when supply falls short of demand.

No poles in V1 — if platforms and rails link to a generator, everything on that cluster shares the same network.

## What it does

- **Platform-based power networks** — connected platforms and rails form one grid; isolated islands stay separate.
- **Supply vs demand** — when generation cannot meet load, all powered machines run at reduced speed (proportional slowdown, not random brownouts).
- **Micro Reactor** — first playable generator; 2×2 building that burns shape items for ~500 kW.
- **Debug readout** — HUD panel for network stats during development.

Planned: Solar Array, Fusion Reactor, Quantum Generator, Antimatter Reactor, plus power poles and transformers for long-range distribution. See [Docs/Roadmap](Docs/roadmap.md).

## Requirements

Subscribe on Steam Workshop (when published) or build from source.

| Dependency | Workshop |
|---|---|
| [Shapez Shifter](https://steamcommunity.com/sharedfiles/filedetails/?id=3542611357) | Required |
| [HarmonyX](https://steamcommunity.com/sharedfiles/filedetails/?id=3720742050) | Required |

Enable **PowerGrid** in the in-game mod list after installing dependencies.

## Quick start (players)

1. Subscribe to Shapez Shifter and HarmonyX, then PowerGrid.
2. Start or load a save; enable the mod if prompted.
3. Place a **Micro Reactor** on a platform and feed it shapes.
4. Build machines on the same connected platform/rail cluster — they draw from the shared network.
5. Watch the debug panel if generation and consumption look off.

**Tip:** A generator on a platform powers that platform and any platform reachable by rails. Belts and pipes do not carry electricity.

## Quick start (developers)

```powershell
# Set SPZ2_PATH, SPZ2_PERSISTENT, SPZ2_SHIFTER — see Docs/development.md
dotnet build PowerGrid.csproj -c Release
```

Full setup, project layout, and troubleshooting: **[Docs/Development](Docs/development.md)**

## Documentation

| Doc | Description |
|---|---|
| [Docs/](Docs/README.md) | Documentation index |
| [Design](Docs/design.md) | Power rules, equations, V1 scope |
| [Roadmap](Docs/roadmap.md) | Generators, poles, transformers, assets |
| [Development](Docs/development.md) | Build, env vars, publishing |
| [Modding Reference](Docs/modding-reference.md) | Shapez 2 mod API notes |

## Repository layout

```text
PowerGrid/
├── Power/           Network simulation and machine profiles
├── MicroReactor/    Shape-fueled generator
├── Rendering/       3D rendering and belt animations
├── Hud/             Stats overlay
├── Resources/       Meshes and icons
└── Docs/            Design and dev docs
```

## License

MIT — see [LICENSE](LICENSE).
