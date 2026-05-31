# PowerGrid

A [Shapez 2](https://store.steampowered.com/app/2162800/shapez_2/) mod that adds electricity to your factory. Generators feed connected platform and rail networks; machines draw power and slow down when supply falls short of demand.

No poles in V1 — if platforms and rails link to a generator, everything on that cluster shares the same network.

## What it does

- **Platform-based power networks** — connected platforms and rails form one grid; isolated islands stay separate.
- **Supply vs demand** — when generation cannot meet load, all powered machines run at reduced speed (proportional slowdown, not random brownouts).
- **Micro Reactor** — first playable generator; 2×2 building that burns shape items for ~500 kW.
- **Power HUD** — in-game panel showing generation, usage, ratio, and efficiency so players can balance supply and demand.

Planned: Solar Array, Fusion Reactor, Quantum Generator, Antimatter Reactor, plus power poles and transformers for long-range distribution. See [Docs/Roadmap](Docs/roadmap.md).

## Requirements

Subscribe on Steam Workshop (when published) or build from source.

| Dependency | Workshop |
|---|---|
| [Shapez Shifter](https://steamcommunity.com/sharedfiles/filedetails/?id=3542611357) | Required |
| [HarmonyX](https://steamcommunity.com/sharedfiles/filedetails/?id=3720742050) | Required |

## Quick start (players)

**In development** — PowerGrid is not on Steam Workshop yet. Once published, follow these steps:

1. Subscribe to **PowerGrid** on the [Shapez 2 Steam Workshop](https://steamcommunity.com/app/2162800/workshop/).
2. When prompted, choose **Subscribe to All** so dependencies (Shapez Shifter, HarmonyX) install automatically.
3. Launch Shapez 2.

## Quick start (developers)

**Windows development** — steps below assume Shapez 2 on Steam for Windows. Linux/macOS paths and env setup differ; use the [official modding docs](https://tobspr-games.notion.site/shapez2-modding-documentation) for your platform.

PowerGrid builds against game DLLs on your machine — not NuGet. Set three environment variables before opening the project or building.

| Variable | What it points to |
|---|---|
| `SPZ2_PATH` | Shapez 2 install folder — where `SPZGameAssembly.dll` and other game assemblies live (used as `<HintPath>` references in the `.csproj`) |
| `SPZ2_PERSISTENT` | Game persistent data folder — `%USERPROFILE%\AppData\LocalLow\tobspr Games\shapez 2`. Build output is copied to `mods\Powergrid\` here so the game loads your local build |
| `SPZ2_SHIFTER` | Full path to `ShapezShifter.dll` from the subscribed [Shapez Shifter](https://steamcommunity.com/sharedfiles/filedetails/?id=3542611357) Workshop mod |

Set them in Windows (User env vars, then restart IDE/terminal):

```powershell
[System.Environment]::SetEnvironmentVariable("SPZ2_PATH", "C:\Program Files (x86)\Steam\steamapps\common\shapez 2", "User")
[System.Environment]::SetEnvironmentVariable("SPZ2_PERSISTENT", "$env:USERPROFILE\AppData\LocalLow\tobspr Games\shapez 2", "User")
[System.Environment]::SetEnvironmentVariable("SPZ2_SHIFTER", "C:\Program Files (x86)\Steam\steamapps\workshop\content\2162800\3542611357\ShapezShifter.dll", "User")
```

Adjust paths if Steam or Workshop IDs differ on your PC — see the [official modding docs](https://tobspr-games.notion.site/shapez2-modding-documentation).

Then build and test in-game:

```powershell
dotnet build PowerGrid.csproj -c Release
```

More detail (project layout, publishing, common errors): **[Docs/Development](Docs/development.md)**

## Documentation

| Doc | Description |
|---|---|
| [Docs/](Docs/README.md) | Documentation index |
| [Design](Docs/design.md) | Power rules, equations, V1 scope |
| [Roadmap](Docs/roadmap.md) | Generators, poles, transformers, assets |
| [Development](Docs/development.md) | Build, env vars, publishing |
| [Modding Reference](Docs/modding-reference.md) | Shapez 2 mod API notes (this repo) |
| [Official modding docs](https://tobspr-games.notion.site/shapez2-modding-documentation) | Environment setup, IDE, publishing (tobspr) |
| [Art guidelines](https://tobspr-games.notion.site/shapez2-modding-art-guidelines) | Meshes, icons, scale, visual style (tobspr) |

## Repository layout

```text
PowerGrid/
├── Power/           Network simulation and machine profiles
├── MicroReactor/    Shape-fueled generator
├── Rendering/       3D rendering and belt animations
├── Hud/             Power HUD (generation, usage, efficiency)
├── Resources/       Meshes and icons
└── Docs/            Design and dev docs
```

## License

MIT — see [LICENSE](LICENSE).
