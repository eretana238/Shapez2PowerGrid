# Development

## Prerequisites

- [Shapez 2](https://store.steampowered.com/app/2162800/shapez_2/) (PC)
- [.NET SDK](https://dotnet.microsoft.com/download) — targets `netstandard2.1`
- [Shapez Shifter](https://steamcommunity.com/sharedfiles/filedetails/?id=3542611357) (Steam Workshop) — required dependency
- [HarmonyX](https://steamcommunity.com/sharedfiles/filedetails/?id=3720742050) (Steam Workshop) — required dependency

## Environment Variables

| Variable | Purpose |
|---|---|
| `SPZ2_PATH` | Game install folder (contains game DLLs) |
| `SPZ2_PERSISTENT` | `%USERPROFILE%\AppData\LocalLow\tobspr Games\shapez 2` — mod output folder |
| `SPZ2_SHIFTER` | Path to `ShapezShifter.dll` |

Build output goes to `$(SPZ2_PERSISTENT)\mods\Powergrid\` (see `PowerGrid.csproj`).

## Build

```powershell
dotnet build PowerGrid.csproj -c Release
```

After a successful build, enable **PowerGrid** in the in-game mod list and restart if prompted.

## Tooling

- **Shapez Shifter** — recommended API layer for buildings, patching, and backwards compatibility.
- Add assembly ref via `SPZ2_SHIFTER`. Dependency is declared in `manifest.json`.
- **Publicizer** (`Krafs.Publicizer` in `.csproj`) exposes private/internal game members. Log output: `logs/krafs.log` (gitignored).
- Sample mods: [shapez2-mod-samples](https://github.com/tobspr-games/shapez2-mod-samples) — DiagonalCutter, BiggerPlatforms, SandboxIslands.

## Project Layout

```text
PowerGrid/
├── Power/              Network manager, producers, consumers, profiles
├── MicroReactor/       First generator simulation
├── Rendering/          Building renderers and belt lane draw data
├── Hud/                Debug overlay and HUD patches
├── UI/                 Custom UI factory
├── Toolbar/            Toolbar tab integration
├── Resources/          FBX meshes, icons (copied to output)
├── manifest.json       Mod metadata and dependencies
├── translations.json   In-game strings
└── Docs/               Design, roadmap, and reference docs
```

## Publishing

```xml
<!-- In .csproj -->
<Target Name="SteamPublish">
  <Exec Command='sh .\Steam\SteamPublish.sh "$(OutputPath)'/>
</Target>
```

```powershell
dotnet msbuild .\PowerGrid.csproj -t:SteamPublish -v:detailed
```

- Script requires `preview.png` in the Steam publish folder.
- Add mod dependencies on the Steam Workshop page so users get the "Subscribe to All" prompt.

## Common Errors

| Error | Fix |
|---|---|
| No `IMod` implementation found | Add class implementing `IMod`, or ignore if no init needed |
| Assembly loaded twice | Remove duplicate local + Steam copy of same mod |
| Mods not in list | Validate game files, restart Steam, re-subscribe |
| Unresolved references | Check env vars; restart IDE; use Rider's "Diagnose Reference" |

See also [Modding Reference](modding-reference.md) for Shapez 2 architecture and resource loading.
